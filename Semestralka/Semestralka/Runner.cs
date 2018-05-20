using NLog;
using RepositoryModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Semestralka
{
    public class Runner
    {
        public CancellationTokenSource cts = new CancellationTokenSource();
        public static DateTime currentDateTime;
        public static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Runner(RepositoryGetter getter, bool setActualDate)
        {
            logger.Info("new runner");
            var dueTime = TimeSpan.FromSeconds(1);
            var interval = TimeSpan.FromSeconds(3600);

            if (setActualDate == true)
            {
                logger.Info("clear datagrid");
                MainWindow win = (MainWindow)Application.Current.MainWindow;
                win.dataGrid.ItemsSource = null;
            }
            //Task.Run(() => CheckingRepositoryPeriodicAsync(OnTick, dueTime, interval, cts.Token, getter).Wait());
            CheckingRepositoryPeriodicAsync(OnTick, dueTime, interval, cts.Token, getter, setActualDate);//CancellationToken.None
                                                                                                         //CheckingConnectionAsync(OnTick, dueTime, interval, CancellationToken.None);



        }

        // The `onTick` method will be called periodically unless cancelled.
        private static async Task CheckingRepositoryPeriodicAsync(Action<RepositoryGetter, DateTime> onTick, TimeSpan dueTime, TimeSpan interval, CancellationToken token, RepositoryGetter getter, bool setActualDate)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token).ConfigureAwait(false);

            if (currentDateTime.ToString("dd.MM.yyyy HH:mm:ss").Equals("01.01.0001 00:00:00") || setActualDate == true)
            {
                logger.Info("set actual date time");
                currentDateTime = DateTime.Now;
            }



            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                //onTick?.Invoke(getter, currentDateTime);

                Task task = Task.Factory.StartNew(() => onTick(getter, currentDateTime));
                //task.Wait(TimeSpan.FromMinutes(1));
                //if (!task.IsCompleted)
                //{
                //    Application.Current.Dispatcher.Invoke(new Action(() =>
                //    {
                //        MainWindow win = (MainWindow)Application.Current.MainWindow;
                //        win.lb_status.Content = "failed";
                //        win.lb_status_connect.Content = (Connection.CheckConnection()) ? "Online" : "Offline";
                //    }));
                //}

                currentDateTime = DateTime.Now;

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token).ConfigureAwait(false);
            }


        }

        private void OnTick(RepositoryGetter getter, DateTime currentDateTime)
        {


            //Task.Run(() => CheckingRepositoryPeriodicAsync(OnTick, dueTime, interval, cts.Token, getter).Wait());
            //CancellationToken.None
            //CheckingConnectionAsync(OnTick, dueTime, interval, CancellationToken.None);



            var k = "";
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow win = (MainWindow)Application.Current.MainWindow;
                win.lb_status_connect.Content = (Connection.CheckConnection()) ? "Online" : "Offline";
                win.lb_status_connect.Foreground = win.lb_status_connect.Content.Equals("Online") ? Brushes.Green : Brushes.Red;
                if (win.lb_status_connect.Content.ToString() == "Offline")
                {
                    //Task.Run(() => CheckingRepositoryPeriodicAsync(OnTick, dueTime, interval, cts.Token, getter).Wait());
                    k = "Offline";
                    win.lb_status.Content = "Failed";                                                                                              //CheckingConnectionAsync(OnTick, dueTime, interval, CancellationToken.None);
                    win.lb_status.Foreground = Brushes.Red;
                }
                else
                {
                    k = "Online";

                    win.lb_status.Content = "Searching...";
                }
            }));

            // load backup from file
            if (k == "Online")
            {
                if (getter.FilesChanges.Count() == 0)
                {
                    logger.Info("load backup from file");
                    getter.loadNewFilesChangesFromListString(Save.LoadBackupContentFromFile(getter.UserName + "_" + getter.RepoName + ".txt"));
                }
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MainWindow win = (MainWindow)Application.Current.MainWindow;
                    win.lb_status_connect.Content = (Connection.CheckConnection()) ? "Online" : "Offline";
                    win.lb_status_connect.Foreground = win.lb_status_connect.Content.Equals("Online") ? Brushes.Green : Brushes.Red;
                    if (win.lb_status_connect.Content.ToString() == "Offline")
                    {
                        k = "Offline";
                        win.lb_status.Content = "Failed";
                        win.lb_status.Foreground = Brushes.Red;
                    }

                    else
                    {
                        k = "Online";

                        win.lb_status.Content = "Searching...";
                    }
                }));
                if (k == "Online")
                {
                    logger.Info("searching files");
                    IDictionary<string, List<string[]>> filesExtensions = new Dictionary<string, List<string[]>>();
                    try
                    {
                        filesExtensions = getter.ChangedFiles(new DateTime(currentDateTime.Year, currentDateTime.Month,
                        currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second)).Result;
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MainWindow win = (MainWindow)Application.Current.MainWindow;
                            win.lb_status.Content = "Failed";
                            win.lb_status_connect.Content = (Connection.CheckConnection()) ? "Online" : "Offline";
                        }));
                    }


                    //var filesExtensions = getter.ChangedFiles(new DateTime(2016, 4, 19, 20, 22, 12)).Result;
                    //currentDateTime = new DateTime(2016, 4, 19, 20, 22, 12);

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {

                        MainWindow win = (MainWindow)Application.Current.MainWindow;
                        if (win.lb_status.Content != "Failed")
                        {
                            //win.dataGrid.ItemsSource = GitFile.convertor(filesExtensions);
                            win.dataGrid.ItemsSource = GitFile.convertorToList(filesExtensions).OrderByDescending(x => x.datetime);

                            foreach (GitFile item in win.dataGrid.ItemsSource)
                            {
                                var row = win.dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                                if (row == null)
                                {
                                    win.dataGrid.UpdateLayout();
                                    row = win.dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                                }
                                DateTime myDate = DateTime.ParseExact(item.datetime, "dd.MM.yyyy HH:mm:ss",
                                    System.Globalization.CultureInfo.InvariantCulture);
                                if (myDate >= currentDateTime)
                                {
                                    try
                                    {
                                        row.Background = Brushes.GreenYellow;
                                    }
                                    catch
                                    {
                                        //nevim proc je row nekdy null
                                    }
                                }
                            }

                            //save to file 
                            logger.Info("save changes to file ");
                            Save.SaveDatagridContent(getter.UserName + "_" + getter.RepoName + ".txt", getter.FilesChanges);
                            win.lb_status.Content = "Finished";
                            win.lb_status.Foreground = Brushes.Green;
                        }
                    }));
                }



            }
        }


    }
}
