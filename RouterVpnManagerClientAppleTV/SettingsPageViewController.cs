using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class SettingsPageViewController : UITableViewController
    {

        public SettingsViewModel DataSource => TableView.DataSource as SettingsViewModel;

        public SettingsDelegate TableDelegate => TableView.Delegate as SettingsDelegate;

        public SettingsPageViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RegisterClassForCellReuse(typeof(SettingsViewCell), SettingsViewModel.settingsCellId);
            TableView.DataSource = new SettingsViewModel();
            TableView.Delegate = new SettingsDelegate();
            TableView.ReloadData();
        }
    }
}