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

namespace Samples.ViewModel
{
	public class DynamicMap_FeaturesPageViewModel : ViewModelBase, IMapComponent
	{
		private IDynamicPropertyFactory _dynamicPropertyFactory = new DynamicPropertyFactory();

		private bool isLocationEnabled;
		private bool isTooFar;
		private IGeolocatorService _geolocatorService;
		private IGeoLocated[] pushpins;
		private IGeoLocated[] selectedPushpins;
		private IGeoLocatedGrouping<IGeoLocated[]> groups;
		private TimeSpan viewPortUpdateMinDelay;
		private IEqualityComparer<MapViewPort> viewPortUpdateFilter;
		private ActionAsync<Geocoordinate> onMapTapped;
		private bool isUserTrackingCurrentlyEnabled;
		private bool isUserDragging;
		private LocationResult userLocation;
		private MapViewPortCoordinates viewPortCoordinates;
		private bool skipAnimations;
		private MapViewPort viewPort;
		private int? animationDurationSecond;
		
		public DynamicMap_FeaturesPageViewModel()
		{
			ViewPort = new MapViewPort( GetStartingCoordinates());
            Pushpins = GetInitialPushpins();
			IsTooFar = false;
			_geolocatorService = new GeolocatorService.GeolocatorService();
			OnLoaded();

			AddDisposable(ObservePushpins());
			if (IsLocationEnabled)
			{
				AddDisposable(ObserveUserLocation());
			}
			AddDisposable(ObserveSelectedPushpin());
		}

		async private void OnLoaded()
		{
			IsLocationEnabled = await _geolocatorService.GetIsPermissionGranted(CancellationToken.None);
		}

		public bool IsLocationEnabled 
		{ 
			get { return isLocationEnabled; } 
			set { isLocationEnabled = value; } 
		}

		public bool IsTooFar 
		{ 
			get { return isTooFar; } 
			set { isTooFar = value; } 
		}

		public IGeoLocated[] Pushpins 
		{
			get => this.Get(initialValue: GetInitialPushpins());
			set => this.Set(value);
		}
        public IGeoLocated[] SelectedPushpins 
		{ 
			get { return selectedPushpins; } 
			set { selectedPushpins = value; } 
		}
        public IGeoLocatedGrouping<IGeoLocated[]> Groups 
		{ 
			get { return groups; } 
			set { groups = value; } 
		}
        public TimeSpan ViewPortUpdateMinDelay 
		{ 
			get { return viewPortUpdateMinDelay; } 
			set { viewPortUpdateMinDelay = value; }
		}
        public IEqualityComparer<MapViewPort> ViewPortUpdateFilter 
		{ get 
			{ return viewPortUpdateFilter; } 
			set { viewPortUpdateFilter = value; } 
		}
        public ActionAsync<Geocoordinate> OnMapTapped 
		{ 
			get { return onMapTapped; } 
			set {onMapTapped = value; }
		}
        public bool IsUserTrackingCurrentlyEnabled 
		{
			get { return isUserTrackingCurrentlyEnabled; }
			set { isUserTrackingCurrentlyEnabled = value; } 
		}
        public bool IsUserDragging
		{
			get { return isUserDragging; }
			set { isUserDragging = value; }
		}
        public LocationResult UserLocation 
		{ 
			get { return userLocation; } 
			set { userLocation = value; } 
		}
        public MapViewPortCoordinates ViewPortCoordinates 
		{ 
			get { return viewPortCoordinates; } 
			set { viewPortCoordinates = value; } 
		}
        public bool SkipAnimations 
		{ 
			get { return skipAnimations; } 
			set { skipAnimations = value; } 
		}
        public MapViewPort ViewPort 
		{ 
			get { return viewPort; } 
			set { viewPort = value; } }
        public int? AnimationDurationSeconds
		{
			get { return animationDurationSecond; }
			set { animationDurationSecond = value; }
		}

		private IDisposable ObservePushpins()
		{
			return this.GetProperty(x => x.Pushpins).GetAndObserve().Subscribe();
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

		private IDisposable ObserveSelectedPushpin()
		{
			return this.GetProperty(x => x.SelectedPushpins).GetAndObserve().Subscribe();
		}

		private Geopoint GetStartingCoordinates()
		{
			return new Geopoint(new BasicGeoposition { Latitude = 45.5016889, Longitude = -73.56725599999999 });
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

		public IDynamicCommand AddPushpin => this.GetCommandFromTask(async ct =>
		{
			var pushpins = Pushpins;

			var newPushpin = await CreatePushpinAtCenter(ct);

			var list = pushpins.ToList();
			list.Add((IGeoLocated)newPushpin);

			Pushpins = list.ToArray();
		});

		private async Task<PushpinEntity> CreatePushpinAtCenter(CancellationToken ct)
		{
			var viewport = await this.GetProperty(x => ViewPort).GetAndObserve().FirstAsync();
			var latitude = (double) viewport.Center.Position.Latitude;
			var longitude = (double) viewport.Center.Position.Longitude;

			return new PushpinEntity()
			{
				Coordinates = new Geopoint(new BasicGeoposition { Latitude = latitude, Longitude = longitude }),
				Name = string.Empty
			};
		}

		public IDynamicCommand RemoveSelectedPushpin => this.GetCommandFromTask(async ct =>
		{
			ClearSelectedPushpin(ct);
		});

        private void ClearSelectedPushpin(CancellationToken ct)
        {
            SelectedPushpins = null;
        }

  //      private async Task OnError(CancellationToken ct)
		//{
		//	IsLocateMeOnError.Value.OnNext(true);
		//}

		//private async Task UpdateViewPort(CancellationToken ct)
		//{
		//	var latitudeString = await ViewPortLatitude;
		//	var longitudeString = await ViewPortLongitude;
		//	if (double.TryParse(latitudeString, out var latitude)
		//		&& double.TryParse(longitudeString, out var longitude)
		//		&& latitude >= -90
		//		&& latitude <= 90
		//		&& longitude >= -180
		//		&& longitude <= 180)
		//	{
		//		var viewport = await MapViewPort;
		//		viewport.Center.Latitude = latitude;
		//		viewport.Center.Longitude = longitude;
		//		MapViewPort.Value.OnNext(viewport);
		//	}
		//	else
		//	{
		//		throw new InvalidOperationException("Both latitude and longitude must be valid");
		//	}
		//}

		//private async Task CenterOnPOI(CancellationToken ct)
		//{
		//	MapViewPort.Value.OnNext(new Core.MapViewPort(new Coordinate { Latitude = 45.582, Longitude = -73.749 }) { ZoomLevel = new ZoomLevel(16.0441079686075) });
		//}
	}
}
