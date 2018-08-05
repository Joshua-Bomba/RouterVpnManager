// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace RouterVpnManagerClient
{
    [Register ("MainPageViewController")]
    partial class MainPageViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView btnStack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblStatus { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblStatusTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (btnStack != null) {
                btnStack.Dispose ();
                btnStack = null;
            }

            if (lblStatus != null) {
                lblStatus.Dispose ();
                lblStatus = null;
            }

            if (lblStatusTitle != null) {
                lblStatusTitle.Dispose ();
                lblStatusTitle = null;
            }
        }
    }
}