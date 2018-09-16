using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;
// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public class VpnSelectorCollectionViewDataSource : UICollectionViewDataSource
    {
        public static NSString vpnCellId = new NSString("VpnCollectionCell");
        
        public VpnSelectorCollectionViewController Controller { get; set; }

        public List<VpnSelectorModel> Vpns { get; set; }= new List<VpnSelectorModel>();


        public VpnSelectorCollectionViewDataSource(VpnSelectorCollectionViewController controller) : base()
        {
            Controller = controller;
            RouterVpnManagerWrapper.Instance.VpnSelectorDataSource = this;
        }

        public void PopulateVpns()
        {
            Vpns = RouterVpnManagerWrapper.Instance.GetVpns();
            Vpns.Insert(0, new VpnSelectorModel { ImageLocation = "back_graident.png", Title = "Disconnect", ConnectionNumber = -2 });
            //for (int i = 1; i <= 60; i++)
            //{
            //    Vpns.Add(new VpnSelectorModel { ImageLocation = "back_graident.png", Title = "Item " + i, ConnectionNumber = -2 });
            //}
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
            if (cell != null)
            {
                var model = Vpns[indexPath.Row];
                cell.Model = model;
            }

            return cell;
        }

        

    }
}