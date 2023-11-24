#if __IOS__
using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace Cartography.DynamicMap;

public static class MapClusterProperties
{
    public static UIColor BackgroundColor { get; set; } = UIColor.Red;
    public static UIColor ForegroundColor { get; set; } = UIColor.Black;
    public static CGRect ClusterSize { get; set; } = new CGRect(0, 0, 40, 40);
    public static UIFont ClusterFontSize { get; set; } = UIFont.BoldSystemFontOfSize(20);
}
#endif