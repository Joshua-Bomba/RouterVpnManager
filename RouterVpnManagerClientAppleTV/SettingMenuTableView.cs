using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class SettingMenuTableView : UITableViewSource
    {
        public SettingMenuTableView (IntPtr handle) : base (handle)
        {
            this.Settings = new List<SettingsModel>();
            PopulateSettings();


        }
        public const string settingsCellId = "SettingsViewModel";


        public SettingsMenuTableViewController Controller { get; set; }

        public List<SettingsModel> Settings { get; set; }



        public void PopulateSettings()
        {
            Settings.Clear();

            Settings.Add(new SettingsModel { Name = "IP Settings" });
            Settings.Add(new SettingsModel { Name = "test" });
            Settings.Add(new SettingsModel { Name = "test" });
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(settingsCellId) as SettingsMenuTableViewCell;

            if (cell != null)
            {
                cell.Model = Settings[indexPath.Row];
            }
            return cell;
        }


        public override nint NumberOfSections(UITableView tableView) => 1;

        public override nint RowsInSection(UITableView tableView, nint section) => Settings.Count;



        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return "Main Section";
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var setting = Settings[indexPath.Row];
            setting.Name = "Clicked";

            //Update the UI
            //Controller.TableView.ReloadData();
        }

        public override bool CanFocusRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            Console.WriteLine("Row Focused");
            return true;
        }

    }
}