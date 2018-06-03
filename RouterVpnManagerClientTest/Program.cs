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
                if (connection.Connect())
                {
                    ControlledRequests requests = new ControlledRequests(connection);
                    requests.AddBroadcastListener();
                    string[] vpns = requests.ListAvaliableVpns().ToArray();
                    Console.ReadLine();
                    RouterVpnManagerLogLibrary.LogCollection(vpns);
                    if (vpns.Any())
                        requests.ConnectToVpn(vpns.First());
                    Console.ReadLine();
                }
            }
            
        }
    }
}
