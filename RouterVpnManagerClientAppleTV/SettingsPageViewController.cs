using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class SettingsPageViewController : UITableViewController
    {

        public SettingsViewModel DataSource
        {
            get { return TableView.DataSource as SettingsViewModel;}
        }

        public SettingsDelegate TableDelegate
        {
            get { return  TableView.Delegate as SettingsDelegate;}
        }

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