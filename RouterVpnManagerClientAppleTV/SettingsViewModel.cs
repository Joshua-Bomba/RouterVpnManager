using System;
using System.Collections.Generic;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

namespace RouterVpnManagerClient
{
    public class SettingsDelegate : UITableViewDelegate
    {
        public SettingsDelegate()
        {
            
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            Console.WriteLine("Row Selected");            
        }

        public override bool CanFocusRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            Console.WriteLine("Row Focused");
            // Inform caller of highlight change
            return true;
        }
    }

    public class SettingsModel
    {
        public string Name { get; set; }
    }

    public class SettingsViewModel : UITableViewDataSource
    {
        public static NSString settingsCellId = new NSString("SettingsViewModel");
        public SettingsViewModel() : base()
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

        public override nint RowsInSection(UITableView tableView, nint section) => Settings.Count;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (SettingsViewCell) tableView.DequeueReusableCell(settingsCellId, indexPath);
            var model = Settings[indexPath.Row];
            cell.Model = model;
            return cell;
        }
    }
}