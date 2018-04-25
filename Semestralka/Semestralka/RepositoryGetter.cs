
using System;
using System.Threading.Tasks;
using Octokit;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Windows;
using Semestralka;
using System.Threading;

namespace RepositoryModel
{
    public class RepositoryGetter
    {
        // Github client
        private GitHubClient client;

        // User and repository name
        public string UserName { get; }
        public string RepoName { get; }

        // List of all available file extensions 
        public List<string> FileExtensions { get; set; } = new List<string>();

        // Repository
        private Repository repository { get; set; }

        // Number of lines for file extensions (suffix), currently only .java
        private IDictionary<string, int> ExtensionsLines = new Dictionary<string, int>();

        // Files changed during app run
        public IDictionary<string, List<string[]>> FilesChanges = new Dictionary<string, List<string[]>>();

        /**
         * Constructor
         * params: user name adn repository name
         */
        public RepositoryGetter(string userName, string repoName)
        {
            UserName = userName;
            RepoName = repoName;
            // Set github client
            client = new GitHubClient(new ProductHeaderValue("stiapp"));
        }

        /**
         * Create new repository getter with all neccessary methods
         * 
         */
        public static RepositoryGetter CreateNewRepositoryGetter(string url)
        {
            // Getting user name and repository name from github url
            var userNameRepositoryName = RepositoryGetter.parseUrl(url);
            // Create new repository getter
            RepositoryGetter getter = new RepositoryGetter(userNameRepositoryName.Item1, userNameRepositoryName.Item2);
            // Authetification for 5000 request per hour
            getter.Authentication().Wait();
            // Testing file extension
            //fileExt.Add("java");
            getter.FileExtensions.Add("java");// = fileExt;

            // Check if repository exists, it is not part of contructor, if repository is changed make sure that this method is called
            var repositoryExists = getter.CheckRepository().Result;
            return !repositoryExists ? null : getter;
        }

        /**
         * Parse url
         * Getting github user name and repository name from github repository url
         * return Tuple
         */
        public static Tuple<string, string> parseUrl(string url)
        {
            Uri uri = new Uri(url);
            // Repository id is user name and repository name
            var ids = new List<string>();
            // Url segments, split by '/'
            var segments = uri.Segments.ToList();
            foreach (var st in segments)
            {
                // Skip items first items with just '/'
                if (st.Length > 1)
                {
                    ids.Add(st);
                }
                // First two items are used
                if (ids.Count == 2)
                {
                    break;
                }
            }

            return ids.Count != 2 ? Tuple.Create("", "") : Tuple.Create(ids[0].TrimEnd('/'), ids[1].TrimEnd('/'));
            // Tuple (user name, repository name)
        }

        /**
         * Getting file extension from file name or file path
         * return string
         */
        private string getFileExtension(string fileName)
        {
            var lastIndex = fileName.LastIndexOf('.');
            return fileName.Substring(lastIndex + 1);
        }


