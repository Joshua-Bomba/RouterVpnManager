using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using CoreGraphics;
using Foundation;
using UIKit;
using RouterVpnManagerClientLibrary;
using RouterVpnManagerClientLibrary.ServerResponseObjects;

namespace RouterVpnManagerClient
{
    public class RouterVpnManagerWrapper : IDisposable, IBroadcastListener
    {
        private static RouterVpnManagerWrapper vpnManager_;
        private bool connected_;

        public static RouterVpnManagerWrapper Instance
        {
            get
            {
                if (vpnManager_ == null)
                   vpnManager_ = new RouterVpnManagerWrapper();

                return vpnManager_;
            }
        }



        private RouterVpnManagerConnection connection_;
        private ControlledRequests request_;


        public static MainPageViewController MainPageController { get; set; }

        public VpnSelectorCollectionViewDataSource VpnSelectorDataSource { get; set; }

        private RouterVpnManagerWrapper()
        {
            try
            {
                connection_ = new RouterVpnManagerConnection();
                GetSettings();
                connected_ = false;
                request_ = new ControlledRequests(connection_);
                request_.AddBroadcastListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Global.BasicNotificationAlert("Something Broke", "We were unable to process your request", MainPageController, e.ToString());
            }

        }

        public static void DeleteInstance()
        {
            vpnManager_ = null;
        }

        private void GetSettings()
        {
            connection_.Host = "192.168.2.36";
            connection_.Port = 8000;
            connection_.CallbackTimeout = -1;
            //TODO: fix that dam settings menu
        }

        public bool Connect()
        {
            if (MainPageController != null)
            {
                try
                {
                    if (!connection_.IsConnected)
                    {
                        MainPageController.LblStatus.Text = "Connecting, Please Wait...";
                        MainPageController.LblStatus.TextColor = UIColor.Black;
                        connection_.Connect();
                        if (VpnSelectorDataSource != null)
                        {
                            VpnSelectorDataSource.PopulateVpns();
                        }

                        MainPageController.BtnSelectAVpn.Enabled = true;
                        MainPageController.LblStatus.Text = "Connected";
                        MainPageController.LblStatus.TextColor = UIColor.Green;
                        MainPageController.BtnConnect.SetTitle("DisconnectFromServer From Server", UIControlState.Normal);
                        connected_ = true;
                        return true;
                    }
                    else
                    {
                        MainPageController.LblStatus.Text = "Disconnecting, Please Wait...";
                        MainPageController.LblStatus.TextColor = UIColor.Black;
                        MainPageController.BtnSelectAVpn.Enabled = false;
                        MainPageController.LblStatus.Text = "Not Connected";
                        MainPageController.LblStatus.TextColor = UIColor.Red;
                        MainPageController.BtnConnect.SetTitle("Connect To Server", UIControlState.Normal);
                        connected_ = false;
                        connection_.Dispose();
                        DeleteInstance();
                        return false;
                    }

                }
                catch (SocketException ex)
                {
                    MainPageController.LblStatus.Text = "Not Connected";
                    MainPageController.LblStatus.TextColor = UIColor.Red;
                    Global.BasicNotificationAlert("Unable To Connect", "We were unable to find the server", MainPageController, ex.ToString());
                    connected_ = false;
                    connection_.Dispose();
                    DeleteInstance();
                    return false;
                }
                catch (Exception ex)
                {
                    MainPageController.LblStatus.Text = "Not Connected";
                    MainPageController.LblStatus.TextColor = UIColor.Red;
                    Global.BasicNotificationAlert("Unable To Connect", "We were unable to process your request", MainPageController,ex.ToString());
                    connected_ = false;
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Can't find main controller");
                return false;
            }
        }

        public void Dispose()
        {
            connection_?.Dispose();
        }

        public List<VpnSelectorModel> GetVpns()
        {
            List<VpnSelectorModel> vpns;
            try
            {
                
                if (connected_)
                {
                    string[] list = request_.ListAvaliableVpns().ToArray();
                    vpns = new List<VpnSelectorModel>(list.Length);
                    for (int i = 0; i < list.Length; i++)
                    {
                        vpns.Add(new VpnSelectorModel { Selectable = true, Title = list[i],ConnectionNumber = i});
                    }

                }
                else
                {
                    vpns = new List<VpnSelectorModel>();
                }
            }
            catch (Exception ex)
            {
                vpns = new List<VpnSelectorModel>();
                Global.BasicNotificationAlert("\nVpn was unable to get Vpns: ","We were unable to process your request", MainPageController, ex.ToString());
            }
            return vpns;
        }

        /// <summary>
        /// Broadcast ExceptionMessage For connect to vpn being compleated
        /// </summary>
        /// <param name="response"></param>
        public void ConnectedToVpn(ConnectToVpnResponse response)
        {
            //try
            //{
            //    if (string.IsNullOrWhiteSpace(response.Status))
            //    {
            //        Console.WriteLine("Vpn Connected to: " + response.VpnLocation);
            //        Global.BasicNotificationAlert("Connected", "Vpn Connected to: " + response.VpnLocation,
            //            MainPageController);
            //    }
            //    else
            //    {
            //        Console.WriteLine("Connection Attempted failed to " + response.VpnLocation + " Reason:" +
            //                          response.Status);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Global.BasicNotificationAlert("Something Broke", "We were unable to process a request", MainPageController, ex.ToString());
            //}


        }

        /// <summary>
        /// Broadcast ExceptionMessage For vpn being disconnected
        /// </summary>
        /// <param name="response"></param>
        public void DisconnectedFromVpn(DisconnectFromVpnResponse response)
        {
            //try
            //{
            //    if (string.IsNullOrWhiteSpace(response.Status))
            //    {
            //        Global.BasicNotificationAlert("\nVpn was disconnected: ", response.Reason, MainPageController);
            //        MainPageController.LblStatus.Text = "Not Connected";
            //        MainPageController.LblStatus.TextColor = UIColor.Red;
            //        connected_ = false;
            //    }
            //    else
            //    {
            //        Global.BasicNotificationAlert("\nVpn was unable to disconnect: ",
            //            response.Status + " Reason: " + response.Reason, MainPageController);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Global.BasicNotificationAlert("Something Broke", "We were unable to process a request", MainPageController, ex.ToString());
            //}
        }

        public void ConnectToVpn(int selection)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }//TODO: add a wait for a broadcast response

        }

        public void DisconnectFromVpn()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }//TODO: add a wait for a broadcast response
        }

        //public bool IsConnectedToVpn()
        //{

        //}
    }
}