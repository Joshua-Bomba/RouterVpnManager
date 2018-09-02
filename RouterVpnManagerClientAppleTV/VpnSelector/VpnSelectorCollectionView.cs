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
            RegisterClassForCell(typeof(VpnSelectorCollectionViewCell), VpnSelectorCollectionViewDataSource.vpnCellId);
            DataSource = new VpnSelectorCollectionViewDataSource(this);
            Delegate = new VpnSelectorCollectionViewDelegateFlowLayout();

        }

        public new VpnSelectorCollectionViewDataSource Source => DataSource as VpnSelectorCollectionViewDataSource;

        public VpnSelectorCollectionViewController ParentController { get; set; }
    }
}