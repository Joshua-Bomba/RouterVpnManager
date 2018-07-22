﻿using System;
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
                                Console.WriteLine("Commands: help, exit, status, avaliablevpns, connect [index], disconnect, copyconfig, deleteconfig, clearconfigfolder, copyspecificconfig");
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
                            case "copyconfig":
                                CopyCurrentConfig();
                                break;
                            case "deleteconfig":
                                DeleteConfig();
                                break;
                            case "clearconfigfolder":
                                ClearConfigFolder();
                                break;;
                            case "copyspecificconfig":
                                CopySpecificConfig();
                                break;
                            default:
                                break;
                        }
                    }
                    catch {Console.WriteLine("Unable to process request");}
                }
            }
        }

        static void CopySpecificConfig()
        {
            Console.WriteLine("Please enter the name of the config you want to delete");
            string name = Console.ReadLine();
            StatusResponse sr = requests.CopyConfig(name);
            if (sr)
            {
                Console.WriteLine("Config was sucessfully copied");
            }
            else
            {
                Console.WriteLine("Config could not be copied: " + sr.Message);   
            }

        }

        static void ClearConfigFolder()
        {
            StatusResponse sr = requests.ClearConfigFolder();
            if (sr)
            {
                Console.WriteLine("Config was sucessfully cleared");
            }
            else
            {
                Console.WriteLine("Config could not be cleared: " + sr.Message);
            }
        }

        static void DeleteConfig()
        {
            Console.WriteLine("Please enter the name of the config you want to delete");
            string name = Console.ReadLine();
            StatusResponse sr = requests.DeleteConfig(name);
            if (sr)
            {
                Console.WriteLine("Config was Deleted sucessfully");
            }
            else
            {
                Console.WriteLine("Could not delete config: " + sr.Message);
            }
        }

        static void CopyCurrentConfig()
        {
            Console.WriteLine("Please enter a new name for your config");
            string name = Console.ReadLine();
            StatusResponse sr = requests.CopyCurrentConfig(name);
            if (sr)
            {
                Console.WriteLine("current config was copied");
            }
            else
            {
                Console.WriteLine("Config was not able to be copied: " + sr.Message);
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
