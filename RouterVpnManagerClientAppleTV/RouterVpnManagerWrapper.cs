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
        public RouterVpnManagerWrapper()
        {
            connection_ = new RouterVpnManagerConnection();
        }

        public void Dispose()
        {
            connection_.Dispose();
        }
    }
}