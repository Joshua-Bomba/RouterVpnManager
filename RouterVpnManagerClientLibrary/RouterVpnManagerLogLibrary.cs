using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterVpnManagerClientLibrary
{
    public static class RouterVpnManagerLogLibrary
    {
        public static object lock_ = new object();
        public static void Log(string message)
        {
            lock (lock_)
            {
                RawLog(message);
            }

        }

        private static void RawLog(string message)
        {
            Console.WriteLine(message);
        }

        public static void LogCollection(IEnumerable<string> col)
        {
            lock (lock_)
            {
                foreach (var c in col)
                {
                    RawLog(c);
                }
            }
        }
    }
}
