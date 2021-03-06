﻿using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Semestralka
{
    class Graph
    {
        public static CartesianChart p;
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public static void drawFileChanges(List<GitFile> listFiles, List<GitFile> filenamematch)
        {
            try
            {
                
                string result = string.Join(", ", listFiles.Select(f => f.datetime));
                listFiles = listFiles.OrderBy(f => f.datetime).ToList();
                p = new CartesianChart();
                var window = new Window();
                List<String> datumList = new List<String>();
                List<int> pocetList = new List<int>();
                string name = filenamematch[0].name;
                string [] k = name.Split('/'); 
                window.Title = "Graf počtu řádků souboru: " + k[k.Length-1];
                window.Name = "Graf";
                foreach (var file in listFiles)
                {
                    if (name.Equals(file.name))
                    {
                        datumList.Add(file.datetime);
                        pocetList.Add(file.countLines);

                    }
                }
                p.Series = new SeriesCollection
            {
                new LineSeries
                {
                 
                    Title = "Počet řádků v souboru "+ k[k.Length-1] + ": ",
                    // můžou se nasoukat data z Dictionary -> viz druhá zkouška
                    Values = new ChartValues<int> (pocetList)
                    //Values = new ChartValues<double> {100, 200, 250, 300, 400, 700},
                },
            };
                p.AxisY.Add(new Axis
                {
                    Title = "Počet řádků",

                });
                // Label na osu X, je třeba přidat hodnoty z Listu ručně
                p.AxisX.Add(new Axis
                {
                    Title = "Datum změny",
                    Labels = datumList
                });

                window.Content = p;
                window.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
