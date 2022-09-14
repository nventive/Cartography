using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Cartography.DynamicMap;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using GeolocatorService;
using Samples.Entities;
using Uno;
using Windows.Devices.Geolocation;

namespace Samples.Presentation
{
	public class DynamicMap_MoveSearchPageViewModel : ViewModel, IDynamicMapComponent
    {
        /* See FeaturePageViewmodel for implementation of the map.
         * ---
         * This sample only show only the pushpins that are contain within the ViewPortCoordinates (what is show on the screen)
         */
        private IGeolocatorService _geolocatorService;
        private ISectionsNavigator _sectionsNavigator;

        public DynamicMap_MoveSearchPageViewModel()
        {
            _geolocatorService = this.GetService<IGeolocatorService>();
            _sectionsNavigator = this.GetService<ISectionsNavigator>();

            ViewPort = GetStartingCoordinates();
            Pushpins = GetPushpins(ViewPortCoordinates);

            AddDisposable(ObserveViewPort());
        }


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
            get => this.Get<TimeSpan>(initialValue: MapComponentDefaultValue.DefaultViewPortUpdateMinDelay);
            set => this.Set(value);
        }
        public IEqualityComparer<MapViewPort> ViewPortUpdateFilter
        {
            get => this.Get<IEqualityComparer<MapViewPort>>(initialValue: MapComponentDefaultValue.DefaultViewPortUpdateFilter);
            set => this.Set(value);
        }
        public Action<Geocoordinate> OnMapTapped
        {
            get => this.Get<Action<Geocoordinate>>(initialValue: (Action<Geocoordinate>)null);
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

        public MapViewPort ViewPort
        {
            get => this.Get<MapViewPort>();
            set => this.Set(value);
        }
        public int? AnimationDurationSeconds
        {
            get => this.Get<int?>();
            set => this.Set(value);
        }

        public IDynamicCommand SearchToDynamicMapMenu => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new DynamicMapMenuViewModel());
        });

        private MapViewPort GetStartingCoordinates()
        {
            var mapViewPort = new MapViewPort(new Geopoint(new BasicGeoposition { Latitude = 45.503343, Longitude = -73.571695 }));
            mapViewPort.ZoomLevel = ZoomLevels.District;
            return mapViewPort;
        }

        private IDisposable ObserveViewPort()
        {
            return this.GetProperty(x => x.ViewPort).GetAndObserve().Subscribe(UpdatePushpins);

            // When Viewport change load new set of Pushpin.
            void UpdatePushpins(MapViewPort viewPort){
                Pushpins = GetPushpins(ViewPortCoordinates);
            }
        }

        private PushpinEntity[] _allPushpins = new PushpinEntity[]
        {
                new PushpinEntity()
                {
                    Name = "Location 1",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.506238, Longitude = -73.576308 })
                },
                new PushpinEntity()
                {
                    Name = "Location 2",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.502042, Longitude = -73.574162 })
                },
                new PushpinEntity()
                {
                    Name = "Location 3",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.505832, Longitude = -73.565654})
                },
                new PushpinEntity()
                {
                    Name = "Location 4",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.504554, Longitude = -73.560611})
                },
                new PushpinEntity()
                {
                    Name = "Location 5",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.497981, Longitude = -73.556204})
                },
                new PushpinEntity()
                {
                    Name = "Location 6",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.492106, Longitude = -73.557889})
                },
                new PushpinEntity()
                {
                    Name = "Location 7",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.485773, Longitude = -73.558404})
                },
                new PushpinEntity()
                {
                    Name = "Location 8",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.479755, Longitude = -73.563404})
                },
                new PushpinEntity()
                {
                    Name = "Location 9",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.473842, Longitude = -73.569498})
                },
                new PushpinEntity()
                {
                    Name = "Location 10",
                    Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.469967, Longitude = -73.591009})
                }
        };

        private PushpinEntity[] GetPushpins(MapViewPortCoordinates boundaries)
        {
            return _allPushpins
                .Where(p => boundaries?.IsSurrounding(p.Coordinates) ?? false)
                .ToArray();
        }
    }
}
