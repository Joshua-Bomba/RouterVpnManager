using Foundation;
using System;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public partial class SplitSettingsPageViewController : UIViewController
    {
        public SplitSettingsPageViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            IPSettingsView.Hidden = true;
            IPSettingsView.UserInteractionEnabled = false;
        }
    }
}