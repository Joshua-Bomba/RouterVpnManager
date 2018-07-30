using Foundation;
using System;
using RouterVpnManagerClient.vpnsViewCollection;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class VpnsCollectionViewController : UICollectionView
    {
        public static AppDelegate App
        {
            get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
        }


        public VpnsCollectionViewController(IntPtr handle) : base(handle)
        {
            RegisterClassForCell(typeof(VpnsCollectionViewCell), VpnsCollectionViewModel.vpnCellId);
            DataSource = new VpnsCollectionViewModel(this);
            Delegate = new VpnsCollectionDelegate();

        }

        public new VpnsCollectionViewModel Source => DataSource as VpnsCollectionViewModel;

        public VpnsCollectionViewPageController ParentController { get; set; }
    }
}