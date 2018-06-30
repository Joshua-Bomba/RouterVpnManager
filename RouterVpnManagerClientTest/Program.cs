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
                try
                {
                    connection.Connect();
                    requests = new ControlledRequests(connection);
                    requests.AddBroadcastListener(new Broadcasts());
                    ListenForCommands();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        static void ListenForCommands()
        {
            string input = "";
            while (input != "exit")
            {
                Console.Write("Please Enter a Command. type help to get a list of commands: ");
                input = Console.ReadLine()?.ToLower();
                if (input != null)
                {
                    try
                    {
                        switch (input.Split(' ').First())
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
                    catch {Console.WriteLine("Unable to process request");}
                }
            }
        }

        static async void PrintConnectionStatus()
        {
            ConnectionStatusResponse csr = await requests.CheckCurrentConnection();
            Console.WriteLine("Connected?: " +csr.Running);
            if(csr.Running)
                Console.WriteLine("Current Connection: " + csr.ConnectedTo);
        }

        static async void ListAvaliableVpns()
        {
            string[] response = (await requests.ListAvaliableVpns()).ToArray();
            for (int i = 0; i < response.Length; i++)
            {
                Console.WriteLine($"{i}: {response[i]}");
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
                    Console.WriteLine("\nVpn was disconnected: " + response.Reason);
                }
                else
                {
                    Console.WriteLine("\nVpn was unable to disconnect: " + response.Status + " Reason: " + response.Reason);
                }
            }
        }

        static async void ConnectToVpn(string connectionNumber)
        {
            int selection;
            string[] s = connectionNumber.Split(' ');
            if (s.Length > 1 &&int.TryParse(s[1], out selection))
            {
                string[] vpns = (await requests.ListAvaliableVpns()).ToArray();
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
