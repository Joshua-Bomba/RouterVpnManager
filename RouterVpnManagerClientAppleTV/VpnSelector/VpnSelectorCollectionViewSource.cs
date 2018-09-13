using System;
using System.Collections.Generic;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;
// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public class VpnSelectorCollectionViewSource : UICollectionViewSource
    {
        public static NSString vpnCellId = new NSString("VpnCollectionCell");
        
        public VpnSelectorCollectionViewController ViewController { get; set; }

        public List<VpnSelectorModel> Vpns { get; set; }= new List<VpnSelectorModel>();


        public VpnSelectorCollectionViewSource(VpnSelectorCollectionViewController viewController) : base()
        {
            ViewController = viewController;
            RouterVpnManagerWrapper.Instance.VpnSelectorSource = this;
        }

        public void PopulateVpns()
        {
            Vpns = RouterVpnManagerWrapper.Instance.GetVpns();
            Vpns.Insert(0, new VpnSelectorModel { ImageLocation = "back_graident.png", Title = "Disconnect",ConnectionNumber = -2});
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Vpns.Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)

        {
            var cell = (VpnSelectorCollectionViewCell) collectionView.DequeueReusableCell(vpnCellId, indexPath);
            var model = Vpns[indexPath.Row];
            cell.Model = model;
            return cell;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            base.ItemSelected(collectionView, indexPath);
            //_selectedItems.Add(indexPath);
            Global.BasicNotificationAlert("Test", "Test", ViewController);
        }

        public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            base.ItemDeselected(collectionView, indexPath);
            Global.BasicNotificationAlert("Test", "Test", ViewController);
            //_selectedItems.Remove(indexPath);
        }

        public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Global.BasicNotificationAlert("Test", "Test", ViewController);
            return base.ShouldHighlightItem(collectionView, indexPath);
        }
    }
}