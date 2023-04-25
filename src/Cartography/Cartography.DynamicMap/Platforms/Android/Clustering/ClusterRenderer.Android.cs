#if __ANDROID__
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using Android.Gms.Maps.Utils.Clustering.View;
using Android.Gms.Maps.Utils.UI;
using Android.Graphics.Drawables;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Windows.UI.Xaml;
using Context = Android.Content.Context;

namespace Cartography.DynamicMap;

internal class ClusterRenderer : DefaultClusterRenderer
{
    private DataTemplate _iconTemplate;
    private DataTemplate _clusterTemplate;

    public ClusterRenderer(DataTemplate iconTemplate, DataTemplate clusterTemplate, Context context, GoogleMap map, ClusterManager clusterManager) : base(context, map, clusterManager)
    {
        _iconTemplate = iconTemplate;
        _clusterTemplate = clusterTemplate;
    }
}
#endif