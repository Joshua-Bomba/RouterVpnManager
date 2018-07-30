using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

namespace RouterVpnManagerClient.vpnsViewCollection
{
    public class VpnsCollectionModel
    {
        public string ImageLocation { get; set; }

        public string Title { get; set; }

        public bool Selectable { get; set; } = true;


        public VpnsCollectionModel()
        {

        }
    }
}