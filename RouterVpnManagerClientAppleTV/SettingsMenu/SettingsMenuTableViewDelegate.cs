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
        public SettingsMenuTableViewController Controller { get; private set; }

        public SettingsMenuTableViewDelegate(SettingsMenuTableViewController controller) : base()
        {
            Controller = controller;
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            Console.WriteLine("Row Selected");
        }

        public override bool CanFocusRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            Console.WriteLine("Row Focused");
            // Inform caller of highlight change
            this.HighLight?.Invoke(Controller.DataSource.Settings[indexPath.Row]);
            return true;
        }

        public delegate void CanFocusRowDelegate(SettingsModel model);

        public event CanFocusRowDelegate HighLight;

    }
}