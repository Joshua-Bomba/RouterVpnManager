using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

namespace RouterVpnManagerClient.vpnsViewCollection
{
    public class VpnsCollectionView : UICollectionView
    {
        public static AppDelegate App
        {
            get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
        }


        public VpnsCollectionView(IntPtr handle) : base(handle)
        {
            RegisterClassForCell(typeof(VpnsCollectionViewCell), VpnsCollectionViewModel.vpnCellId);
            DataSource = new VpnsCollectionViewModel(this);
            Delegate = new VpnsCollectionDelegate();
            
        }

        public new VpnsCollectionViewModel Source => DataSource as VpnsCollectionViewModel;

        public VpnsCollectionViewController ParentController { get; set; }
    }
}