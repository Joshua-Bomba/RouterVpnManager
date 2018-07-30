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
    [Register ("VpnsCollectionViewPageController")]
    partial class VpnsCollectionViewPageController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        RouterVpnManagerClient.VpnsCollectionViewController vpnSelection { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (vpnSelection != null) {
                vpnSelection.Dispose ();
                vpnSelection = null;
            }
        }
    }
}