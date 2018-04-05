using System;
using System.Collections.Generic;

namespace RepositoryModel
{
    /**
     * Class for testing RepositoryGetter methods, in our case something like controller
     */
    public static class MainRunner
    {
        private static RepositoryGetter getter;
        private static readonly List<string> fileExt = new List<string>();
        
        public static void Main(string[] args)
        {         
            // Create new repository getter
            getter = new RepositoryGetter("martinspetlik", "TGH");
            getter.Authentication().Wait();

            // Check if repository exists, it is not part of contructor, if repository is changed make sure that this method is called
            var repositoryExists = getter.CheckRepository().Result;
            Console.WriteLine("REPOSITORY EXISTS " + repositoryExists);

            if (!repositoryExists) return;
            
            // User name and repository are really set
            Console.WriteLine("USER NAME " + getter.UserName);
            Console.WriteLine("REPOSITORY NAME " + getter.RepoName);

            // Network checker
            Console.WriteLine("NETWORK " + System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable());

            // Testing file extension
            fileExt.Add("java");
            getter.FileExtensions = fileExt;
            getter.FileExtensions.ForEach(Console.WriteLine);

            //Octokit methods are async so Wait() is necessary
            // Auth for testing, increase number of requests per hour
            

            Console.WriteLine("REPOSITORY CONTENT");
            getter.RepositoryContent().Wait();


            getter.LastChangedFiles().Wait();


            // Get file content
            getter.getFileContent("TGH/Hrana.java").Wait();
        }
    }
}