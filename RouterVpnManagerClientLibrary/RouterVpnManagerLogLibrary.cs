//#define LOG_BROADCAST

using System;
using System.Collections.Generic;

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

        public static void LogBroadcastMessage(string message)
        {
#if LOG_BROADCAST
            Log(message);
#endif
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
