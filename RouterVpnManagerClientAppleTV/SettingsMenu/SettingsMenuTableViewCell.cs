using Foundation;
using System;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public partial class SettingsMenuTableViewCell : UITableViewCell
    {
        private SettingsModel _model;
        public SettingsMenuTableViewCell(IntPtr handle) : base(handle)
        {

        }

        public SettingsModel Model
        {
            get { return _model; }
            set
            {
                try
                {
                    _model = value;

                    TextLabel.Text = _model.Name;
                }
                catch
                {

                }
            }
        }

    }
}