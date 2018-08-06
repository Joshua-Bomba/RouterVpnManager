using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class MainPageViewController : UIViewController
    {
        public static RouterVpnManagerWrapper vpnManager_;
        public MainPageViewController (IntPtr handle) : base (handle)
        {


        }

        public UILabel LblStatus => lblStatus;

        public UIButton BtnSelectAVpn => btnSelectAVpn;

        public UIButton BtnConnect => btnConnect;


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetLayout();

            if(vpnManager_ == null)
                vpnManager_ = new RouterVpnManagerWrapper(this);

        }

        partial void Click_ConnectToServer(UIKit.UIButton sender)
        {
            vpnManager_.Connect();
        }



        public void SetLayout()
        {
            //btnStack.Axis = UILayoutConstraintAxis.Vertical;
            //btnStack.Alignment = UIStackViewAlignment.Fill;
            btnStack.Distribution = UIStackViewDistribution.EqualSpacing;
            btnStack.Spacing = 10;
            btnStack.TranslatesAutoresizingMaskIntoConstraints = false;
            btnSelectAVpn.Enabled = false;
        }

        
    }
}