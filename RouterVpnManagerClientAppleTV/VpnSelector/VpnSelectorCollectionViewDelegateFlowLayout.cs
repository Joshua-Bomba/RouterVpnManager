using System;
using System.Collections.Generic;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public class VpnSelectorCollectionViewDelegateFlowLayout : UICollectionViewDelegateFlowLayout
    {
        public VpnSelectorCollectionViewDelegateFlowLayout(VpnSelectorCollectionViewController controller) : base()
        {
            this.Controller = controller;
        }
        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return new CGSize(361, 256);
        }

        public VpnSelectorCollectionViewController Controller { get; private set; }

        public override bool CanFocusItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (indexPath == null)
            {
                return false;
            }
            else
            {
                return Controller?.CollectionView?.DataSource != null && ((VpnSelectorCollectionViewDataSource) Controller.CollectionView.DataSource).Vpns[indexPath.Row].Selectable;
            }
        }

        public NSIndexPath[] SelectedItems { get { return _selectedItems.ToArray(); } }
        readonly List<NSIndexPath> _selectedItems = new List<NSIndexPath>();

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            base.ItemSelected(collectionView, indexPath);
            _selectedItems.Add(indexPath);
            Global.BasicNotificationAlert("Test", "Test", Controller);
        }

        public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            base.ItemDeselected(collectionView, indexPath);
            _selectedItems.Remove(indexPath);
        }
        public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Global.BasicNotificationAlert("Test", "Test", Controller);
            return base.ShouldHighlightItem(collectionView, indexPath);
        }
    }
}