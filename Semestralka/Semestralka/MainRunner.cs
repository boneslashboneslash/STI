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

        //public static void Main(string[] args)
        //{
        //    // Getting user name and repository name from github url
        //    var userNameRepositoryName = RepositoryGetter.parseUrl("https://github.com/martinspetlik/TGH");

        //    // Create new repository getter
        //    getter = new RepositoryGetter(userNameRepositoryName.Item1, userNameRepositoryName.Item2);
        //    // Authetification for 5000 request per hour
        //    getter.Authentication().Wait();

        //    // Check if repository exists, it is not part of contructor, if repository is changed make sure that this method is called
        //    var repositoryExists = getter.CheckRepository().Result;

        //    if (!repositoryExists) return;

        //    // Network checker
        //    //Console.WriteLine("NETWORK " + System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable());

        //    // Testing file extension
        //    fileExt.Add("java");
        //    getter.FileExtensions = fileExt;

        //    Console.WriteLine("Files extensions line number" + getter.FileExtensionsLinesNumber().Result["java"]);

        //    var filesExtensions = getter.ChangedFiles(new DateTime(2016, 4, 19, 20, 22, 12)).Result;

        //    // Loop through filesExtensions
        //    // Dictionary {fileName: List(commit1[number of changes, file content], commit2, ...)}
        //    /*foreach(KeyValuePair<string, List<string[]>> entry in filesExtensions)
        //    {
        //        foreach (string[] commit in entry.Value)
        //        {
        //            // commit is array -> []
        //            Console.WriteLine(string.Join(",", commit));
        //        }
                
        //    }*/

        //}
    }
}
