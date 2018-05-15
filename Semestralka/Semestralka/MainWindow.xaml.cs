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
using NLog;
using NLog.Targets;

namespace Semestralka
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static RepositoryGetter getter;
        private static DateTime searchingDateTime = new DateTime();
        private static Runner runner = null;
        private static readonly List<string> fileExt = new List<string>();
        SettingsHandler settingshandler = null;
        Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            searchingDateTime = DateTime.Now;
            //Sets default properties rows from project settings
            settingshandler = new SettingsHandler(sp_settings);

            #region logger init
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget() { FileName = "log.txt", Name = "logfile" };
            var logconsole = new NLog.Targets.ConsoleTarget() { Name = "logconsole" };

            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Info, logconsole));
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Debug, logfile));

            NLog.LogManager.Configuration = config;
            //Logger logger = NLog.LogManager.GetCurrentClassLogger();
            #endregion

            logger.Info("start");

            //checking net status
            runner = new Runner();
            //GetterInit(settingshandler.getUrlTB());

            //int Desc;
            //if (!InternetGetConnectedState(out Desc, 0))
            //    lb_status.Content = "No connection";
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
                //if(runner != null)
                //{
                //    runner.cts.Cancel();
                //}
                try
                {
                    //searchingDateTime = DateTime.Now;
                    Thread t = new Thread(() => searchingDateTime = getter.CheckingRepository(searchingDateTime));
                    t.Start();
                    //getter.CheckingRepository(searchingDateTime);
                    //runner = new Runner(getter, true);
                }
                catch(Exception ex)
                {
                    ex.ToString();
                }
                
            }
        }
        
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_export_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                //dialog.SelectedPath = settingshandler.getStorageTB();
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result.ToString() == "OK")
                {
                    Save.ExportFilesToExcel((dataGrid.ItemsSource != null) ? dataGrid.ItemsSource.Cast<GitFile>().ToList() : null, dialog.SelectedPath); //dataGrid.SelectedItems.Cast<GitFile>().ToList()
                    System.Windows.MessageBox.Show("Saved");
                }
            }      
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
                    getter.SaveFile(settingshandler.getStorageTB(), GitFile.convertorToDict(dataGrid.SelectedItems.Cast<GitFile>().ToList(), getter.FilesChanges));
                    System.Windows.MessageBox.Show("Saved");
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
            
        }

        private void button_count_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!Connection.CheckConnection())
                {
                    System.Windows.MessageBox.Show("No connection");
                }
                else
                {
                    System.Windows.MessageBox.Show("Count rows from java files: " + getter.FileExtensionsLinesNumber().Result["java"]);
                }
            }
            catch (AggregateException ae)
            {
                ae.ToString();
            }
        }

        private void button_graf_Click(object sender, RoutedEventArgs e)
        {
            Graph.drawFileChanges(dataGrid.ItemsSource.Cast<GitFile>().ToList(),dataGrid.SelectedItems.Cast<GitFile>().ToList());

        }

        private void button_refresh_Click(object sender, RoutedEventArgs e)
        {
            if (!Connection.CheckConnection())
            {
                System.Windows.MessageBox.Show("No connection");
            }
            else
            {
                if(getter == null)
                {
                    logger.Info("refresh_Click start");
                    GetterInit(settingshandler.getUrlTB());
                }
                else
                {
                    //if (runner != null)
                    //{
                    //    runner.cts.Cancel();
                    //}
                    try
                    {
                        logger.Info("refresh_Click start");
                        //runner = new Runner(getter, false);
                        //getter.CheckRepository(getter.CheckingRepository,searchingDateTime);
                        Thread t = new Thread(() => searchingDateTime = getter.CheckingRepository(searchingDateTime));
                        t.Start();
                        //getter.CheckingRepository(searchingDateTime);
                        //set datetime for next search

                        //searchingDateTime = DateTime.Now;                       
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }                            
            }
        }
    }
}
