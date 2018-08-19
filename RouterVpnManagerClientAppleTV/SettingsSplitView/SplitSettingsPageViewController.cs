using Foundation;
using System;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public partial class SplitSettingsPageViewController : UIViewController
    {

        private SettingsModel currentSetting_;
        public SplitSettingsPageViewController (IntPtr handle) : base (handle)
        {
        }

        public SettingsModel CurrentSetting
        {
            get => currentSetting_;
            set
            {
                currentSetting_ = value;
                UpdateUI();
            }
        }

        public void UpdateUI()
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