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

        public void Connect()
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
                }
                else
                {
                    Global.BasicNotificationAlert("Unable To Connect", "You are already connected to the server",
                        mainPageController_);
                }

            }
            catch (Exception ex)
            {
                mainPageController_.LblStatus.Text = "Not Connected";
                mainPageController_.LblStatus.TextColor = UIColor.Red;
                Global.BasicNotificationAlert("Unable To Connect", ex.ToString(),mainPageController_);
            }

        }

        public void Dispose()
        {
            connection_.Dispose();
        }
    }
}