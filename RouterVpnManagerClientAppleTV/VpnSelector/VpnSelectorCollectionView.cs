using Foundation;
using System;
using UIKit;
// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    //This code is proably never ran
    public partial class VpnSelectorCollectionView : UICollectionView
    {
        public static AppDelegate App
        {
            get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
        }


        public VpnSelectorCollectionView(IntPtr handle) : base(handle)
        {

        }

        //public new VpnSelectorCollectionViewSource Source => Source as VpnSelectorCollectionViewSource;

        public VpnSelectorCollectionViewController ParentController { get; set; }
    }
}