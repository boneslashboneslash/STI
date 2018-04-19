using RepositoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Semestralka
{
    public class GitFile
    {
        public string name { get; set; }
        public string datetime { get; set; }
        public int countLines { get; set; }


        public GitFile(string name, string datetime, int countLines)
        {
            this.name = name;
            this.datetime = datetime;
            this.countLines = countLines;
        }

        //
        /**
          * convert dict to GitFile obj
          * return List<GitFile>
          */
        public static List<GitFile> convertor(IDictionary<string, List<string[]>> dict)
        {
            List<GitFile> result = new List<GitFile>();

            foreach (var dictItem in dict)
            {
                foreach(var item in dictItem.Value)
                {
                    GitFile gf = new GitFile(dictItem.Key, item[2], Int32.Parse(item[0]));
                    result.Add(gf);
                }         
            }
            return result;
        }
        
    }
}
