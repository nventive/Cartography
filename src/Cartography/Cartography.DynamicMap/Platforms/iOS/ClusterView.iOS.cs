#if __IOS__
using System;
using CoreGraphics;
using Foundation;
using MapKit;
using UIKit;

namespace Cartography.DynamicMap;

[Register("ClusterView")]
public class ClusterView : MKAnnotationView
{
    /// <summary>
    /// Sets the selector that will be used to create Pin group view templates. The first parameter
    /// is the instance of the native annotation.
    /// </summary>
    public static Func<UIView> ClusterPinTemplate { get; set; }

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

                Image = renderer.CreateImage((context) =>
                {
                    var backgroundColor = MapClusterProperties.BackgroundColor;

                    var foregroundColor = MapClusterProperties.ForegroundColor;

                    backgroundColor.SetFill();
                    UIBezierPath.FromOval(MapClusterProperties.ClusterSize).Fill();

                    var attributes = new UIStringAttributes()
                    {
                        ForegroundColor = foregroundColor,
                        Font = MapClusterProperties.ClusterFontSize
                    };
                    var text = new NSString($"{count}");
                    var size = text.GetSizeUsingAttributes(attributes);
                    var rect = new CGRect(MapClusterProperties.ClusterSize.GetMidX() - size.Width / 2, MapClusterProperties.ClusterSize.GetMidY() - size.Height / 2, size.Width, size.Height);
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

#endif