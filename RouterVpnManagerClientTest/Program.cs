using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouterVpnManagerClientLibrary;

namespace RouterVpnManagerClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (RouterVpnManagerConnection connection = new RouterVpnManagerConnection())
            {
                connection.Connect();
                ControlledRequests requests = new ControlledRequests(connection);
                requests.ListAvaliableVpns();
                Console.ReadLine();
            }
            
        }
    }
}
