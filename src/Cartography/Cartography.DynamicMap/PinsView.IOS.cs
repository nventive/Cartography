#if __IOS__
using MapKit;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Windows.UI.Xaml.Controls;

namespace Cartography.DynamicMap
{
    public class PinsView : MKAnnotationView
    {
        private nfloat _iconWidth;
        private nfloat _iconHeight;
        private nfloat _selectedIconWidth;
        private nfloat _selectedIconHeight;
        private UIImage _selectedImage;
        private UIImage _unselectedImage;

        public PinsView(IMKAnnotation annotation,
            string reuseIdentifier,
            nfloat iconWidth,
            nfloat iconHeight,
            nfloat selectedIconWidth,
            nfloat selectedIconHeight,
            string unselectedImage,
            string selectedImage) : base(annotation, reuseIdentifier)
        {
            _iconWidth = iconWidth;
            _iconHeight = iconHeight;
            _selectedIconWidth= selectedIconWidth;
            _selectedIconHeight= selectedIconHeight;
            _selectedImage = UIImage.FromBundle(selectedImage.Substring(11));
            _unselectedImage = UIImage.FromBundle(unselectedImage.Substring(11));
        }

        public override void Draw(CoreGraphics.CGRect rect)
        {
            base.Draw(rect);

            // Customize the appearance of the annotation view
            Frame = new CoreGraphics.CGRect(0, 0, _iconWidth, _iconHeight);
            CenterOffset = new CoreGraphics.CGPoint(0, -_iconHeight / 2);
            Image = _unselectedImage;
        }

        public override bool Selected
        {
            get { return base.Selected; }
            set
            {
                base.Selected = value;

                // Change the image of the annotation view when it is selected
                if (value)
                {
                    Frame = new CoreGraphics.CGRect(0, 0, _selectedIconWidth, _selectedIconHeight);
                    CenterOffset = new CoreGraphics.CGPoint(0, -_selectedIconHeight / 2);
                    Image = _selectedImage;
                }
                else
                {
                    Frame = new CoreGraphics.CGRect(0, 0, _iconWidth, _iconHeight);
                    CenterOffset = new CoreGraphics.CGPoint(0, -_iconHeight / 2);
                    Image = _unselectedImage;
                }
            }
        }

        /// <summary>
        /// This means the selection/deselection of this pin
        /// is already handled and shouldn't trigger
        /// additional selection/deselection events (avoid infinite loops).
        /// </summary>
        public bool IsSelectionChangeAlreadyHandled { get; set; }
    }
}
#endif