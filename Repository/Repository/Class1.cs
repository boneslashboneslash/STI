using System;
using Octokit;

namespace Repository

{
    public class Class1
    {
        /*public Class1()
        {
            //GetRepository().Wait();

        }*/

        /**
        public async Task GetRepository()
        {
            var github = new GitHubClient(new ProductHeaderValue("MyAmazingApp"));
            var user = await github.User.Get("half-ogre");
            Console.WriteLine(user.Followers + " folks love the half ogre!");
        }*/
        
        static void Main() 
        {
            Console.WriteLine("Hello World!");

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
    
    
}