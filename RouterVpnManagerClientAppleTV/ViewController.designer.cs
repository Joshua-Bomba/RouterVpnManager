// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace RouterVpnManagerClient
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnSettings { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnViewVpns { get; set; }

        [Action ("UIButton397_PrimaryActionTriggered:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton397_PrimaryActionTriggered (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnSettings != null) {
                btnSettings.Dispose ();
                btnSettings = null;
            }

            if (btnViewVpns != null) {
                btnViewVpns.Dispose ();
                btnViewVpns = null;
            }
        }
    }
}