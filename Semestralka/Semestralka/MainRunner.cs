using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepositoryModel
{
    /**
     * Class for testing RepositoryGetter methods, in our case it is something like controller
     */
    public static class MainRunner
    {
        private static RepositoryGetter getter;
        private static readonly List<string> fileExt = new List<string>();
        
        /*public static void Main(string[] args)
        {
            //binary.makeBinary();
            
            //Environment.Exit(1);
            
            // Getting user name and repository name from github url
            var userNameRepositoryName = RepositoryGetter.parseUrl("https://github.com/martinspetlik/TGH");
 
            // Create new repository getter
            getter = new RepositoryGetter(userNameRepositoryName.Item1, userNameRepositoryName.Item2);
            // Authetification for 5000 request per hour
            getter.Authentication().Wait();

            // Check if repository exists, it is not part of contructor, if repository is changed make sure that this method is called
            var repositoryExists = getter.CheckRepository().Result;

            Console.WriteLine(repositoryExists);

            if (!repositoryExists) return;

            // Network checker
            //Console.WriteLine("NETWORK " + System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable());

            // Testing file extension
            fileExt.Add("java");
            getter.FileExtensions = fileExt;

            Console.WriteLine("Files extensions line number " + getter.FileExtensionsLinesNumber().Result["java"]);

            var filesExtensions = getter.ChangedFiles(new DateTime(2016, 4, 19, 20, 22, 12)).Result;

            //System.Environment.Exit(1);
            
            //var firstItem = filesExtensions.First();
            
            
            /*Console.WriteLine(firstItem.Key);
            Console.WriteLine(firstItem.Value[0][0]);
            Console.WriteLine(firstItem.Value[0][1]);
            Console.WriteLine(firstItem.Value[0][2]);*/
                     
            // Loop through filesExtensions
            // Dictionary {fileName: List(commit1[number of changes, file content], commit2, ...)}
            /*foreach(KeyValuePair<string, List<string[]>> entry in filesExtensions)
            {
                Console.WriteLine(entry.Key);
                /*foreach (string[] commit in entry.Value)
                {
                    // commit is array -> []
                    Console.WriteLine(string.Join(",", commit));
                }*/
                
            //}*
            
            //getter.SaveFile("/home/martin/Desktop/STI", filesExtensions);
        //}
    }
}
