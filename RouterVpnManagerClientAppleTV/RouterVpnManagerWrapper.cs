using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;
using RouterVpnManagerClientLibrary;
using RouterVpnManagerClientLibrary.ServerResponseObjects;

namespace RouterVpnManagerClient
{
    public class RouterVpnManagerWrapper : IDisposable, IBroadcastListener
    {
        private RouterVpnManagerConnection connection_;
        private ControlledRequests request_;
        private MainPageViewController mainPageController_;
        public RouterVpnManagerWrapper(MainPageViewController mainPageController)
        {
            try
            {
                mainPageController_ = mainPageController;
                connection_ = new RouterVpnManagerConnection();
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
            //TODO: fix that dam settings menu
        }

        public bool Connect()
        {
            try
            {
                if (!connection_.IsConnected)
                {
                    mainPageController_.LblStatus.Text = "Connecting, Please Wait...";
                    mainPageController_.LblStatus.TextColor = UIColor.Black;
                    GetSettings();
                    connection_.Connect();
                    mainPageController_.BtnSelectAVpn.Enabled = true;
                    mainPageController_.LblStatus.Text = "Connected";
                    mainPageController_.LblStatus.TextColor = UIColor.Green;
                    mainPageController_.BtnConnect.SetTitle("Disconnect From Server", UIControlState.Normal);
                    return true;
                }
                else
                {
                    mainPageController_.LblStatus.Text = "Disconnecting, Please Wait...";
                    mainPageController_.LblStatus.TextColor = UIColor.Black;
                    request_.DisconnectFromVpn();
                    mainPageController_.BtnSelectAVpn.Enabled = false;
                    mainPageController_.LblStatus.Text = "Not Connected";
                    mainPageController_.LblStatus.TextColor = UIColor.Red;
                    mainPageController_.BtnConnect.SetTitle("Connect To Server", UIControlState.Normal);

                    return false;
                }

            }
            catch (Exception ex)
            {
                mainPageController_.LblStatus.Text = "Not Connected";
                mainPageController_.LblStatus.TextColor = UIColor.Red;
                Global.BasicNotificationAlert("Unable To Connect", ex.ToString(),mainPageController_);
                return false;
            }

        }

        public void Dispose()
        {
            connection_.Dispose();
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
                Global.BasicNotificationAlert("\nVpn was disconnected: ", response.Reason, mainPageController_);
                                mainPageController_.LblStatus.Text = "Not Connected";
                mainPageController_.LblStatus.TextColor = UIColor.Red;
            }
            else
            {
                Global.BasicNotificationAlert("\nVpn was unable to disconnect: ",
                    response.Status + " Reason: " + response.Reason, mainPageController_);
            }
        }
    }
}