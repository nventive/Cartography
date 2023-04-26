#if __ANDROID__
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cartography.DynamicMap;

public class MapClusterItem : Java.Lang.Object, IMapControlItem, IClusterItem
{
    public MapClusterItem(IGeoLocated pushpin, bool isSelected = false)
    {
        Item = pushpin;
        Position = new LatLng(pushpin.Coordinates.Position.Latitude, pushpin.Coordinates.Position.Longitude);
        Snippet = null;
        Title = null;
        IsSelected= isSelected;
    }

    public IGeoLocated Item { get; set; }

    public LatLng Position { get; set; }

    public string Snippet { get; set; }

    public string Title { get; set; }

    public bool IsSelected { get; set; }
}
#endif