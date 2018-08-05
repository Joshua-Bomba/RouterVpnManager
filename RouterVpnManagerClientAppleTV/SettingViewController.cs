using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class SettingViewController : UITableView
    {
        public SettingViewController (IntPtr handle) : base (handle)
        {

        }

        public new SettingsViewModel Source => DataSource as SettingsViewModel;
    }
}