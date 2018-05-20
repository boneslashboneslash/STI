using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Semestralka
{
    public class Connection
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool CheckConnection()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        public static void CheckingConnection()
        {
            Thread t = new Thread(() => OnTick());
            t.Start();
        }

        private static void OnTick()
        {
            // Repeat this loop until cancelled.
            while (true)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    
                    MainWindow win = (MainWindow)Application.Current.MainWindow;
                    win.lb_status_connect.Content = (Connection.CheckConnection()) ? "Online" : "Offline";
                    var status = win.lb_status.Content;
                
                    win.lb_status_connect.Foreground = win.lb_status_connect.Content.Equals("Online") ? Brushes.Green : Brushes.Red;
                    if (win.lb_status_connect.Content.ToString() == "Offline" && status.ToString() == "Searching...")
                    {
                        win.lb_status.Content = "Failed";
                        win.lb_status.Foreground = Brushes.Red;
                    }
                }));
                // Wait to repeat again.
                Thread.Sleep(20);
            }
        }

    }
}
