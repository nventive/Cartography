#if __ANDROID__
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Utils.Clustering;
using Android.Gms.Maps.Utils.Collections;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cartography.DynamicMap
{
    public class CustomClusterManager : ClusterManager, ClusterManager.IOnClusterClickListener, ClusterManager.IOnClusterInfoWindowClickListener, ClusterManager.IOnClusterItemInfoWindowClickListener
    {
        private ClusterManager _clusterManager;

        public CustomClusterManager(Context context, GoogleMap map) : base(context, map)
        {
        }

        public CustomClusterManager(Context context, GoogleMap map, MarkerManager makerManager) : base(context, map, makerManager)
        {
        }

        public bool OnClusterClick(ICluster cluster)
        {
            return false;
        }

        public void OnClusterInfoWindowClick(ICluster cluster)
        {
        }

        public void OnClusterItemInfoWindowClick(Java.Lang.Object item)
        {
        }
    }
}
#endif