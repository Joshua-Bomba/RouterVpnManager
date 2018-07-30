using Foundation;
using System;
using UIKit;
using RouterVpnManagerClient.vpnsViewCollection;

namespace RouterVpnManagerClient
{
    public partial class VpnsCollectionViewController : UICollectionViewController
    {
        public VpnsCollectionViewController (IntPtr handle) : base (handle)
        {
        }

        public VpnsCollectionView Collection => (CollectionView as VpnsCollectionView);

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            //Adds reference to the controller in the child element (The VpnsCollectionView)
            Collection.ParentController = this;
        }
    }
}