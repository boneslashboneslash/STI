using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections;

namespace GraphTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Pro zprovoznění třeba vpravo v projectu References, pravým tlačítkem
        // myši přidat Nuget Package LiveChart WPF
        // Poté přidat do XAML souboru řádek
        // xmlns:lvc="clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf"
        // Následně stačí v kódu: usin LiveCharts, LiveCharts.Wpf
        // problém je zatím, že se to vytváří dynamicky, staticky by to bylo možná lepší,
        // ale takhle si to lze také upravit dle libosti
        // problém je trochu formát dat, příliš dlouhá data, málo se jich zobrazí


        CartesianChart s = new CartesianChart();
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        // potřebný List pro vykreslení hodnot na osu X
        List<String> iList = new List<String>();


        
        public MainWindow()
        {



            InitializeComponent();
            button_Copy.Visibility = Visibility.Hidden;
            //přidání věcí do listu pro osu x
            iList.Add("26.12.2018, 11:22");
            iList.Add("27.12.2018, 10:55");
            iList.Add("27.12.2018, 12:22");
            iList.Add("27.12.2018, 12:55");
            iList.Add("28.12.2018, 22:22");
            iList.Add("28.12.2018, 23:55");
            //nastavení šířky a výšky grafu
            s.Width = 200;
            s.Height = 200;
            //přidání grafu do gridu
            myGrid.Children.Add(s);
            s.Visibility = Visibility.Hidden;


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            s.Visibility = Visibility.Visible;
            button_Copy.Visibility = Visibility.Visible;
            // vymazání popisků osy X (jinak se hromadí)
            s.AxisX.Clear();
            s.AxisY.Clear();

            // dle návodu https://lvcharts.net/App/examples/v1/wpf/Basic%20Column
            s.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Počet řádků v souboru: ",
                    // můžou se nasoukat data z Dictionary -> viz druhá zkouška
                    Values = new ChartValues<double> {100, 200, 250, 300, 400, 700},
                },
            };
            // Label na osu Y, hodnoty se automaticky přidají
            s.AxisY.Add(new Axis
            {
                Title = "Počet řádků",
                
            });
            // Label na osu X, je třeba přidat hodnoty z Listu ručně
            s.AxisX.Add(new Axis
            {
                Title = "Datum změny",
                Labels = iList
            });
        
    



            
            


            

        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {

            s.Visibility = Visibility.Hidden;
            button_Copy.Visibility = Visibility.Hidden;

        }
    }
}
