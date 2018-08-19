using System;
using System.Collections.Generic;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public class SettingsModel : NSObject
    {
        public string Name { get; set; }
    }

    public class SettingsMenuTableViewDataSource : UITableViewDataSource
    {
        public const string settingsCellId = "SettingsViewModel";


        public SettingsMenuTableViewController Controller { get; set; }

        public List<SettingsModel> Settings { get; set; }

        public SettingsMenuTableViewDataSource(SettingsMenuTableViewController controller)
        {
            this.Controller = controller;
            this.Settings = new List<SettingsModel>();
            PopulateSettings();
        }



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

    }
}