#if __IOS__
using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using MapKit;
using CoreGraphics;
using CoreLocation;
using System.Drawing;
using Uno.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Cartography.DynamicMap
{
    [Register("ClusterView")]
    public class ClusterView : MKAnnotationView
    {
        /// <summary>
        /// Sets the selector that will be used to create Pin group view templates. The first parameter
        /// is the instance of the native annotation.
        /// </summary>
        public static Func<UIView> ClusterPinTemplate { get; set; }

        public static DataTemplate ClusterTemplate { get; set; }

        public static UIColor ClusterColor = UIColor.FromRGB(202, 150, 38);
        public override IMKAnnotation Annotation
        {
            get
            {
                return base.Annotation;
            }
            set
            {
                base.Annotation = value;
                var cluster = MKAnnotationWrapperExtensions.UnwrapClusterAnnotation(value);
                if (cluster != null)
                {
                    var renderer = new UIGraphicsImageRenderer(new CGSize(40, 40));
                    var count = cluster.MemberAnnotations.Length;

                    Image = renderer.CreateImage(async (context) =>
                    {
                        ContentControl contentControl = new ContentControl()
                        {
                            ContentTemplate = ClusterPinTemplate
                        };

                        // Render the content control into a bitmap
                        RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                        await renderTargetBitmap.RenderAsync(contentControl, 40, 40);

                        // Convert the bitmap into a UIImage
                        var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
                        NSData data = NSData.FromArray(pixelBuffer.ToArray());
                        UIImage image = UIImage.LoadFromData(data);

                        // Draw the image in the center of the context
                        image.Draw(new CGRect(0,0, image.Size.Width, image.Size.Height));

                        UIColor.Red.SetFill();
                        UIBezierPath.FromOval(new CGRect(0, 0, 40, 40)).Fill();

                        // Draw pushpin count
                        var foregroundColor = UIColor.Black;
                        var attributes = new UIStringAttributes()
                        {
                            ForegroundColor = foregroundColor,
                            Font = UIFont.BoldSystemFontOfSize(20)
                        };
                        var text = new NSString($"{count}");
                        var rectSize = text.GetSizeUsingAttributes(attributes);
                        var rect = new CGRect(20 - rectSize.Width / 2, 20 - rectSize.Height / 2, rectSize.Width, rectSize.Height);
                        text.DrawString(rect, attributes);
                    });
                }
            }
        }
        #region Constructors
        public ClusterView()
        {
        }

        public ClusterView(NSCoder coder) : base(coder)
        {
        }

        public ClusterView(IntPtr handle) : base(handle)
        {
        }

        public ClusterView(IMKAnnotation annotation, string reuseIdentifier) : base(annotation, reuseIdentifier)
        {
            // Initialize
            DisplayPriority = MKFeatureDisplayPriority.DefaultHigh;
            CollisionMode = MKAnnotationViewCollisionMode.Circle;

            // Offset center point to animate better with marker annotations
            CenterOffset = new CoreGraphics.CGPoint(0, -10);
        }
        #endregion
    }

    public static class MKAnnotationWrapperExtensions
    {
        public static MKClusterAnnotation UnwrapClusterAnnotation(IMKAnnotation annotation)
        {
            if (annotation == null) return null;
            return ObjCRuntime.Runtime.GetNSObject(annotation.Handle) as MKClusterAnnotation;
        }
    }
}
#endif