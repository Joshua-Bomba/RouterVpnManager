using Foundation;
using System;
using UIKit;
// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public partial class VpnSelectorCollectionView : UICollectionView
    {
        public static AppDelegate App
        {
            get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
        }


        public VpnSelectorCollectionView(IntPtr handle) : base(handle)
        {
            RegisterClassForCell(typeof(VpnSelectorCollectionViewCell), VpnSelectorCollectionViewSource.vpnCellId);
            Source = new VpnSelectorCollectionViewSource(ParentController);
            //Delegate = new VpnSelectorCollectionViewDelegateFlowLayout(ParentController);
            

        }

        //public new VpnSelectorCollectionViewSource Source => Source as VpnSelectorCollectionViewSource;

        public VpnSelectorCollectionViewController ParentController { get; set; }
    }
}