using System;
using System.Collections.Generic;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public class SettingsModel
    {
        public string Name { get; set; }
    }

    public class SettingsMenuTableViewDataSource : UITableViewDataSource
    {
        public static NSString settingsCellId = new NSString("SettingsViewModel");
        public SettingsMenuTableViewDataSource() : base()
        {
            PopulateSettings();
        }

        public List<SettingsModel> Settings { get; set; } = new List<SettingsModel>();

        public void PopulateSettings()
        {
            Settings.Clear();

            Settings.Add(new SettingsModel { Name = "IP Settings" });
            Settings.Add(new SettingsModel { Name = "test" });
            Settings.Add(new SettingsModel { Name = "test" });
        }


        public override nint NumberOfSections(UITableView tableView) => 1;

        public override nint RowsInSection(UITableView tableView, nint section) => Settings.Count;

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return "Main Section";
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(settingsCellId) as SettingsMenuTableViewCell ?? new SettingsMenuTableViewCell(Handle);
            cell.Model = Settings[indexPath.Row];
            return cell;
        }
    }
}