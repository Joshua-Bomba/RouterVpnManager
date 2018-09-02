using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;
// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public class VpnSelectorModel
    {
        public string ImageLocation { get; set; } = "back_graident.png";

        public string Title { get; set; } 

        public bool Selectable { get; set; } = true;


        public VpnSelectorModel()
        {

        }
    }
}