using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class SettingViewController : UITableView
    {
        public SettingViewController (IntPtr handle) : base (handle)
        {
            RegisterClassForCellReuse(typeof(SettingsViewModel),SettingsViewModel.settingsCellId);
            DataSource = new SettingsViewModel();
            Delegate = new SettingsDelegate();
        }

        public new SettingsViewModel Source => DataSource as SettingsViewModel;
    }
}