using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class SettingsViewCell : UITableViewCell
    {
        private SettingsModel _model;
        public SettingsViewCell (IntPtr handle) : base (handle)
        {
            
        }

        public SettingsModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                TextLabel.Text = _model.Name;
            }
        }

    }
}