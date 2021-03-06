﻿using Foundation;
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

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            //Adds reference to the controller in the child element (The VpnsCollectionView)
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            CollectionView.RegisterClassForCell(typeof(VpnSelectorCollectionViewCell), VpnSelectorCollectionViewDataSource.vpnCellId);
            CollectionView.DataSource = new VpnSelectorCollectionViewDataSource(this);
            CollectionView.Delegate = new VpnSelectorCollectionViewDelegateFlowLayout(this);
            

            (CollectionView.DataSource as VpnSelectorCollectionViewDataSource)?.PopulateVpns();
        }

    }
}