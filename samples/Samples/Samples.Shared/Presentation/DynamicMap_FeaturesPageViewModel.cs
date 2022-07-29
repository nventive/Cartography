using Chinook.DynamicMvvm;
using Uno.Extensions;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Cartography.Core;
using Cartography.DynamicMap;
using GeolocatorService;
using Uno.Logging;
using Samples.Entities;
using System.Collections.Generic;
using Uno;
using Windows.Devices.Geolocation;
using Chinook.SectionsNavigation;

namespace Samples.Presentation
{
    public class DynamicMap_FeaturesPageViewModel : ViewModel, IMapComponent
    {
        private IGeolocatorService _geolocatorService;
        private ISectionsNavigator _sectionsNavigator;

        public DynamicMap_FeaturesPageViewModel()
        {
            _geolocatorService = this.GetService<IGeolocatorService>();
            _sectionsNavigator = this.GetService<ISectionsNavigator>();
            OnLoaded();
        }

        async private void OnLoaded()
        {
            IsLocationEnabled = await _geolocatorService.GetIsPermissionGranted(CancellationToken.None);
            if (!IsLocationEnabled)
            {
                IsLocationEnabled = await _geolocatorService.RequestPermission(CancellationToken.None);
            }

            BuildMap();
        }

        private void BuildMap()
        {

            AddDisposable(ObserveViewPort());
            if (IsLocationEnabled)
            {
                AddDisposable(ObserveUserLocation());
            }
            AddDisposable(ObserveSelectedPushpin());
            Pushpins = GetInitialPushpins();
        }

        #region Page-Property
        public bool IsLocationEnabled
        {
            get => this.Get<bool>();
            set => this.Set(value);
        }

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

