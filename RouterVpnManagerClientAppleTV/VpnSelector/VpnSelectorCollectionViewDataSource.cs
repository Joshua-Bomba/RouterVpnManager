﻿using System;
using System.Collections.Generic;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;
// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public class VpnSelectorCollectionViewDataSource : UICollectionViewDataSource
    {
        public static NSString vpnCellId = new NSString("VpnCollectionCell");
        
        public VpnSelectorCollectionView ViewController { get; set; }

        public List<VpnSelectorModel> Vpns { get; set; }= new List<VpnSelectorModel>();


        public VpnSelectorCollectionViewDataSource(VpnSelectorCollectionView viewController) : base()
        {
            ViewController = viewController;
            RouterVpnManagerWrapper.Instance.VpnSelectorDataSource = this;
        }

        public void PopulateVpns()
        {
            //Vpns.Add(new VpnSelectorModel{ImageLocation = "back_graident.png", Title = "Hello World!"});
            Vpns = RouterVpnManagerWrapper.Instance.GetVpns();
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
    }
}