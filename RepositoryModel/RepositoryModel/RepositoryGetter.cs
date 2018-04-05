
using System;
using System.Threading.Tasks;
using Octokit;
using System.Text;
using System.Collections.Generic;
using System.Linq;



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
         
         private readonly IDictionary<string, string>  FilesContents = new Dictionary<string, string>();

         
         public RepositoryGetter(string userName, string repoName)
         {
             UserName = userName;
             RepoName = repoName;
             // Set github client
             client = new GitHubClient(new ProductHeaderValue("stiapp"));
              
         }

         private string getFileExtension(string fileName)
         {
             int lastIndex = fileName.LastIndexOf('.');
             //var name = fileName.Substring(0,lastIndex);
             return fileName.Substring(lastIndex + 1);
         }



         /**
          * Check if repository exists
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
 
         
         public void AppendFileExtension(string extension)
         {         
             FileExtensions.Add(extension);
         }


         public async Task RepositoryContent()
         {
             // Use only files and directories on master branch
             //var contents = await client.Repository.Content.GetAllContents(repository.Id);
             //var contents = await client.Repository.Content.GetAllContentsByRef(UserName,  RepoName, "master");           

             //var commits = client.Repository.Commit.GetAll(repository.Id).Result;
             
             var files = await client.Git.Tree.GetRecursive (UserName, RepoName, "master"); 
             var filePaths = files.Tree.Select (x => x.Path);
             //var stats = new Dictionary<string, int>();
             foreach (var path in filePaths)
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
                 
                 
                 /*Console.WriteLine(path);
                 var request = new CommitRequest { Path = path };
                 var commitsForFile = await client.Repository.Commit.GetAll(UserName, RepoName,request);*/
             }          
         }
         
         /**
          * 
          *
          * Pass date of last visiting this app
          */
         public async Task LastChangedFiles()
         {
             // Get all commits for repository
             // Latest commit is first in list commits
             var commits = await client.Repository.Commit.GetAll(UserName, RepoName);


             DateTime date = new DateTime(2017, 4, 19, 20, 22, 12);


             // Loop thourgh all commits
             foreach (var currentCommit in commits)
             {
                 var commit = client.Repository.Commit.Get(UserName, RepoName, currentCommit.Sha);

                 var commitDate = commit.Result.Commit.Author.Date;
                 // Commit after last app run
                 if (DateTimeOffset.Compare(commitDate, new DateTimeOffset(date)) > 0)
                 {
                     // Files changed in current commit
                     var commitChangedFiles = commit.Result.Files.GetEnumerator();

                     while (commitChangedFiles.MoveNext())
                     {
                         Console.WriteLine(commitChangedFiles.Current.Filename);
                     }
                 }

             }
         }


         public async Task<string> getFileContent(string file)
         {
          
             var contents = await client.Repository.Content.GetAllContentsByRef(UserName,  RepoName, file, "master");
             var targetFile = contents[0];
             
             var currentFileText = targetFile.EncodedContent != null ? 
                 Encoding.UTF8.GetString(Convert.FromBase64String(targetFile.EncodedContent)) :
                 targetFile.Content;

             FilesContents.Add(file, currentFileText);

             return currentFileText;
         }

         private int FileLineNumber(string fileContent)
         {
             fileContent = getFileContent(fileContent).Result;
             
             return fileContent.Split('\n').Length;
         }


         public async Task Authentication()
         {          
            var basicAuth = new Credentials("martinspetlik", "heslo"); 
            client.Credentials = basicAuth;     
         }
    }
}