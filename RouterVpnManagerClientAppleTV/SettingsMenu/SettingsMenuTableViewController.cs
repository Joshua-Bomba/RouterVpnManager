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

        public SettingsMenuTableViewDataSource DataSource => TableView.DataSource as SettingsMenuTableViewDataSource;


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new SettingsMenuTableViewDataSource(this);
            TableView.ReloadData();

            TableView.RegisterClassForCellReuse(typeof(SettingsMenuTableViewCell), SettingsMenuTableViewDataSource.settingsCellId);
        }
    }
}

