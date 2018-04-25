using RepositoryModel;
using System;
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
        public Runner(RepositoryGetter getter)
        {
            var dueTime = TimeSpan.FromSeconds(5);
            var interval = TimeSpan.FromSeconds(120);

            MainWindow win = (MainWindow)Application.Current.MainWindow;
            win.dataGrid.ItemsSource = null;
            //Task.Run(() => CheckingRepositoryPeriodicAsync(OnTick, dueTime, interval, cts.Token, getter).Wait());
            CheckingRepositoryPeriodicAsync(OnTick, dueTime, interval, cts.Token, getter);//CancellationToken.None
            

        }

        // The `onTick` method will be called periodically unless cancelled.
        private static async Task CheckingRepositoryPeriodicAsync(Action<RepositoryGetter, DateTime> onTick, TimeSpan dueTime, TimeSpan interval, CancellationToken token, RepositoryGetter getter)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token).ConfigureAwait(false);

            DateTime currentDateTime = DateTime.Now;
            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke(getter, currentDateTime);

                currentDateTime = DateTime.Now;

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token).ConfigureAwait(false);
            }
        }

        private void OnTick(RepositoryGetter getter, DateTime currentDateTime)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => {
                MainWindow win = (MainWindow)Application.Current.MainWindow;
                win.lb_status.Content = "Searching...";
            }));
            // load backup from file
            if (getter.FilesChanges.Count() == 0)
            {
                getter.loadNewFilesChangesFromListString(Save.LoadBackupContentFromFile(getter.UserName + "_" + getter.RepoName + ".txt"));
            }
            
            var filesExtensions = getter.ChangedFiles(new DateTime(currentDateTime.Year, currentDateTime.Month, 
                currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second)).Result;
            //var filesExtensions = getter.ChangedFiles(new DateTime(2016, 4, 19, 20, 22, 12)).Result;
            //currentDateTime = new DateTime(2016, 4, 19, 20, 22, 12);

            Application.Current.Dispatcher.Invoke(new Action(() => {

                MainWindow win = (MainWindow)Application.Current.MainWindow;
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
                        }catch
                        {
                            //nevim proc je row nekdy null
                        }                        
                    }
                }
                //save to file 
                Save.SaveDatagridContent(getter.UserName + "_" + getter.RepoName+ ".txt", getter.FilesChanges);
                win.lb_status.Content = "Finished";
            }));

        }
    }
}
