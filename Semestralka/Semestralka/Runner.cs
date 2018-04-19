using RepositoryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Semestralka
{
    public class Runner
    {
        public Runner(RepositoryGetter getter)
        {
            var dueTime = TimeSpan.FromSeconds(5);
            var interval = TimeSpan.FromSeconds(120);
            // TODO: Add a CancellationTokenSource and supply the token here instead of None.
            CheckingRepositoryPeriodicAsync(OnTick, dueTime, interval, CancellationToken.None, getter);
        }

        // The `onTick` method will be called periodically unless cancelled.
        private static async Task CheckingRepositoryPeriodicAsync(Action<RepositoryGetter> onTick, TimeSpan dueTime, TimeSpan interval, CancellationToken token, RepositoryGetter getter)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token).ConfigureAwait(false);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke(getter);
                //MainWindow wnd = (MainWindow)Application.Current.MainWindow;
                //wnd.datagridshw();
                
                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token).ConfigureAwait(false);
            }
        }

        private void OnTick(RepositoryGetter getter)
        {
            // TODO: Your code here
            var filesExtensions = getter.ChangedFiles(new DateTime(2016, 4, 19, 20, 22, 12)).Result;

            Application.Current.Dispatcher.Invoke(new Action(() => {
                MainWindow win = (MainWindow)Application.Current.MainWindow;
                win.dataGrid.ItemsSource = GitFile.convertor(filesExtensions);
            }));
        }

        
    }
}
