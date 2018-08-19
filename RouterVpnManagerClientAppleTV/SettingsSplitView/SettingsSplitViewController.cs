using Foundation;
using System;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public partial class SettingsSplitViewController : UISplitViewController
    {
        public SettingsSplitViewController (IntPtr handle) : base (handle)
        {
        }

        public SettingsMenuTableViewController Menu=> ViewControllers[0] as SettingsMenuTableViewController;

        public SplitSettingsPageViewController ContentArea=>ViewControllers[1] as SplitSettingsPageViewController;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Sets the current option to the current model
        }
    }
}