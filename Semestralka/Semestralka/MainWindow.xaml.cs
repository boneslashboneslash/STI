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
using System.Windows.Forms;
using RepositoryModel;
using System.Threading;
using System.Runtime.InteropServices;

namespace Semestralka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static RepositoryGetter getter;
        private static readonly List<string> fileExt = new List<string>();
        SettingsHandler settingshandler = null;

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        private static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        public MainWindow()
        {
            InitializeComponent();
            //Sets default properties rows from project settings
            settingshandler = new SettingsHandler(sp_settings);

            //TODO:async loop
            int Desc;
            if (InternetGetConnectedState(out Desc, 0))
                ellipse_net_staus.Fill = System.Windows.Media.Brushes.Green;
            else
                ellipse_net_staus.Fill = System.Windows.Media.Brushes.Red;



            getter = RepositoryGetter.CreateNewRepositoryGetter(settingshandler.getUrlTB());
            if(getter == null)
            {
                System.Windows.MessageBox.Show("repository not found");
                settingshandler.setUrlTB("");
            }
            else
            {
                Runner runner = new Runner(getter);     
            }
        }
        
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_export_Click(object sender, RoutedEventArgs e)
        {
            
            Save.ExportFilesToExcel(dataGrid.ItemsSource.Cast<GitFile>().ToList(), settingshandler.getStorageTB()); //dataGrid.SelectedItems.Cast<GitFile>().ToList()
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            getter.SaveFile(settingshandler.getStorageTB(), GitFile.convertorToDict(dataGrid.SelectedItems.Cast<GitFile>().ToList(), getter.FilesChanges));
        }
    }
}
