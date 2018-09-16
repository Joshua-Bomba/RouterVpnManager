using CoreGraphics;
using Foundation;
using System;
using UIKit;
// ReSharper disable once CheckNamespace
namespace RouterVpnManagerClient
{
    public partial class VpnSelectorCollectionViewCell : UICollectionViewCell
    {
        private VpnSelectorModel _model;


        public UIImageView Image { get; set; }

        public UILabel Title { get; set; }

        public VpnSelectorModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                if (_model?.ImageLocation != null)
                {
                    Image.Image = UIImage.FromFile(_model.ImageLocation);
                }

                Title.Text = _model.Title;
            }
        }

        public VpnSelectorCollectionViewCell(IntPtr handle) : base(handle)
        {

            CreateUI();

        }

        public void CreateUI()
        {
            Image = new UIImageView(new CGRect(22, 19, 320, 171));
            Image.AdjustsImageWhenAncestorFocused = true;

            if (Model?.ImageLocation != null)
            {
                Image.Image = UIImage.FromFile(_model.ImageLocation);
            }

            ContentView.AddSubview(Image);

            Title = new UILabel(new CGRect(22, 209, 320, 21))
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.Black,
                Alpha = 1.0f,
                Font = UIFont.PreferredFootnote.WithSize(16f),
                Text = Model?.Title
            };

            ContentView.AddSubview(Title);
        }
    }
}