        #region Map-Property
        public IGeoLocated[] Pushpins
        {
            get => this.Get<IGeoLocated[]>();
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
            get => this.Get<TimeSpan>(initialValue: TimeSpan.FromMilliseconds(250));
            set => this.Set(value);
        }
        public IEqualityComparer<MapViewPort> ViewPortUpdateFilter
        {
            get => this.Get<IEqualityComparer<MapViewPort>>();
            set => this.Set(value);
        }
        public ActionAsync<Geocoordinate> OnMapTapped
        {
            get => this.Get<ActionAsync<Geocoordinate>>();
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
        public MapViewPortCoordinates ViewPortCoordinates
        {
            get => this.Get<MapViewPortCoordinates>();
            set => this.Set(value);
        }
        public bool SkipAnimations
        {
            get => this.Get<bool>();
            set => this.Set(value);
        }
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

        #region Disposable
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

        private IDisposable ObserveSelectedPushpin()
        {
            return this.GetProperty(x => x.SelectedPushpins).GetAndObserve().Subscribe(UpdateSelectedPushpin);

            void UpdateSelectedPushpin(IGeoLocated[] selectedPushpins)
            {
                if (selectedPushpins != null && selectedPushpins.Length > 0)
                {
                    SelectedPushpin = new PushpinEntity
                    {
                        Name = "Selected Pushpin",
                        Coordinates = selectedPushpins[0].Coordinates
                    };
                }
                else
                {
                    SelectedPushpin = new PushpinEntity
                    {
                        Name = "No Pushpin Selected",
                        Coordinates = new Geopoint(new BasicGeoposition { Latitude = 0, Longitude = 0 })
                    };
                }
            }
        }

        private IDisposable ObserveViewPort()
        {
            return this.GetProperty(x => x.ViewPort).GetAndObserve().Subscribe(getMapViewPortCustomString);

            void getMapViewPortCustomString(MapViewPort viewPort)
            {
                MapViewPortCustomString = "[MapViewPort] Center: lat: {0}, lon: {1}, Zoom: {2}, POIs: {3}".InvariantCultureFormat(
                    viewPort.Center.Position.Latitude,
                    viewPort.Center.Position.Longitude,
                    viewPort.ZoomLevel,
                    string.Join(",", viewPort.PointsOfInterest.Safe()));
            }
        }
        #endregion


        private MapViewPort GetStartingCoordinates()
        {
            var mapViewPort = new MapViewPort(new Geopoint(new BasicGeoposition { Latitude = 45.504071, Longitude = -73.558709 }));
            mapViewPort.ZoomLevel = ZoomLevels.District;
            return mapViewPort;
        }

        private PushpinEntity[] GetInitialPushpins()
        {
            return new[]
            {
                    new PushpinEntity
                    {
                        Name = "Pushpin 1",
                        Coordinates = new Geopoint(new BasicGeoposition{Latitude = 46.3938717, Longitude = -72.0921769})
                    },
                    new PushpinEntity
                    {
                        Name = "Pushpin 2",
                        Coordinates = new Geopoint(new BasicGeoposition { Latitude = 45.5502838, Longitude = -73.2801901 })
                    },
                    new PushpinEntity
                    {
                        Name = "Pushpin 3",
                        Coordinates = new Geopoint(new BasicGeoposition { Latitude = 45.5502838, Longitude = -72.0921769 })
                    },
                };
        }

        public IDynamicCommand LocateMe => this.GetCommandFromTask(async ct =>
        {
            if (IsLocationEnabled)
            {
                try
                {
                    var currentLocation = (await _geolocatorService.GetLocation(ct)).Point;

                    await OnLocateMeSuccess(ct, currentLocation);
                }
                catch
                {
                    OnLocateMeError(ct);
                }

            }
            else
            {
                OnLocateMeError(ct);
            }
        });

        private async Task OnLocateMeSuccess(CancellationToken ct, Geopoint location)
        {
            ClearSelectedPushpin();
            MessageErrorLocateMe = "";

            ViewPort = await ComputeMapViewPort(ct, location);
        }

        private void OnLocateMeError(CancellationToken ct)
        {
            MessageErrorLocateMe = "Cannot get your location this time.";
        }

        private void ClearSelectedPushpin()
        {
            SelectedPushpins = null;
        }

        private async Task<MapViewPort> ComputeMapViewPort(CancellationToken ct, Geopoint mapCenter = null)
        {
            var zoomLevel = ZoomLevels.District;

            if (mapCenter == default(Geopoint))
            {
                var userLocation = await GetUserCoordinates(ct);
                mapCenter = userLocation.Geopoint;
                zoomLevel = userLocation.ZoomLevel;
            }

            var viewPort = new MapViewPort(mapCenter);
            viewPort.ZoomLevel = zoomLevel;

            return viewPort;
        }

        private async Task<GeoViewPort> GetUserCoordinates(CancellationToken ct)
        {
            var defaultGeoPoint = new Geopoint(new BasicGeoposition { Latitude = 45.504071, Longitude = -73.558709 });
            try
            {
                return new GeoViewPort((await _geolocatorService.GetLocationOrDefault(ct))?.Point ?? defaultGeoPoint, ZoomLevels.District);
            }
            catch
            {
                this.Log().Debug(() => "Couldn't get a valid location. Country zoom level will be applied.");
                return new GeoViewPort(defaultGeoPoint, ZoomLevels.District);
            }
        }

        public IDynamicCommand AddPushpin => this.GetCommandFromTask(async ct =>
        {
            var pushpins = Pushpins;

            var newPushpin = await CreatePushpinAtCenter(ct);

            var list = pushpins.ToList();
            list.Add((IGeoLocated)newPushpin);

            Pushpins = list.ToArray();
        });

        public IDynamicCommand FeatureToDynamicMapMenu => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new DynamicMapMenuViewModel());
        });

        private async Task<PushpinEntity> CreatePushpinAtCenter(CancellationToken ct)
        {
            var viewport = await this.GetProperty(x => ViewPort).GetAndObserve().FirstAsync();
            var latitude = (double)viewport.Center.Position.Latitude;
            var longitude = (double)viewport.Center.Position.Longitude;

            return new PushpinEntity()
            {
                Coordinates = new Geopoint(new BasicGeoposition { Latitude = latitude, Longitude = longitude }),
                Name = string.Empty
            };
        }

        public IDynamicCommand RemoveSelectedPushpin => this.GetCommandFromTask(async ct =>
        {
            PushpinEntity selectedPushpin = SelectedPushpin;
            IGeoLocated[] pushpins = Pushpins;

            List<IGeoLocated> pushpinslist = pushpins.ToList();
            pushpinslist.RemoveAll(pin => pin.Coordinates == selectedPushpin.Coordinates);

            Pushpins = pushpinslist.ToArray();
            ClearSelectedPushpin(ct);
        });

        private void ClearSelectedPushpin(CancellationToken ct)
        {
            SelectedPushpins = null;
        }

        public IDynamicCommand UpdateViewPort => this.GetCommand(() =>
        {
            var latitudeString = ViewPortLatitude;
            var longitudeString = ViewPortLongitude;
            if (double.TryParse(latitudeString, out var latitude)
                && double.TryParse(longitudeString, out var longitude)
                && latitude >= -90
                && latitude <= 90
                && longitude >= -180
                && longitude <= 180)
            {
                var center = new Geopoint(new BasicGeoposition { Latitude = latitude, Longitude = longitude });
                var zoomLevel = ZoomLevels.City;
                ViewPort = new MapViewPort(center);
                ViewPort.ZoomLevel = zoomLevel;
            }
            else
            {
                throw new InvalidOperationException("Both latitude and longitude must be valid");
            }
        });
    }
}
