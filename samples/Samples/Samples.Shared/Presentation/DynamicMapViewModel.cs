using Cartography.Core;
using Cartography.DynamicMap;
using GeolocatorService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Uno;
using Windows.Devices.Geolocation;
using Chinook.DynamicMvvm;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace Samples.ViewModel
{
    class DynamicMapViewModel : ViewModelBase, IMapComponent
    {
        private readonly IGeolocatorService _geolocatorService;
        private bool IsLocationEnabled;

        public DynamicMapViewModel()
        {
            _geolocatorService = new GeolocatorService.GeolocatorService();
            GetPermission();
            BuildMap();
        }

        public IGeoLocated[] Pushpins
        {
            get => this.Get(initialValue: Array.Empty<IGeoLocated>());
            set => this.Set(value);
        }

        public IGeoLocated[] SelectedPushpins
        {
            get => this.Get(initialValue: Array.Empty<IGeoLocated>());
            set => this.Set(value);
        }

        public IGeoLocatedGrouping<IGeoLocated[]> Groups
        {
            get => this.Get<IGeoLocatedGrouping<IGeoLocated[]>>();
            set => this.Set(value);
        }

        public TimeSpan ViewPortUpdateMinDelay
        {
            get => this.Get(initialValue: MapComponentDefaultValue.DefaultViewPortUpdateMinDelay);
            set => this.Set(value);
        }

        public IEqualityComparer<MapViewPort> ViewPortUpdateFilter
        {
            get => this.Get(initialValue: MapComponentDefaultValue.DefaultViewPortUpdateFilter);
            set => this.Set(value);
        }

        public ActionAsync<Geocoordinate> OnMapTapped
        {
            get => this.Get(initialValue: (ActionAsync<Geocoordinate>)null);
            set => this.Set(value);
        }

        public bool IsUserTrackingCurrentlyEnabled
        {
            get => this.Get(initialValue: false);
            set => this.Set(value);
        }

        public bool IsUserDragging
        {
            get => this.Get(initialValue: false);
            set => this.Set(value);
        }

        public LocationResult UserLocation
        {
            get => this.Get<LocationResult>();
            set => this.Set(value);
        }

        public MapViewPortCoordinates ViewPortCoordinates
        {
            get => this.Get<MapViewPortCoordinates>();
            set => this.Set(value);
        }

        public bool SkipAnimations { get; set; }

        public MapViewPort ViewPort
        {
            get => this.Get(initialValue: new MapViewPort());
            set => this.Set(value);
        }

        public int? AnimationDurationSeconds
        {
            get => this.Get<int>();
            set => this.Set(value);
        }

        private async void GetPermission()
        {
            IsLocationEnabled = await _geolocatorService.RequestPermission(CancellationToken.None);
        }

        private void BuildMap()
        {
            ViewPort = GetInitialCoordinates();
            OnMapTapped = ClearSelectedPushpin;

            if (IsLocationEnabled)
            {
                AddDisposable(ObserveUserLocation());
            }
        }

        private MapViewPort GetInitialCoordinates()
        {
            var mapViewPort = new MapViewPort(new Geopoint(new BasicGeoposition { Latitude = 45.504071, Longitude = -73.558709 }));
            mapViewPort.ZoomLevel = ZoomLevels.District;

            return mapViewPort;
        }

        private async Task ClearSelectedPushpin(CancellationToken ct, Geocoordinate geocoordinate)
        {
            SelectedPushpins = null;
        }

        private IDisposable ObserveUserLocation()
        {
            var obs = Observable.CombineLatest(
                _geolocatorService.GetAndObserveLocationOrDefault(),
                this.GetProperty(x => IsLocationEnabled).GetAndObserve(),
                (location, isEnabled) => (location, isEnabled));

            void UpdateLocationState((Geocoordinate location, bool isLocationEnabled) result)
            {
                if (!result.isLocationEnabled)
                {
                    return;
                }

                UserLocation = new LocationResult(true, result.location);
            }
            return obs.Subscribe(UpdateLocationState);
        }
    }
}
