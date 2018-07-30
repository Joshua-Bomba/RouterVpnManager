using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

namespace RouterVpnManagerClient.vpnsViewCollection
{
    public class VpnsCollectionDelegate : UICollectionViewDelegateFlowLayout
    {
        public VpnsCollectionDelegate() : base()
        {

        }
        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return new CGSize(361, 256);
        }

        public override bool CanFocusItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (indexPath == null)
            {
                return false;
            }
            else
            {
                var controller = collectionView as VpnsCollectionViewController;
                return controller.Source.Vpns[indexPath.Row].Selectable;
            }
        }

        //public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        //{
        //    var view = collectionView as VpnsCollectionView;
        //    App

        //}
    }
}