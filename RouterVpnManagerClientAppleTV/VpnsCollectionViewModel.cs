using System;
using System.Collections.Generic;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

namespace RouterVpnManagerClient
{
    public class VpnsCollectionViewModel : UICollectionViewDataSource
    {
        public static NSString vpnCellId = new NSString("VpnCollectionCell");
        
        public VpnsCollectionViewController ViewController { get; set; }

        public List<VpnsCollectionModel> Vpns { get; set; }= new List<VpnsCollectionModel>();


        public VpnsCollectionViewModel(VpnsCollectionViewController viewController) : base()
        {
            ViewController = viewController;
            PopulateVpns();
        }

        public void PopulateVpns()
        {
            Vpns.Clear();

            Vpns.Add(new VpnsCollectionModel{ImageLocation = "back_graident.png", Title = "Hello World!"});

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
            var cell = (VpnsCollectionViewCell) collectionView.DequeueReusableCell(vpnCellId, indexPath);
            var model = Vpns[indexPath.Row];
            cell.Model = model;
            return cell;
        }
    }
}