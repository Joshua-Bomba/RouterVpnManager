using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public class VpnSelectorCollectionViewDelegateFlowLayout : UICollectionViewDelegateFlowLayout
    {
        private bool inMethod;
        public VpnSelectorCollectionViewDelegateFlowLayout(VpnSelectorCollectionViewController controller) : base()
        {
            this.Controller = controller;
            inMethod = false;
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
            if (!inMethod)
            {
                inMethod = true;
                Task.Run(() =>
                {
                    bool disconnect = false;
                    int connectionNumber = -1;
                    UIThreadHook.HookOntoGuiThead(() =>
                    {
                        
                        VpnSelectorModel model = Source.Vpns.FirstOrDefault(x => x.Selected);
                        if (model != null)
                        {
                            if (model == Source.Vpns[indexPath.Row]
                            ) //if it's the same model that is already selected then there is no point in marking it again
                                return;
                            //TODO: move the index selection code the broadcast response
                            model.Selected = false;
                            disconnect = true;
                            //RouterVpnManagerWrapper.Instance.DisconnectFromVpn();
                        }

                        Source.Vpns[indexPath.Row].Selected = true;

                        connectionNumber = Source.Vpns[indexPath.Row].ConnectionNumber;
                    });

                    if (connectionNumber == -2)
                    {
                        RouterVpnManagerWrapper.Instance.DisconnectFromVpn();
                    }
                    else
                    {
                        RouterVpnManagerWrapper.Instance.ConnectToVpn(connectionNumber);
                    }

                    UIThreadHook.HookOntoGuiThead(() => {
                        Controller.CollectionView.ReloadData();
                        inMethod = false;
                    });


                });

            }
        }

        public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            //Source.Vpns[indexPath.Row].Selected = false;
            //Controller.CollectionView.ReloadData();
            //_selectedItems.Remove(indexPath);
        }
        public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            //Global.BasicNotificationAlert("Test", "Test", Controller);
            return true;
        }
    }
}