        /**
         * Check if repository exists
         * return bool
         */
        public async Task<bool> CheckRepository()
        {
            try
            {
                repository = await client.Repository.Get(UserName, RepoName).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /**
         * Get all repository commits
         * return GitHubCommit
         */
        public async Task<IReadOnlyList<GitHubCommit>> repositoryCommits()
        {
            return await client.Repository.Commit.GetAll(UserName, RepoName).ConfigureAwait(false);
        }

        /** 
         * Getting all repository files path
         */
        public async Task<IEnumerable<string>> repositoryFilePaths()
        {
            var files = await client.Git.Tree.GetRecursive(UserName, RepoName, "master").ConfigureAwait(false);
            return files.Tree.Select(x => x.Path);
        }


        /**
         * File extensions with number of lines from whole repository
         * return Dictionary extension:number of lines
         */
        public async Task<IDictionary<string, int>> FileExtensionsLinesNumber()
        {
            ExtensionsLines = new Dictionary<string, int>();
            // Loop through all repository files (as path)
            foreach (var path in repositoryFilePaths().Result)
            {
                // Get file extension (suffix)
                var ext = getFileExtension(path);
                // Add suffix to list of all extensions and add number of lines for selected path
                if (FileExtensions.Contains(ext))
                {
                    if (ExtensionsLines.ContainsKey(ext))
                    {
                        ExtensionsLines[ext] += FileLineNumber(path);
                    }
                    else
                    {
                        ExtensionsLines.Add(ext, FileLineNumber(path));
                    }
                }
            }
            return ExtensionsLines;
        }

        /**
         * Return dictionary of files with their commits
         * Pass date of last visiting this app
         */
        public async Task<IDictionary<string, List<string[]>>> ChangedFiles(DateTime date)
        {
            
            // Latest commit is first in list commits
            var commits = repositoryCommits().Result;

            // Loop thourgh all commits
            foreach (var currentCommit in commits)
            {
                // Commit detail
                var commit = client.Repository.Commit.Get(UserName, RepoName, currentCommit.Sha);

                // Commit after date (method parameter)
                if (DateTimeOffset.Compare(commit.Result.Commit.Author.Date, new DateTimeOffset(date)) > 0)
                {
                    foreach (GitHubCommitFile file in commit.Result.Files)
                    {
                        // Information about file in current commit
                        var version = new string[6];

                        var url = file.ContentsUrl.Split(new[] { "/contents/" }, StringSplitOptions.None)[1];
                        var fileData = url.Split(new[] { "?ref=" }, StringSplitOptions.None);

                        // Save information about type of file
                        if (isFileBinary(fileData, commit.Result).Result)
                        {
                            version[1] = "binary";
                        }
                        else
                        {
                            version[1] = "text";
                        }

                        version[0] = file.Changes.ToString(); // Number of file changes
                        version[2] = commit.Result.Commit.Author.Date.ToString("dd.MM.yyyy HH:mm:ss"); // Date of commit 
                                                                                                       // Commit identificator
                        version[3] = commit.Result.Sha;
                        // Save file name
                        version[4] = fileData[0];
                        // Save file request for deleted or renamed files
                        version[5] = fileData[1];


                        // Create new item in dictionary
                        if (FilesChanges.ContainsKey(file.Filename))
                        {
                            FilesChanges[file.Filename].Add(version);
                        }
                        // Append file info from other commit
                        else
                        {
                            var fileVersions = new List<string[]>();
                            fileVersions.Add(version);
                            FilesChanges.Add(file.Filename, fileVersions);
                        }
                    }
                }
            }
            return FilesChanges;
        }

        /**
         * Check if file is binary or not
         * return bool
         */
        public async Task<bool> isFileBinary(string[] fileData, GitHubCommit commit)
        {
            IReadOnlyList<RepositoryContent> contents;

            try
            {
                // Check if file is binary
                contents = await client.Repository.Content
                    .GetAllContentsByRef(UserName, RepoName, fileData[0], commit.Sha).ConfigureAwait(false);
                var targetFile = contents[0];

                // Download file
                string con;
                using (var wc = new WebClient())
                    con = wc.DownloadString(targetFile.DownloadUrl);

                // Check if file is binary
                for (int i = 1; i < 512 && i < con.Length; i++)
                {
                    if (con[i] == 0x00 && con[i - 1] == 0x00)
                    {
                        return true;
                    }
                }

            }
            catch
            {   // Chck if file is binary for deleted or renamed files
                contents = await client.Repository.Content
                    .GetAllContentsByRef(UserName, RepoName, fileData[0], fileData[1]).ConfigureAwait(false);
                var targetFile = contents[0];

                string con;
                using (var wc = new WebClient())
                    con = wc.DownloadString(targetFile.DownloadUrl);

                // Check if file is binary
                for (int i = 1; i < 512 && i < con.Length; i++)
                {
                    if (con[i] == 0x00 && con[i - 1] == 0x00)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /**
         * Save text file
         * return void
         */
        public async Task saveBinaryFile(string fileName, string fileRequest, string fileDestination, string commitSha)
        {
            IReadOnlyList<RepositoryContent> contents;
            byte[] binaryContent;

            try
            {
                // File content
                contents = await client.Repository.Content
                    .GetAllContentsByRef(UserName, RepoName, fileName, commitSha).ConfigureAwait(false);
                var targetFile = contents[0];
                binaryContent = new WebClient().DownloadData(targetFile.DownloadUrl);

            }
            catch
            {   // File content for deleted or renamed files during app run
                contents = await client.Repository.Content
                    .GetAllContentsByRef(UserName, RepoName, fileName, fileRequest).ConfigureAwait(false);
                var targetFile = contents[0];
                binaryContent = new WebClient().DownloadData(targetFile.DownloadUrl);
            }

            // Create directory if doesn't exists
            FileInfo fi = new FileInfo(fileDestination);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }
            // Write bytes to file
            File.WriteAllBytes(fileDestination, binaryContent);
        }

        /**
         * Save text file
         * return void
         */
        public async Task saveTextFile(string fileName, string fileRequest, string fileDestination, string commitSha)
        {
            IReadOnlyList<RepositoryContent> contents;
            var fileContent = "";

            try
            {
                // File content
                contents = await client.Repository.Content
                    .GetAllContentsByRef(UserName, RepoName, fileName, commitSha).ConfigureAwait(false);

                var targetFile = contents[0];
                fileContent = targetFile.EncodedContent != null
                    ? Encoding.UTF8.GetString(Convert.FromBase64String(targetFile.EncodedContent))
                    : targetFile.Content;

            }
            catch
            {   // File content for deleted or renamed files during app run
                contents = await client.Repository.Content
                    .GetAllContentsByRef(UserName, RepoName, fileName, fileRequest).ConfigureAwait(false);
                var targetFile = contents[0];
                fileContent = targetFile.EncodedContent != null
                        ? Encoding.UTF8.GetString(Convert.FromBase64String(targetFile.EncodedContent))
                        : targetFile.Content;

            }

            // Create directory if doesn't exists
            FileInfo fi = new FileInfo(fileDestination);
            if (!fi.Directory.Exists)
            {
                Directory.CreateDirectory(fi.DirectoryName);
            }
            // Write to file
            File.WriteAllText(fileDestination, fileContent);
        }

        /**
         * Getting file content from file name (file path) and commit sha
         * return string (text in UTF-8)
         */
        public async Task<string> getFileContent(string file)
        {
            // All contents of repository
            var contents = await client.Repository.Content.GetAllContentsByRef(UserName, RepoName, file, "master").ConfigureAwait(false);
            // Searched file
            var targetFile = contents[0];
            var currentFileText = targetFile.EncodedContent != null ?
                Encoding.UTF8.GetString(Convert.FromBase64String(targetFile.EncodedContent)) :
                targetFile.Content;

            return currentFileText;
        }

        /**
         * Save files - text file or binary
         * return void
         */
        public void SaveFile(string targetDirectory, IDictionary<string, List<string[]>> filesPath)
        {
            if (!Directory.Exists(targetDirectory)) return;
            // Go through all files -> same dictionary structure like FilesChanges
            foreach (KeyValuePair<string, List<string[]>> entry in filesPath)
            {
                // File path from repository
                var path = entry.Key;
                path = path.Replace("/", "\\");

                // Commit identification
                var commitSha = entry.Value[0][3];
                // File name
                var fileName = entry.Value[0][4];
                // File request - for files that were deleted or renamed during the program run
                var fileRequest = entry.Value[0][5];
                // File destination
                var targetFile = targetDirectory + "\\" + path;

                if (entry.Value[0][1] == "binary")
                {
                    // Save binary files
                    saveBinaryFile(fileName, fileRequest, targetFile, commitSha).Wait();
                }
                else
                {
                    // Save text files
                    saveTextFile(fileName, fileRequest, targetFile, commitSha).Wait();
                }
            }
        }

        /**
        * File number of lines
        * return int
        */
        private int FileLineNumber(string filePath)
        {
            var fileContent = getFileContent(filePath).Result;
            return fileContent.Split('\n').Length;
        }

        /**
         * Authetification for 5000 requests per hour
         */
        public async Task Authentication()
        {
            var basicAuth = new Credentials("stiapp", "pecinasoučekšpetlík");
            client.Credentials = basicAuth;
        }

        /**
          * Filter suffixes
          * return void
          */
        public void filterDict()
        {
            IDictionary<string, List<string[]>> filteredDict = new Dictionary<string, List<string[]>>();
            bool found = false;
            foreach (var unfilteredItem in FilesChanges)
            {
                found = false;
                foreach (var filter in FileExtensions)
                {
                    if (unfilteredItem.Key.Contains(filter))
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    filteredDict.Add(unfilteredItem);
                }
            }
            FilesChanges = filteredDict;
        }

        public void loadNewFilesChangesFromListString(List<string> listContent)
        {
            foreach (var line in listContent)
            {
                string[] parts = line.Split('|');
                var version = new string[6];

                version[0] = parts[2]; // Number of file changes
                version[2] = parts[1]; // Date of commit 
                version[3] = parts[3]; // Commit identificator
                version[4] = parts[0]; //name

                // Create new item in dictionary
                if (FilesChanges.ContainsKey(parts[0]))
                {
                    FilesChanges[parts[0]].Add(version);
                }
                // Append file info from other commit
                else
                {
                    var fileVersions = new List<string[]>();
                    fileVersions.Add(version);
                    FilesChanges.Add(parts[0], fileVersions);
                }
            }
        }
    }
}
