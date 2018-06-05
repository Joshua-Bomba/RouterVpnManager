using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RouterVpnManagerClientLibrary;
using RouterVpnManagerClientLibrary.ServerResponseObjects;

namespace RouterVpnManagerClientTest
{
    class Program
    {
        private static RouterVpnManagerConnection connection;
        private static ControlledRequests requests;


        static void Main(string[] args)
        {
            using (connection = new RouterVpnManagerConnection())
            {
                if (connection.Connect())
                {
                    requests = new ControlledRequests(connection);
                    ListenForCommands();
                }
            }
            
        }

        static void ListenForCommands()
        {
            string input = "";
            while (input != "exit")
            {
                Console.WriteLine("Please Enter a Command. type help to get a list of commands");
                input = Console.ReadLine()?.ToLower();
                if (input != null)
                {
                    switch (input)
                    {
                        case "help":
                            Console.WriteLine("Commands: help, ");
                            break;
                        case "exit":
                            Console.WriteLine("Exiting");
                            break;
                        case "status":
                            PrintConnectionStatus();
                            break;
                        case "avaliablevpns":
                            ListAvaliableVpns();
                            break;
                        case "connect":
                            ConnectToVpn(input);
                            break;
                        case "disconnect":
                            Disconnect();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        static void PrintConnectionStatus()
        {
            ConnectionStatusResponse csr = requests.CheckCurrentConnection();
            Console.WriteLine("Connected?: " +csr.Running);
            if(csr.Running)
                Console.WriteLine("Current Connection: " + csr.ConnectedTo);
        }

        static void ListAvaliableVpns()
        {

        }

        static void ConnectToVpn(string connectionNumber)
        {

        }

        static void Disconnect()
        {

        }

        //string[] vpns = requests.ListAvaliableVpns().ToArray();
        //Console.ReadLine();
        //RouterVpnManagerLogLibrary.LogCollection(vpns);
        //if (vpns.Any())
        //requests.ConnectToVpn(vpns.First());
        //Console.ReadLine();
    }
}
