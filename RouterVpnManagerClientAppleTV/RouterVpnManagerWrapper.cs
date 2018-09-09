using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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


        public MainPageViewController MainPageController { get; set; }

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
            }

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
                        MainPageController.BtnConnect.SetTitle("Disconnect From Server", UIControlState.Normal);
                        connected_ = true;
                        return true;
                    }
                    else
                    {
                        MainPageController.LblStatus.Text = "Disconnecting, Please Wait...";
                        MainPageController.LblStatus.TextColor = UIColor.Black;
                        request_.DisconnectFromVpn();
                        MainPageController.BtnSelectAVpn.Enabled = false;
                        MainPageController.LblStatus.Text = "Not Connected";
                        MainPageController.LblStatus.TextColor = UIColor.Red;
                        MainPageController.BtnConnect.SetTitle("Connect To Server", UIControlState.Normal);
                        connected_ = false;
                        return false;
                    }

                }
                catch (Exception ex)
                {
                    MainPageController.LblStatus.Text = "Not Connected";
                    MainPageController.LblStatus.TextColor = UIColor.Red;
                    Global.BasicNotificationAlert("Unable To Connect", ex.ToString(), MainPageController);
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
            connection_.Dispose();
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
                    foreach (string s in list)
                    {
                        vpns.Add(new VpnSelectorModel {Selectable = true, Title = s});
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
                Global.BasicNotificationAlert("\nVpn was unable to get Vpns: ",ex.ToString(), MainPageController);
            }
            return vpns;
        }

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
                Global.BasicNotificationAlert("\nVpn was disconnected: ", response.Reason, MainPageController);
                MainPageController.LblStatus.Text = "Not Connected";
                MainPageController.LblStatus.TextColor = UIColor.Red;
                connected_ = false;
            }
            else
            {
                Global.BasicNotificationAlert("\nVpn was unable to disconnect: ",
                    response.Status + " Reason: " + response.Reason, MainPageController);
            }
        }
    }
}