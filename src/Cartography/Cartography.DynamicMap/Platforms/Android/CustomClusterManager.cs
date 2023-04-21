#if __ANDROID__
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Utils.Clustering;
using Android.Gms.Maps.Utils.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cartography.DynamicMap
{
    public class CustomClusterManager : ClusterManager, ClusterManager.IOnClusterClickListener, ClusterManager.IOnClusterItemClickListener, ClusterManager.IOnClusterInfoWindowClickListener, ClusterManager.IOnClusterItemInfoWindowClickListener
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
            throw new NotImplementedException();
        }

        public void OnClusterInfoWindowClick(ICluster cluster)
        {
            throw new NotImplementedException();
        }

        public bool OnClusterItemClick(Java.Lang.Object item)
        {
            throw new NotImplementedException();
        }

        public void OnClusterItemInfoWindowClick(Java.Lang.Object item)
        {
            throw new NotImplementedException();
        }
    }
}
#endif