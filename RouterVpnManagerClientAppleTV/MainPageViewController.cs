using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class MainPageViewController : UIViewController
    {
        public MainPageViewController (IntPtr handle) : base (handle)
        {
            RouterVpnManagerWrapper.MainPageController = this;

        }

        public UILabel LblStatus => lblStatus;

        public UIButton BtnSelectAVpn => btnSelectAVpn;

        public UIButton BtnConnect => btnConnect;

        public UIButton BtnSettings => btnSettings;


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetLayout();

        }

        partial void Click_ConnectToServer()
        {
            RouterVpnManagerWrapper.Instance.Connect();
        }



        public void SetLayout()
        {
            //btnStack.Axis = UILayoutConstraintAxis.Vertical;
            //btnStack.Alignment = UIStackViewAlignment.Fill;
            btnStack.Distribution = UIStackViewDistribution.EqualSpacing;
            btnStack.Spacing = 10;
            btnStack.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectAVpn.Enabled = false;
            btnSettings.Enabled = false;
        }

        
    }
}