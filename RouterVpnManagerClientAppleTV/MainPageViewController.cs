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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SetLayout();

            if(vpnManager_ == null)
                vpnManager_ = new RouterVpnManagerWrapper();

        }



        public void SetLayout()
        {
            btnStack.Axis = UILayoutConstraintAxis.Vertical;
            btnStack.Alignment = UIStackViewAlignment.Fill;
            btnStack.Distribution = UIStackViewDistribution.EqualSpacing;
            btnStack.Spacing = 10;
            btnStack.TranslatesAutoresizingMaskIntoConstraints = false;
        }

        
    }
}