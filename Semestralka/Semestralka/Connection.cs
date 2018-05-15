using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
    }
}
