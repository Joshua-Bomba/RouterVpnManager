using System;
using System.Collections.Generic;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

namespace RouterVpnManagerClient.vpnsViewCollection
{
    public class VpnsCollectionViewModel : UICollectionViewDataSource
    {
        public static NSString vpnCellId = new NSString("VpnCollectionCell");
        
        public VpnsCollectionView ViewController { get; set; }

        public List<VpnsCollectionModel> Vpns { get; set; }= new List<VpnsCollectionModel>();


        public VpnsCollectionViewModel(VpnsCollectionView view) : base()
        {
            ViewController = view;
            PopulateVpns();
        }

        public void PopulateVpns()
        {
            Vpns.Clear();

            Vpns.Add(new VpnsCollectionModel{ImageLocation = "", Title = "Hello World!"});

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