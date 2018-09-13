using Foundation;
using System;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public partial class VpnSelectorCollectionViewController : UICollectionViewController
    {
        public VpnSelectorCollectionViewController(IntPtr handle) : base(handle)
        {
        }

        //public VpnSelectorCollectionView Collection => (CollectionView as VpnSelectorCollectionView);

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CollectionView.RegisterClassForCell(typeof(VpnSelectorCollectionViewCell), VpnSelectorCollectionViewSource.vpnCellId);
            CollectionView.Source = new VpnSelectorCollectionViewSource(this);

            (CollectionView.Source as VpnSelectorCollectionViewSource)?.PopulateVpns();
        }
    }
}