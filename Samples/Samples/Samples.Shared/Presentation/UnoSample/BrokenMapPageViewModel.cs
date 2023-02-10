using Cartography.DynamicMap;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using GeolocatorService;
using Samples.Entities;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Text;
using Uno.Extensions;
using Windows.Devices.Geolocation;

namespace Samples.Presentation
{
    internal class BrokenMapPageViewModel : ViewModel, IDynamicMapComponent
    {
        private IGeolocatorService _geolocatorService;
        private ISectionsNavigator _sectionsNavigator;
        private readonly IDispatcherScheduler _dispatcherScheduler;

        public BrokenMapPageViewModel()
        {
            _geolocatorService = this.GetService<IGeolocatorService>();
            _sectionsNavigator = this.GetService<ISectionsNavigator>();
            _dispatcherScheduler = this.GetService<IDispatcherScheduler>();
            OnLoaded();
        }

        private void OnLoaded()
        {
           BuildMap();
        }

        private void BuildMap()
        {
            AddDisposable(ObserveViewPort());
        }

        //These properties are not mandatory but some very usefull.
        #region Page-Property

        // Store if the location is granted by GeolocatorService.
        public bool IsLocationEnabled
        {
            get => this.Get<bool>();
            set => this.Set(value);
        }

        // Store if we follow the user.
        public bool IsMapFollowTheUser
        {
            get => this.Get<bool>(initialValue: false);
            set => this.Set(value);
        }

        //The Animation Setting is include in the Viewport object. Must be add when setting viewport. See ComputeMapViewPort.
        public bool IsViewPortAnimationDisabled
        {
            get => this.Get<bool>(initialValue: false);
            set => this.Set(value);
        }

        // All next properties of this region are only for UI purpose.
        public string MessageErrorLocateMe
        {
            get => this.Get<string>(initialValue: "");
            set => this.Set(value);
        }

        public string ViewPortLongitude
        {
            get => this.Get<string>(initialValue: "0");
            set => this.Set(value);
        }

        public string ViewPortLatitude
        {
            get => this.Get<string>(initialValue: "0");
            set => this.Set(value);
        }

        public PushpinEntity SelectedPushpin
        {
            get => this.Get<PushpinEntity>();
            set => this.Set(value);
        }

        public string MapViewPortCustomString
        {
            get => this.Get<string>();
            set => this.Set(value);
        }
        #endregion

        // These properties are mandatory and depend from IDynamicMapComponent.
        #region Map-Property

        // These Pushpins will be show as inactive.
        public IGeoLocated[] Pushpins
        {
            get => this.Get<IGeoLocated[]>();
            set => this.Set(value);
        }

        // These are your active pushpins.
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

        // 250 ms is a good starting point.
        public TimeSpan ViewPortUpdateMinDelay
        {
            get => this.Get<TimeSpan>(initialValue: MapComponentDefaultValue.DefaultViewPortUpdateMinDelay);
            set => this.Set(value);
        }

        // This is de default Filter. Reject map drifting. Can be not enough precise for some kind of application. ex: Your pushpin might be of center when you place it.
        public IEqualityComparer<MapViewPort> ViewPortUpdateFilter
        {
            get => this.Get<IEqualityComparer<MapViewPort>>(initialValue: MapComponentDefaultValue.DefaultViewPortUpdateFilter);
            set => this.Set(value);
        }

        public Action<Geocoordinate> OnMapTapped
        {
            get => this.Get<Action<Geocoordinate>>();
            set => this.Set(value);
        }

        public bool IsUserTrackingCurrentlyEnabled
        {
            get => this.Get<bool>(initialValue: false);
            set => this.Set(value);
        }

        public bool IsUserDragging
        {
            get => this.Get<bool>(initialValue: false);
            set => this.Set(value);
        }

        public LocationResult UserLocation
        {
            get => this.Get<LocationResult>();
            set => this.Set(value);
        }

        // This calculating the borders of your map.
        public MapViewPortCoordinates ViewPortCoordinates
        {
            get => this.Get<MapViewPortCoordinates>();
            set => this.Set(value);
        }

        // IMPORTANT !!! Must be set. This is where you want to go on the map.
        public MapViewPort ViewPort
        {
            get => this.Get<MapViewPort>(initialValue: GetStartingCoordinates());
            set => this.Set(value);
        }

        public int? AnimationDurationSeconds
        {
            get => this.Get<int?>();
            set => this.Set(value);
        }
        #endregion

        private IDisposable ObserveViewPort()
        {
            return this.GetProperty(x => x.ViewPort).GetAndObserve().Subscribe(getMapViewPortCustomString);

            void getMapViewPortCustomString(MapViewPort viewPort)
            {
                // Generate string to show.
                MapViewPortCustomString = "[MapViewPort] Center: lat: {0}, lon: {1}, Zoom: {2}, POIs: {3}".InvariantCultureFormat(
                    viewPort.Center.Position.Latitude,
                    viewPort.Center.Position.Longitude,
                    viewPort.ZoomLevel,
                    string.Join(",", viewPort.PointsOfInterest.Safe()));
            }
        }

        private MapViewPort GetStartingCoordinates()
        {
            var mapViewPort = new MapViewPort(new Geopoint(new BasicGeoposition { Latitude = 45.504071, Longitude = -73.558709 }));
            mapViewPort.ZoomLevel = ZoomLevels.District;
            mapViewPort.IsAnimationDisabled = IsViewPortAnimationDisabled;
            return mapViewPort;
        }

        public IDynamicCommand BackToMainMenu => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new MainPageViewModel());
        });
    }
}
