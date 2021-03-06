﻿using RepositoryModel;
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
        public static List<GitFile> convertorToList(IDictionary<string, List<string[]>> dict)
        {
            List<GitFile> result = new List<GitFile>();

            foreach (var dictItem in dict)
            {
                foreach (var item in dictItem.Value)
                {
                    GitFile gf = new GitFile(dictItem.Key, item[2], Int32.Parse(item[0])); 
                    result.Add(gf);
                }
            }
            return result;
        }

        public static IDictionary<string, List<string[]>> convertorToDict(List<GitFile> list, IDictionary<string, List<string[]>> dictAll)
        {
            IDictionary<string, List<string[]>> result = new Dictionary<string, List<string[]>>();

            foreach (var listItem in list)
            {
                var dictItem = dictAll.Where(x => x.Key == listItem.name).First();
                foreach(var item in dictItem.Value)
                {
                    if(item[2] == listItem.datetime)
                    {
                        List<string[]> l = new List<string[]>(1);
                        l.Add(item);
                        KeyValuePair<string, List<string[]>> kvp = new KeyValuePair<string, List<string[]>>(listItem.name, l);
                        result.Add(kvp);
                    }                  
                }
                //result.Add(dictItem);
            }
            return result;
        }

        //public static List<GitFile> convertFromListString(List<string> listContent)
        //{
        //    List<GitFile> result = new List<GitFile>();
        //    foreach (var line in listContent)
        //    {
        //        string[] parts = line.Split('|');
        //        result.Add(new GitFile(parts[0], parts[1], Int32.Parse(parts[2])));
        //    }
        //    return result;
        //}
    }
}
