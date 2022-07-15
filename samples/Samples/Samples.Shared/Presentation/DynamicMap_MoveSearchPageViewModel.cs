using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Cartography.Core;
using Cartography.DynamicMap;
using Chinook.DynamicMvvm;
using GeolocatorService;
using Samples.Entities;
using Uno;
using Windows.Devices.Geolocation;

namespace Umbrella.Location.Samples.Uno
{
	public class DynamicMap_MoveSearchPageViewModel /*: ViewModelBase, IMapComponent*/
    {

        //private IGeolocatorService _geolocatorService;

        //private MapViewPort mapViewPort;
        //private Geopoint currentCoordinate;
        //private MapViewPortCoordinates mapViewPortCoordinates;
        //private ZoomLevel zoomLevelTreshhold;
        //private bool isTooFar;

        //private IGeoLocated[] pushpins;
        //private IGeoLocated[] selectedPushpins;
        //private IGeoLocatedGrouping<IGeoLocated[]> groups;
        //private TimeSpan viewPortUpdateMinDelay;
        //private IEqualityComparer<MapViewPort> viewPortUpdateFilter;
        //private ActionAsync<Geocoordinate> onMapTapped;
        //private bool isUserTrackingCurrentlyEnabled;
        //private bool isUserDragging;
        //private LocationResult userLocation;
        //private MapViewPortCoordinates viewPortCoordinates;
        //private bool skipAnimations;
        //private MapViewPort viewPort;
        //private int? animationDurationSecond;


        //public DynamicMap_MoveSearchPageViewModel()
        //{
        //    mapViewPort = new MapViewPort(GetStartingCoordinates());
        //    MapViewPortCoordinates = mapViewPortCoordinates;
        //    ZoomLevelTreshhold = ZoomLevels.Region;
        //    pushpins = (IGeoLocated[])GetPushpins(MapViewPort, MapViewPortCoordinates);
        //    AddDisposable(SubscribeOnRefreshIsTooFar());
        //}

        //private MapViewPort MapViewPort 
        //{ 
        //    get { return mapViewPort; } 
        //    set { mapViewPort = value; } 
        //}
        //private Geopoint CurrentCoordinate
        //{
        //    get { return currentCoordinate; }
        //    set { currentCoordinate = value; }
        //}
        //private MapViewPortCoordinates MapViewPortCoordinates 
        //{ 
        //    get { return mapViewPortCoordinates; } 
        //    set { mapViewPortCoordinates = value; } 
        //}
        //private bool IsTooFar
        //{
        //    get { return isTooFar; }
        //    set { isTooFar = value; }
        //}
        //private ZoomLevel ZoomLevelTreshhold
        //{
        //    get { return zoomLevelTreshhold; }
        //    set { zoomLevelTreshhold = value; }
        //}

        //public IGeoLocated[] Pushpins
        //{
        //    get { return pushpins; }
        //    set { pushpins = value; }
        //}
        //public IGeoLocated[] SelectedPushpins
        //{
        //    get { return selectedPushpins; }
        //    set { selectedPushpins = value; }
        //}
        //public IGeoLocatedGrouping<IGeoLocated[]> Groups
        //{
        //    get { return groups; }
        //    set { groups = value; }
        //}
        //public TimeSpan ViewPortUpdateMinDelay
        //{
        //    get { return viewPortUpdateMinDelay; }
        //    set { viewPortUpdateMinDelay = value; }
        //}
        //public IEqualityComparer<MapViewPort> ViewPortUpdateFilter
        //{
        //    get
        //    { return viewPortUpdateFilter; }
        //    set { viewPortUpdateFilter = value; }
        //}
        //public ActionAsync<Geocoordinate> OnMapTapped
        //{
        //    get { return onMapTapped; }
        //    set { onMapTapped = value; }
        //}
        //public bool IsUserTrackingCurrentlyEnabled
        //{
        //    get { return isUserTrackingCurrentlyEnabled; }
        //    set { isUserTrackingCurrentlyEnabled = value; }
        //}
        //public bool IsUserDragging
        //{
        //    get { return isUserDragging; }
        //    set { isUserDragging = value; }
        //}
        //public LocationResult UserLocation
        //{
        //    get { return userLocation; }
        //    set { userLocation = value; }
        //}
        //public MapViewPortCoordinates ViewPortCoordinates
        //{
        //    get { return viewPortCoordinates; }
        //    set { viewPortCoordinates = value; }
        //}
        //public bool SkipAnimations
        //{
        //    get { return skipAnimations; }
        //    set { skipAnimations = value; }
        //}
        //public MapViewPort ViewPort
        //{
        //    get { return viewPort; }
        //    set { viewPort = value; }
        //}
        //public int? AnimationDurationSeconds
        //{
        //    get { return animationDurationSecond; }
        //    set { animationDurationSecond = value; }
        //}


        //private void CheckMapViewPortIsTooFar()
        //{
        //    isTooFar = MapViewPort.ZoomLevel <= ZoomLevelTreshhold;
        //}

        //private IDisposable SubscribeOnRefreshIsTooFar()
        //{
        //    return this.GetProperty(x => x.IsTooFar).GetAndObserve().Subscribe();
        //}

        //private Geopoint GetStartingCoordinates()
        //{
        //    return new Geopoint(new BasicGeoposition { Latitude = 45.503343, Longitude = -73.571695 });
        //}

        //private PushpinEntity[] _allPushpins = new PushpinEntity[]
        //{
        //        new PushpinEntity()
        //        {
        //            Name = "Location 1",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.506238, Longitude = -73.576308 })
        //        },
        //        new PushpinEntity()
        //        {
        //            Name = "Location 2",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.502042, Longitude = -73.574162 })
        //        },
        //        new PushpinEntity()
        //        {
        //            Name = "Location 3",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.505832, Longitude = -73.565654})
        //        },
        //        new PushpinEntity()
        //        {
        //            Name = "Location 4",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.504554, Longitude = -73.560611})
        //        },
        //        new PushpinEntity()
        //        {
        //            Name = "Location 5",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.497981, Longitude = -73.556204})
        //        },
        //        new PushpinEntity()
        //        {
        //            Name = "Location 6",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.492106, Longitude = -73.557889})
        //        },
        //        new PushpinEntity()
        //        {
        //            Name = "Location 7",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.485773, Longitude = -73.558404})
        //        },
        //        new PushpinEntity()
        //        {
        //            Name = "Location 8",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.479755, Longitude = -73.563404})
        //        },
        //        new PushpinEntity()
        //        {
        //            Name = "Location 9",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.473842, Longitude = -73.569498})
        //        },
        //        new PushpinEntity()
        //        {
        //            Name = "Location 10",
        //            Coordinates = new Geopoint(new BasicGeoposition{Latitude = 45.469967, Longitude = -73.591009})
        //        }
        //};

        //private PushpinEntity[] GetPushpins(MapViewPort mapViewPort, MapViewPortCoordinates boundaries)
        //{
        //    return _allPushpins
        //        .Where(p => boundaries?.IsSurrounding(new Geocoordinate(p.Coordinates.Position.Latitude, p.Coordinates.Position.Longitude, 0, new DateTimeOffset(), p.Coordinates)) ?? false)
        //        .ToArray();
        //}
    }
}
