using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public class SettingsMenuTableViewDelegate : UITableViewDelegate
    {
        public SettingsMenuTableViewController Controller { get; set; }

        public SettingsMenuTableViewDelegate(SettingsMenuTableViewController controller) : base()
        {
            this.Controller = controller;
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var setting = Controller.DataSource.Settings[indexPath.Row];
            setting.Name = "Clicked";

            //Update the UI
            Controller.TableView.ReloadData();
        }

        public override bool CanFocusRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            Console.WriteLine("Row Focused");
            return true;
        }
    }
}