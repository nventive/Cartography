#if __IOS__
using System;
using System.Collections.Generic;
using System.Text;

namespace Cartography.DynamicMap;

public static class MapClusterConstants
{
    /// <summary>
    /// approximately 1 miles (1 degree of arc ~= 69 miles)
    /// </summary>
    public const double MINIMUM_ZOOM_ARC = 0.014;
    public const double MAX_DEGREES_ARC = 360.0;
    public const double MAX_GOOGLE_LEVELS = 20;
    /// <summary>
    /// Used to align apple map zoom scale to google map scale
    /// </summary>
    public const double ZOOM_LEVEL_COEFFICIENT = 1.15;
}
#endif