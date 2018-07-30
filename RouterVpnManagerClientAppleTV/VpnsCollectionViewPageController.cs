using Foundation;
using System;
using UIKit;

namespace RouterVpnManagerClient
{
    public partial class VpnsCollectionViewPageController : UICollectionViewController
    {
        public VpnsCollectionViewPageController(IntPtr handle) : base(handle)
        {
        }

        public VpnsCollectionViewController Collection => (CollectionView as VpnsCollectionViewController);

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            //Adds reference to the controller in the child element (The VpnsCollectionView)
            Collection.ParentController = this;
        }
    }
}