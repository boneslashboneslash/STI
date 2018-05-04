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
using LiveCharts.Wpf;
using System.Net.NetworkInformation;
using System.Net;
using System.Windows.Threading;


namespace Semestralka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private static RepositoryGetter getter;
        private static Runner runner = null;
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


            //getter = RepositoryGetter.CreateNewRepositoryGetter(settingshandler.getUrlTB());
            //if(getter == null)
            //{
            //    System.Windows.MessageBox.Show("repository not found");
            //    settingshandler.setUrlTB("");
            //}
            //else
            //{
            //    Runner runner = new Runner(getter);     
            //}
            GetterInit(settingshandler.getUrlTB());
            int Desc;
            if (!InternetGetConnectedState(out Desc, 0))
                lb_status.Content = "No connection";
        }

       
        public static void GetterInit(string url)
        {
            
            getter = RepositoryGetter.CreateNewRepositoryGetter(url);
            if (getter == null)
            {
                System.Windows.MessageBox.Show("repository not found");
            }
            else
            {
                if(runner != null)
                {
                    runner.cts.Cancel();
                }
                try
                {
                    runner = new Runner(getter, true);
                }
                catch(Exception ex)
                {
                    ex.ToString();
                }
                
            }
        }


public static bool IsInternetAvailable()
{
	System.Net.WebClient workstation = new System.Net.WebClient();
	byte[] data = null;
 
	try
	{
		data = workstation.DownloadData("http://www.google.com");// use any address to check connection.
		if (data != null && data.Length > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	catch
	{
		return false;
	}
 
}


        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_export_Click(object sender, RoutedEventArgs e)
        {

            Save.ExportFilesToExcel((dataGrid.ItemsSource != null)?dataGrid.ItemsSource.Cast<GitFile>().ToList():null, settingshandler.getStorageTB()); //dataGrid.SelectedItems.Cast<GitFile>().ToList()
        }
        
        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = settingshandler.getStorageTB();
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if(result.ToString() == "OK")
                {
                    settingshandler.setStorageTB(dialog.SelectedPath);
                }   
            }



            //// Configure save file dialog box
            //Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            //dlg.FileName = "Document"; // Default file name
            //dlg.DefaultExt = ".text"; // Default file extension
            //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            //// Show save file dialog box
            //Nullable<bool> result = dlg.ShowDialog();

            //// Process save file dialog box results
            //if (result == true)
            //{
            //    // Save document
            //    string filename = dlg.FileName;
            //}
            getter.SaveFile(settingshandler.getStorageTB(), GitFile.convertorToDict(dataGrid.SelectedItems.Cast<GitFile>().ToList(), getter.FilesChanges));
        }

        private void button_count_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.MessageBox.Show("Count rows from java files: " + getter.FileExtensionsLinesNumber().Result["java"]);
            }
            catch (Exception ae)
            { 
                if(ae is AggregateException || ae is KeyNotFoundException || ae is NullReferenceException)
                ae.ToString();
            }
            
            
                
            
        }
        private void button_graf_Click(object sender, RoutedEventArgs e)
        {
            try {
                Graph.drawFileChanges(dataGrid.ItemsSource.Cast<GitFile>().ToList(), dataGrid.SelectedItems.Cast<GitFile>().ToList());
                    }
            catch (ArgumentNullException)
            {
                System.Windows.MessageBox.Show("Not connected to internet");
            }

        }

        private void button_refresh_Click(object sender, RoutedEventArgs e)
        {
            bool isNetworkAvailable = IsInternetAvailable();
            if (isNetworkAvailable)
            { 
                if (runner != null)
                {
                    runner.cts.Cancel();
                }
                try
                {
                    lb_status.Content = "Searching...";
                    runner = new Runner(getter, false);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            else
            { lb_status.Content = "No connection"; }

        }
      
    }
}
