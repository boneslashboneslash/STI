using LiveCharts;
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
                p = new CartesianChart();
                var window = new Window();
                window.Name = "Graf";
                p.AxisX.Clear();
                p.AxisY.Clear();
                List<String> datumList = new List<String>();
                List<int> pocetList = new List<int>();

                string name = filenamematch[0].name;
                MessageBox.Show(name);
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
                new ColumnSeries
                {
                    Title = "Počet řádků v souboru: ",
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
