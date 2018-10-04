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

        public VpnSelectorCollectionViewDataSource Source =>
            Controller.CollectionView.DataSource as VpnSelectorCollectionViewDataSource;

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

        //public NSIndexPath[] SelectedItems { get { return _selectedItems.ToArray(); } }
        //readonly List<NSIndexPath> _selectedItems = new List<NSIndexPath>();

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            //TODO: finish implementing this
            //Global.BasicNotificationAlert("Test", "Test", Controller);
            VpnSelectorModel model = Source.Vpns.FirstOrDefault(x => x.Selected);
            if (model != null)
            {
                if (model == Source.Vpns[indexPath.Row])//if it's the same model that is already selected then there is no point in marking it again
                    return;
                //TODO: move the index selection code the broadcast response
                model.Selected = false;
                RouterVpnManagerWrapper.Instance.DisconnectFromVpn();  
            }

            Source.Vpns[indexPath.Row].Selected = true;

            if (Source.Vpns[indexPath.Row].ConnectionNumber == -2)
            {
                RouterVpnManagerWrapper.Instance.DisconnectFromVpn();
            }
            else
            {
                RouterVpnManagerWrapper.Instance.ConnectToVpn(Source.Vpns[indexPath.Row].ConnectionNumber);
            }

            Controller.CollectionView.ReloadData();


        }

        public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Source.Vpns[indexPath.Row].Selected = false;
            Controller.CollectionView.ReloadData();
            //_selectedItems.Remove(indexPath);
        }
        public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            //Global.BasicNotificationAlert("Test", "Test", Controller);
            return true;
        }
    }
}