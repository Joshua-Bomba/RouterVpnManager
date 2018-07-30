using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

namespace RouterVpnManagerClient.vpnsViewCollection
{
   public class VpnsCollectionViewCell : UICollectionViewCell
    {

        private VpnsCollectionModel _model;


        public UIImageView Image { get; set; }

        public UILabel Title { get; set; }

        public VpnsCollectionModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                Image.Image =  UIImage.FromFile(_model.ImageLocation);
                Title.Text = _model.Title;
            }
        }

        public VpnsCollectionViewCell(IntPtr handle) : base(handle)
        {
            Image = new UIImageView(new CGRect(22, 19, 320, 171));
            Image.AdjustsImageWhenAncestorFocused = true;
            AddSubview(Image);

            Title = new UILabel(new CGRect(22, 209, 320, 21))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Alpha = 0.0f
            };
            AddSubview(Title);


        }
    }
}