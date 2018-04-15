
using System;
using System.Threading.Tasks;
using Octokit;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;


namespace RepositoryModel
{    
     public class RepositoryGetter
     {
         // Github client
         private GitHubClient client;
         
         // User and repository name
         public string UserName { get;}
         public string RepoName { get;}

         // File extensions 
         public List<string> FileExtensions { get; set; } = new List<string>();
         
         
         private readonly IDictionary<string, int>  ExtensionsLines = new Dictionary<string, int>();

         private Repository repository { get; set; }
         
         private readonly IDictionary<string, List<string[]>> FilesChanges = new Dictionary<string, List<string[]>>();
         
         
      
         public RepositoryGetter(string userName, string repoName)
         {
             UserName = userName;
             RepoName = repoName;
             // Set github client
             client = new GitHubClient(new ProductHeaderValue("stiapp"));            
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
             // Tuple (user name, repository name)
             return Tuple.Create(ids[0].TrimEnd('/'), ids[1].TrimEnd('/'));
         }
         
         /**
          * Getting file extension from file name or file path
          * return string
          */
         private string getFileExtension(string fileName)
         {
             int lastIndex = fileName.LastIndexOf('.');
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
                 repository = await client.Repository.Get(UserName, RepoName);
             }
             catch (NotFoundException)
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
             return await client.Repository.Commit.GetAll(UserName, RepoName);
         }
         
         /** 
          * Getting all repository files path
          */
         public async Task<IEnumerable<string>> repositoryFilePaths()
         {
             var files = await client.Git.Tree.GetRecursive (UserName, RepoName, "master"); 
             return files.Tree.Select (x => x.Path);
         }
 

         /**
          * File extensions with number of lines from whole repository
          * return Dictionary extension:number of lines
          */
         public async Task<IDictionary<string, int>> FileExtensionsLinesNumber()
         {
             foreach (var path in repositoryFilePaths().Result)
             {
                 // Count file number of lines
                 var ext = getFileExtension(path);
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
                         // INformation about file in current commit
                         var version = new string[3];
                         version[0] = file.Changes.ToString(); // Number of file changes
                         version[1] = getFileFromCommit(commit.Result.Files[0].Filename, commit.Result).Result; // File content as string

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
         * Getting file content from file name (file path)
         * return string (text in UTF-8)
         */
         public async Task<string> getFileFromCommit(string file, GitHubCommit commit)
         {             
             // All contents of repository
             var contents = await client.Repository.Content.GetAllContentsByRef(UserName,  RepoName, file, commit.Sha);
             // Searched file
             var targetFile = contents[0];           
             var currentFileText = targetFile.EncodedContent != null ? 
                 Encoding.UTF8.GetString(Convert.FromBase64String(targetFile.EncodedContent)) :
                 targetFile.Content;

             return currentFileText;
         }

         /**
          * Getting file content from file name (file path) and commit sha
          * return string (text in UTF-8)
          */
         public async Task<string> getFileContent(string file)
         {      
             // All contents of repository
             var contents = await client.Repository.Content.GetAllContentsByRef(UserName,  RepoName, file, "master");
             // Searched file
             var targetFile = contents[0];           
             var currentFileText = targetFile.EncodedContent != null ? 
                 Encoding.UTF8.GetString(Convert.FromBase64String(targetFile.EncodedContent)) :
                 targetFile.Content;

             return currentFileText;
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
    }
}