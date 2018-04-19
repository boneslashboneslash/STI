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

namespace Semestralka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static RepositoryGetter getter;
        private static readonly List<string> fileExt = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            //Sets default properties rows from project settings
            SettingsHandler settingshandler = new SettingsHandler(sp_settings);

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

        public void datagridshw()
        {
            dataGrid.ItemsSource = getter.FilesChanges;
        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_export_Click(object sender, RoutedEventArgs e)
        {
            //Save.ExportFilesToExcel(listSouboru, "C:/TUL/STI/pokus/test.xlsx");





        }
    }
}
