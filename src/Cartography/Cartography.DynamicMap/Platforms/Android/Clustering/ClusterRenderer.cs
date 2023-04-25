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

namespace Cartography.DynamicMap.Platforms.Android.Clustering
{
    internal class ClusterRenderer : DefaultClusterRenderer
    {
        private DataTemplate _iconTemplate;
        private DataTemplate _clusterTemplate;

        public ClusterRenderer(DataTemplate iconTemplate, DataTemplate clusterTemplate, Context context, GoogleMap map, ClusterManager clusterManager) : base(context, map, clusterManager)
        {
            _iconTemplate = iconTemplate;
            _clusterTemplate = clusterTemplate;
        }

        //protected override void OnBeforeClusterItemRendered(Java.Lang.Object item, MarkerOptions markerOptions)
        //{
        //    base.OnBeforeClusterItemRendered(item, markerOptions);
        //    // Draw a single person - show their profile photo and set the info window to show their name
        //    var clusterItem = item as MapClusterItem;
        //    markerOptions.SetIcon(getItemIcon(clusterItem)).SetTitle(clusterItem.Title);
        //}

        //private BitmapDescriptor getItemIcon(MapClusterItem clusterItem)
        //{
        //    Bitmap iconImage = new Bitmap(30, 30);
        //    _iconTemplate.DrawToBitmap(iconImage, new Rectangle(0, 0, iconImage.Width, iconImage.Height));
        //    return BitmapDescriptorFactory.FromBitmap(icon);
        //}

        //protected override void OnBeforeClusterRendered(ICluster cluster, MarkerOptions markerOptions)
        //{
        //    // Draw multiple people.
        //    // Note: this method runs on the UI thread. Don't spend too much time in here (like in this example).
        //    markerOptions.SetIcon(getClusterIcon(cluster));
        //}

        //private BitmapDescriptor getClusterIcon(ICluster cluster)
        //{

        //}
    }
}
#endif