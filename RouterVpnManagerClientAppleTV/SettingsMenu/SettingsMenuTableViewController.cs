﻿using Foundation;
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

        public SettingsMenuTableViewDelegate TableDelegate => TableView.Delegate as SettingsMenuTableViewDelegate;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.DataSource = new SettingsMenuTableViewDataSource(this);
            TableView.Delegate = new SettingsMenuTableViewDelegate(this);
            TableView.ReloadData();

            TableView.RegisterClassForCellReuse(typeof(SettingsMenuTableViewCell), SettingsMenuTableViewDataSource.settingsCellId);
        }
    }
}

