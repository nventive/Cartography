﻿using Cartography.DynamicMap;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using GeolocatorService;
using Sample.Entities;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System;
using Uno.Extensions;

using Geopoint = Cartography.DynamicMap.Geopoint;
using BasicGeoposition = Cartography.DynamicMap.BasicGeoposition;
using Geocoordinate = Cartography.DynamicMap.Geocoordinate;


namespace Sample.Presentation;

public class GoogleMapsControl_FeaturesPageViewModel : ViewModel, IDynamicMapComponent
{
	private IGeolocatorService _geolocatorService;
	private ISectionsNavigator _sectionsNavigator;
	private readonly IDispatcherScheduler _dispatcherScheduler;

	public GoogleMapsControl_FeaturesPageViewModel()
	{
		_geolocatorService = this.GetService<IGeolocatorService>();
		_sectionsNavigator = this.GetService<ISectionsNavigator>();
		_dispatcherScheduler = this.GetService<IDispatcherScheduler>();
		OnLoaded();
	}

	private void OnLoaded()
	{
		// check if permission is granted.
		_dispatcherScheduler.ScheduleTask(async (ct2, dispatcher) =>
		{
			IsLocationEnabled = await _geolocatorService.GetIsPermissionGranted(ct2);
			if (!IsLocationEnabled)
			{
				// Ask for Permission
				IsLocationEnabled = await _geolocatorService.RequestPermission(ct2);
			}
			BuildMap();
		});
	}

	private void BuildMap()
	{
		AddDisposable(ObserveViewPort());
		if (IsLocationEnabled)
		{
			// Track Userlocation if permission is granted.
			AddDisposable(ObserveUserLocation());
		}
		AddDisposable(ObserveSelectedPushpin());

		Pushpins = GetInitialPushpins();
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

	public GeolocatorService.LocationResult UserLocation
	{
		get => this.Get<GeolocatorService.LocationResult>();
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

	#region Disposable
	// Observe userlocation change.
	private IDisposable ObserveUserLocation()
	{
		var obs = Observable.CombineLatest(
			GeolocatorServiceExtensions.GetAndObserveLocationOrDefault(_geolocatorService),
			this.GetProperty(x => IsLocationEnabled).GetAndObserve(),
			(location, isEnabled) => (location, isEnabled));

		async void UpdateLocationState((GeolocatorService.Geocoordinate location, bool isLocationEnabled) result)
		{
			if (result.isLocationEnabled)
			{
				UserLocation = new LocationResult(true, result.location);
				// Center ViewPort on User.
				if (IsMapFollowTheUser)
				{
					var mapCenter = new Geopoint(new BasicGeoposition { Latitude = result.location.Point.Position.Latitude, Longitude = result.location.Point.Position.Longitude });
					var mapViewPort = await ComputeMapViewPort(CancellationToken.None, mapCenter);
					ViewPort = mapViewPort;
				}
			}
		}

		return obs.Subscribe(UpdateLocationState);
	}

	//Observe any change on the selectedPushpin.
	private IDisposable ObserveSelectedPushpin()
	{
		return this.GetProperty(x => x.SelectedPushpins).GetAndObserve().Subscribe(UpdateSelectedPushpin);

		void UpdateSelectedPushpin(IGeoLocated[] selectedPushpins)
		{
			// Here we only change text output. PushpinEntity generated a string.
			if (selectedPushpins != null && selectedPushpins.Length > 0)
			{
				SelectedPushpin = new PushpinEntity
				{
					Name = "Selected Pushpin : ",
					Coordinates = selectedPushpins[0].Coordinates
				};
			}
			else
			{
				SelectedPushpin = new PushpinEntity
				{
					Name = "No Pushpin Selected : ",
					Coordinates = new Geopoint(new BasicGeoposition { Latitude = 0, Longitude = 0 })
				};
			}
		}
	}

	// Observe Change on Viewport.
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
	#endregion


	private MapViewPort GetStartingCoordinates()
	{
		var mapViewPort = new MapViewPort(new Geopoint(new BasicGeoposition { Latitude = 45.504071, Longitude = -73.558709 }));
		mapViewPort.ZoomLevel = ZoomLevels.District;
		mapViewPort.IsAnimationDisabled = IsViewPortAnimationDisabled;
		return mapViewPort;
	}

	private PushpinEntity[] GetInitialPushpins()
	{
		return
		[
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
				];
	}

	public IDynamicCommand LocateMe => this.GetCommandFromTask(async ct =>
	{
		if (IsLocationEnabled)
		{
			try
			{
				var currentLocation = (await _geolocatorService.GetLocation(ct)).Point;

				var userLocation = new Geopoint(new BasicGeoposition { Latitude = currentLocation.Position.Latitude, Longitude = currentLocation.Position.Longitude });
				await OnLocateMeSuccess(ct, userLocation);
				IsLocationEnabled = true;
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
		viewPort.IsAnimationDisabled = IsViewPortAnimationDisabled;

		return viewPort;
	}

	private async Task<GeoViewPort> GetUserCoordinates(CancellationToken ct)
	{
		var defaultGeoPoint = new Geopoint(new BasicGeoposition { Latitude = 45.504071, Longitude = -73.558709 });
		try
		{
			// We need to convert from GeolocatorService types to Cartography types because GeoViewPort wants a Cartography type and they are not interchangeable.
			var locationFromGeolocator = await _geolocatorService.GetLocation(ct);
			var geoPointFromCartography = new Geopoint(new BasicGeoposition { Latitude = locationFromGeolocator.Point.Position.Latitude, Longitude = locationFromGeolocator.Point.Position.Longitude });

			return new GeoViewPort(geoPointFromCartography ?? defaultGeoPoint, ZoomLevels.District);
		}
		catch
		{
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

	public IDynamicCommand RemoveSelectedPushpin => this.GetCommand(() =>
	{
		PushpinEntity selectedPushpin = SelectedPushpin;
		IGeoLocated[] pushpins = Pushpins;

		List<IGeoLocated> pushpinslist = pushpins.ToList();
		pushpinslist.RemoveAll(pin => pin.Coordinates == selectedPushpin.Coordinates);

		Pushpins = pushpinslist.ToArray();
		ClearSelectedPushpin();
	});

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
			ViewPort.IsAnimationDisabled = IsViewPortAnimationDisabled;
		}
		else
		{
			throw new InvalidOperationException("Both latitude and longitude must be valid");
		}
	});
}
