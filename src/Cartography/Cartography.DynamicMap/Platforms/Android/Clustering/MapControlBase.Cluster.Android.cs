#if __ANDROID__
using Android.Gms.Maps;
using Android.Gms.Maps.Utils.Clustering;
using Chinook.DynamicMvvm;
using GeolocatorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Uno.Extensions;
using Uno.Logging;
using Windows.UI.Xaml;

namespace Cartography.DynamicMap;

public partial class MapControlBase
{
    private CustomClusterManager _clusterManager;
    private MapClusterItems _clusterPins;

    private void OnMapClusterReady(GoogleMap map)
    {
        _map = map;

        _padding = Thickness.Empty;

        // Zoomlevel does not need to be manage here. Clustermanager will handle how and when a cluster is shown. 
        // A cluster can show a single pin. The diffrents styles will be manage in ClusterManager.

        _clusterPins = new MapClusterItems(new MapClusterItem[0]);
        _clusterManager = new CustomClusterManager(Context, _map);
        SetupCustomClusterManager(_clusterManager);

        UpdateMapPushpinOnCameraIdle();

        _isReady = true;

        UpdateAutolocateButtonVisibility(AutolocateButtonVisibility);
        UpdateCompassButtonVisibility(CompassButtonVisibility);
        UpdateIsRotateGestureEnabled(IsRotateGestureEnabled);
        UpdateMapStyleJson(MapStyleJson);

        UpdateIcon(PushpinIcon);
        UpdateSelectedIcon(SelectedPushpinIcon);

        TryStart();
    }

    private void SetupCustomClusterManager(CustomClusterManager cluster)
    {
        _map.SetOnMarkerClickListener(cluster);
        _map.SetOnInfoWindowClickListener(cluster);
        cluster.SetOnClusterClickListener(cluster);
        cluster.SetOnClusterInfoWindowClickListener(cluster);
        cluster.SetOnClusterItemClickListener(new ClusterItemClick(this));
        cluster.SetOnClusterItemInfoWindowClickListener(cluster);
    }

    private void UpdateAndroidClusteringPushpins(IGeoLocated[] items, IGeoLocated[] selectedItems)
    {
        if (_clusterPins != null)
        {
            var pinsToAdd = ClusterItemsToAdd(items, false);
            var pinsToRemove = ClusterItemsToRemove(items, false);
            _clusterPins.UpdateItems(pinsToAdd, pinsToRemove);

            var selectedPinsToAdd = ClusterItemsToAdd(selectedItems, true);
            var selectedPinsToRemove = ClusterItemsToRemove(selectedItems, true);
            _clusterPins.UpdateItems(selectedPinsToAdd, selectedPinsToRemove);

            _clusterManager.RemoveItems(pinsToRemove);
            _clusterManager.RemoveItems(selectedPinsToRemove);
            _clusterManager.AddItems(pinsToAdd);
            _clusterManager.AddItems(selectedPinsToAdd);
            _clusterManager.Cluster();
        }
    }

    private MapClusterItem[] ClusterItemsToAdd(IGeoLocated[] items, bool isSelected)
    {
        var mapClusterItemsLookup = _clusterPins.ClusterItems.ToDictionary(mc => mc.Item);

        return items
            .Where(item => !mapClusterItemsLookup.ContainsKey(item))
            .Select(item => new MapClusterItem(item, isSelected))
            .ToArray();
    }

    private MapClusterItem[] ClusterItemsToRemove(IGeoLocated[] items, bool isSelected)
    {
        var ItemLookup = items.ToDictionary(mc => mc);

        return _clusterPins.ClusterItems
            .Where(mapItem => mapItem.IsSelected == isSelected && !ItemLookup.ContainsKey(mapItem.Item))
            .ToArray();
    }

    private class ClusterItemClick : Java.Lang.Object, CustomClusterManager.IOnClusterItemClickListener
    {
        private MapControlBase _control;

        public ClusterItemClick(MapControlBase mapControl)
        {
            _control = mapControl;
        }

        public bool OnClusterItemClick(Java.Lang.Object item)
        {
            var _clusterItems = _control._clusterPins.ClusterItems;
            var clusterItem = (MapClusterItem)item;
            clusterItem.IsSelected = !clusterItem.IsSelected;

            if (!_control.AllowMultipleSelection)
            {
                _clusterItems
                    .Where(item => item != clusterItem)
                    .ForEach(item => item.IsSelected = false);
            }

            var eventSelectedPins = _clusterItems
                .Where(item => item.IsSelected)
                .Select(item => item.Item)
                .ToArray();

            _control._selectedPushpins.OnNext(eventSelectedPins);
            _control._clusterManager.Cluster();

            return true;
        }
    }
}
#endif