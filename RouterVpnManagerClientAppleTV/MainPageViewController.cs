using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class MainPageViewController : UIViewController
    {
        public MainPageViewController (IntPtr handle) : base (handle)
        {


        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            btnStack.Axis = UILayoutConstraintAxis.Vertical;
            btnStack.Alignment = UIStackViewAlignment.Fill;
            btnStack.Distribution = UIStackViewDistribution.EqualSpacing;
            btnStack.Spacing = 10;
            btnStack.TranslatesAutoresizingMaskIntoConstraints = false;

        }

        
    }
}