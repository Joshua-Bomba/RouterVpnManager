using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class SettingsSplitViewController : UISplitViewController
    {
        public SettingsSplitViewController (IntPtr handle) : base (handle)
        {
        }

        public SettingsPageViewController Menu=> ViewControllers[0] as SettingsPageViewController;

        public SplitSettingsPageViewController ContentArea=>ViewControllers[1] as SplitSettingsPageViewController;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
    }
}