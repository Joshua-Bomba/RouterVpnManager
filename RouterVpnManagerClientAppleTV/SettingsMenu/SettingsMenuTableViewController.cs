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

        public new SettingMenuTableView TableView => base.TableView.Delegate as SettingMenuTableView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            //TableView.Controller = this;
            //TableView.DataSource = new SettingsMenuTableViewDataSource(this);
            //TableView.Delegate = new SettingsMenuTableViewDelegate(this);
            
            //TableView.RegisterClassForCellReuse(typeof(SettingsMenuTableViewCell), SettingMenuTableView.settingsCellId);
        }
    }
}

