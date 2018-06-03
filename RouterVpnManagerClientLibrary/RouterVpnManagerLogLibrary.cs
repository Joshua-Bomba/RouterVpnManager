﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouterVpnManagerClientLibrary
{
    public static class RouterVpnManagerLogLibrary
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void LogCollection(IEnumerable<string> col)
        {
            foreach (var c in col)
            {
                Log(c);
            }
        }
    }
}
