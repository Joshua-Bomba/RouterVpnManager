using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;
using RouterVpnManagerClientLibrary;

namespace RouterVpnManagerClient
{
    public class RouterVpnManagerWrapper : IDisposable
    {
        private RouterVpnManagerConnection connection_;
        private MainPageViewController mainPageController_;
        public RouterVpnManagerWrapper(MainPageViewController mainPageController)
        {
            try
            {
                mainPageController_ = mainPageController;
                connection_ = new RouterVpnManagerConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private void GetSettings()
        {

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
                    mainPageController_.LblStatus.Text = "Connected";
                    mainPageController_.LblStatus.TextColor = UIColor.Green;
                    return true;
                }
                else
                {
                    Global.BasicNotificationAlert("Unable To Connect", "You are already connected to the server",
                        mainPageController_);
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
    }
}