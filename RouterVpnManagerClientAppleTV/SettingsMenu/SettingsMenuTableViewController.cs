using Foundation;
using System;
using UIKit;
// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public partial class SettingsMenuTableViewController : UITableViewController
    {
        public SettingsMenuTableViewController (IntPtr handle) : base (handle)
        {
        }

        public SettingsMenuTableViewSource Source => TableView.DataSource as SettingsMenuTableViewSource;


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new SettingsMenuTableViewSource(this);
            TableView.ReloadData();

            TableView.RegisterClassForCellReuse(typeof(SettingsMenuTableViewCell), SettingsMenuTableViewSource.settingsCellId);
        }
    }
}

