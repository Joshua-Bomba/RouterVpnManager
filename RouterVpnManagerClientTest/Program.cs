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
                    requests.AddBroadcastListener(new Broadcasts());
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
                            Console.WriteLine("Commands: help, exit, status, avaliablevpns, connect [index], disconnect");
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
            string[] response = requests.ListAvaliableVpns().ToArray();
            for (int i = 0; i < response.Length; i++)
            {
                Console.WriteLine($"{i}: {response}");
            }
        }

        private class Broadcasts : IBroadcastListener
        {
            public void ConnectToVpn(ConnectToVpnResponse response)
            {
                if (string.IsNullOrWhiteSpace(response.Status))
                {
                    Console.WriteLine("Vpn Connected to: " + response.VpnLocation);
                }
                else
                {
                    Console.WriteLine("Connection Attempted failed to " + response.VpnLocation + " Reason:" + response.Status);
                }

            }

            public void DisconnectFromVpn(DisconnectFromVpnResponse response)
            {
                if (string.IsNullOrWhiteSpace(response.Status))
                {
                    Console.WriteLine("Vpn was disconnected: " + response.Reason);
                }
                else
                {
                    Console.WriteLine("Vpn was unable to disconnect: " + response.Status + " Reason: " + response.Reason);
                }
            }
        }

        static void ConnectToVpn(string connectionNumber)
        {
            int selection;
            string[] s = connectionNumber.Split(' ');
            if (s.Length > 1 &&int.TryParse(s[1], out selection))
            {
                string[] vpns = requests.ListAvaliableVpns().ToArray();
                if (selection < vpns.Length)
                {
                    requests.ConnectToVpn(vpns[selection]);
                }
                else
                {
                    Console.WriteLine("Please select a valid option");
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid number");
            }
        }

        static void Disconnect()
        {
            requests.DisconnectFromVpn();
        }

        //string[] vpns = requests.ListAvaliableVpns().ToArray();
        //Console.ReadLine();
        //RouterVpnManagerLogLibrary.LogCollection(vpns);
        //if (vpns.Any())
        //requests.ConnectToVpn(vpns.First());
        //Console.ReadLine();
    }
}
