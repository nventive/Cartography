#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using GeolocatorService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Uno.Extensions;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cartography.DynamicMap
{
	public partial class MapControlBase
	{
		private const string COMPASS_TAG = "GoogleMapCompass";

		private GoogleMapView _internalMapView;

		private Thickness _padding;
		private GoogleMapLayer _pushpins;
		private MapLifeCycleCallBacks _callbacks;
		private Android.App.Application _application;
		private BitmapDescriptor _icon;
		private BitmapDescriptor _selectedIcon;
		private MapReadyCallback _callback;
		private View _compass;

		/// <summary>
		/// Sets the selector that will be used to update the marker. This will get 
		/// called when the DataContext changes, the position changes and the selected 
		/// state changes for a marker.
		/// </summary>
		/// <remarks>
		/// The call chain for the update are the following:
		/// (UseIcons && PushpinIconsMarkerUpdater)? -> UpdateMarker -> MarkerUpdater?
		/// </remarks>
		public Action<Pushpin, Marker> MarkerUpdater { get; set; }

		/// <summary>
		/// Handles margin for the compass icon
		/// </summary>
		public Thickness CompassMargin
		{
			get
			{
				if (_compass == null)
				{
					_compass = _internalMapView.FindViewWithTag(COMPASS_TAG);
				}

				RelativeLayout.LayoutParams rlp = (RelativeLayout.LayoutParams)_compass.LayoutParameters;

				return new Thickness(rlp.LeftMargin, rlp.TopMargin, rlp.RightMargin, rlp.BottomMargin);
			}

			set
			{
				if (_compass == null)
				{
					_compass = _internalMapView.FindViewWithTag(COMPASS_TAG);
				}

				RelativeLayout.LayoutParams rlp = (RelativeLayout.LayoutParams)_compass.LayoutParameters;
				rlp.TopMargin = (int)value.Top;
				rlp.RightMargin = (int)value.Right;
				rlp.LeftMargin = (int)value.Left;
				rlp.BottomMargin = (int)value.Bottom;
			}
		}


		/// <summary>
		/// Enables multiple selected pins
		/// </summary>
		private bool AllowMultipleSelection { get { return SelectionMode == MapSelectionMode.Multiple; } }
		partial void PartialConstructor(ILogger<MapControlBase> logger = null)
		{
			Loaded += (sender, args) => OnLoaded();
			Unloaded += (sender, args) => OnUnloaded();

			_internalMapView = new GoogleMapView(Android.App.Application.Context, new GoogleMapOptions());

			Template = new ControlTemplate(() => _internalMapView);//TODO support templateing

			MapsInitializer.Initialize(Android.App.Application.Context);

			_internalMapView.GetMapAsync(_callback = new MapReadyCallback(OnMapReady));

			_internalMapView.OnCreate(null); // This otherwise the map does not appear

			_logger = logger ?? NullLogger<MapControlBase>.Instance;

		}

		private void OnLoaded()
		{
			_internalMapView.OnResume(); // This otherwise the map stay empty

			HandleActivityLifeCycle();

			_internalMapView.TouchOccurred += MapTouchOccurred;
		}

		private void OnUnloaded()
		{
			// These line is required for the control to 
			// stop actively monitoring the user's location.
			_internalMapView.OnPause();

			_application.UnregisterActivityLifecycleCallbacks(_callbacks);

			if (_internalMapView != null)
			{
				_internalMapView.TouchOccurred -= MapTouchOccurred;
			}
		}

		private void MapTouchOccurred(object sender, MotionEvent e)
		{
			_isUserDragging.OnNext(e.Action == MotionEventActions.Move);
		}

		private GoogleMap _map;
		private void OnMapReady(GoogleMap map)
		{
			_map = map;

			_padding = Thickness.Empty;
			_pushpins = new GoogleMapLayer(map);
			_isReady = true;

			map.MarkerClick += Map_MarkerClick;
			map.MapClick += Map_MapClick;

			UpdateMapPushpinOnCameraIdle();

			UpdateAutolocateButtonVisibility(AutolocateButtonVisibility);
			UpdateCompassButtonVisibility(CompassButtonVisibility);
			UpdateIsRotateGestureEnabled(IsRotateGestureEnabled);
			UpdateMapStyleJson(MapStyleJson);

			UpdateIcon(PushpinIcon);
			UpdateSelectedIcon(SelectedPushpinIcon);

			TryStart();
		}

		/// <summary>
		/// Idea is to register to the LifeCycleCallbacks and properly call the OnResume and OnPause methods when needed.
		/// This will release the GPS while the application is in the background
		/// </summary>
		private void HandleActivityLifeCycle()
		{
			_callbacks = new MapLifeCycleCallBacks(onPause: _internalMapView.OnPause, onResume: _internalMapView.OnResume);

			_application = Context.ApplicationContext as Android.App.Application;
			if (_application != null)
			{
				_application.RegisterActivityLifecycleCallbacks(_callbacks);
			}
			else
			{
				_logger.LogInformation("ApplicationContext is invalid, could not RegisterActivityLifecycleCallbacks to release GPS when application is paused.");
			}
		}

#region UserLocation
		private void UpdateMapUserLocation(LocationResult locationAndStatus)
		{
			if (locationAndStatus != null)
				_map.MyLocationEnabled = locationAndStatus.IsSuccessful;
		}
#endregion

#region ViewPort

		private IEnumerable<IObservable<Unit>> GetViewPortChangedTriggers()
		{
			var map = _map;

			yield return System.Reactive.Linq.Observable
				.FromEventPattern<GoogleMap.CameraChangeEventArgs>(
					h => map.CameraChange += h,
					h => map.CameraChange -= h)
				.Where(_ => !_isAnimating)
				.Select(_ => Unit.Default);
		}

		private MapViewPort GetViewPort()
		{
			var position = _map.CameraPosition;
			var point = new BasicGeoposition();
			point.Longitude = position.Target.Longitude;
			point.Latitude = position.Target.Latitude;

			return new MapViewPort
			{
				Center = new Geopoint(point),
				Heading = position.Bearing,
				Pitch = position.Tilt,
				ZoomLevel = (ZoomLevel)position.Zoom,
			};
		}

		private bool GetInitializationStatus() => true;

		private async Task SetViewPort(CancellationToken ct, MapViewPort viewPort)
		{
			await _viewLayedOut.Task;

			var animation = new MapCancellableCallback(ct);

			if (viewPort.PointsOfInterest.Safe().Any())
			{
				//Rubber-band bounds to PointsOfInterest
				var bounds = viewPort
					.PointsOfInterest
					.Aggregate(
						new LatLngBounds.Builder(),
						(builder, poi) => builder.Include(new LatLng(poi.Position.Latitude, poi.Position.Longitude)))
					.Build();

				if (viewPort.Center != default(Geopoint))
				{
					bounds = AddPushpinPaddingToBounds(viewPort);
				}

				var cameraBounds = CameraUpdateFactory.NewLatLngBounds(bounds, 0);

				if (viewPort.IsAnimationDisabled)
				{
					_map.MoveCamera(cameraBounds);
				}
				else
				{
					_map.AnimateCamera(cameraBounds, animation);
				}
			}
			else
			{
				var builder = new CameraPosition.Builder(_map.CameraPosition)
					.Target(new LatLng(viewPort.Center.Position.Latitude, viewPort.Center.Position.Longitude));

				if (viewPort.Heading.HasValue)
				{
					builder.Bearing((float)viewPort.Heading.Value);
				}

				if (viewPort.Pitch.HasValue)
				{
					builder.Tilt((float)viewPort.Pitch.Value);
				}

				if (viewPort.ZoomLevel.HasValue)
				{
					builder.Zoom((float)viewPort.ZoomLevel);
				}

				var cameraUpdate = CameraUpdateFactory.NewCameraPosition(builder.Build());

				if (viewPort.IsAnimationDisabled)
				{
					_map.MoveCamera(cameraUpdate);
				}
				else
				{
					_map.AnimateCamera(cameraUpdate, animation);
				}
			}

			await animation;
		}

		private LatLngBounds AddPushpinPaddingToBounds(MapViewPort viewPort)
		{
			var frontiers = viewPort.GetBounds();

			// create ViewPort with calculated dimensions
			var northEastCorner = new LatLng(frontiers.EastFrontier, frontiers.NorthFrontier);
			var southWestCorner = new LatLng(frontiers.WestFrontier, frontiers.SouthFrontier);

			return new LatLngBounds(southWestCorner, northEastCorner);
		}
#endregion

#region ViewPortCoordinates
		private MapViewPortCoordinates GetViewPortCoordinates()
		{
			var visibleRegion = _map.Projection.VisibleRegion;
			return new MapViewPortCoordinates(
				northWest: new BasicGeoposition { Latitude = visibleRegion.FarLeft.Latitude, Longitude = visibleRegion.FarLeft.Longitude },
				northEast: new BasicGeoposition { Latitude = visibleRegion.FarRight.Latitude, Longitude = visibleRegion.FarRight.Longitude },
				southWest: new BasicGeoposition { Latitude = visibleRegion.NearLeft.Latitude, Longitude = visibleRegion.NearLeft.Longitude },
				southEast: new BasicGeoposition { Latitude = visibleRegion.NearRight.Latitude, Longitude = visibleRegion.NearRight.Longitude }
			);
		}
#endregion

#region Pushpins
		private void UpdateMapPushpins(IGeoLocated[] items, IGeoLocated[] selectedItems)
		{
			_pushpins.Update(
				items: items,
				selectedItems: selectedItems,
				containerFactory: _ => new Pushpin
				{
					Map = this,
					// call chain: PushpinIconsMarkerUpdater? -> UpdateMarker -> MarkerUpdater?
					MarkerUpdater = UseIcons
						? PushpinIconsMarkerUpdater
						: (Action<Pushpin, Marker>)UpdateMarker
				},

				// Pin instances cannot be recycled in Google Maps.
				canRecycle: false
			);
		}

		private void UpdateMarker(Pushpin pushpin, Marker marker)
		{
			// update z-index
			marker.ZIndex = pushpin.ZIndex;

			// call injected updater
			MarkerUpdater?.Invoke(pushpin, marker);
		}
#endregion

#region Pushpin ICONS
		private bool UseIcons => _icon != null;

		private void UpdateIcon(object icon)
		{
			if (_icon != null)
			{
				throw new InvalidOperationException("Pushpins icons cannot be changed.");
			}

			UpdateIcon(ref _icon, icon);
		}

		private void UpdateSelectedIcon(object icon)
		{
			if (_selectedIcon != null)
			{
				throw new InvalidOperationException("Pushpins icons cannot be changed.");
			}

			UpdateIcon(ref _selectedIcon, icon);
		}

		private void UpdateIcon(ref BitmapDescriptor icon, object value)
		{
			if (!_isReady)
			{
				// Deferring the update, the map control is not available yet.
				return;
			}

			if (value == null)
			{
				icon = null;
				return;
			}

			icon = ToImageSource(value);
			if (icon == null)
			{
				throw new InvalidOperationException("Failed to convert '{0}' to a PushpinIcon".InvariantCultureFormat(value));
			}
		}

		private static BitmapDescriptor ToImageSource(object value)
		{
			var uriStr = value as string;
			if (uriStr.HasValueTrimmed()
				&& Uri.IsWellFormedUriString(uriStr, UriKind.RelativeOrAbsolute))
			{
				value = new Uri(uriStr, UriKind.RelativeOrAbsolute);
			}

			var uri = value as Uri;
			if (uri != null)
			{
				if (!uri.IsAbsoluteUri)
				{
					//return BitmapDescriptorFactory.FromAsset(uri.OriginalString);
					return BitmapDescriptorFactory_FromAsset(uri.OriginalString);
				}

				switch (uri.Scheme.ToUpperInvariant())
				{
					case "RES":
					case "RESOURCE":
						return BitmapDescriptorFactory.FromResource(int.Parse(uri.LocalPath.Trim(new[] { '/' })));

					case "FILE":
						return BitmapDescriptorFactory.FromFile(uri.LocalPath);

					case "ASSET":
						//return BitmapDescriptorFactory.FromAsset(uri.LocalPath);
						return BitmapDescriptorFactory_FromAsset(uri.LocalPath);

					case "HTTP":
					default:
						throw new NotSupportedException("Scheme '{0}' not supported as source of a pushpin icon".InvariantCultureFormat(uri.Scheme));
				}
			}

			var bitmap = value as Bitmap;
			if (bitmap != null)
			{
				return BitmapDescriptorFactory.FromBitmap(bitmap);
			}

			return null;
		}

		private static BitmapDescriptor BitmapDescriptorFactory_FromAsset(string assetName)
		{
			// A known bug with the Google Play services (fixed in 7.3) is that we cannot use assets as icon of pushpins
			// cf. https://code.google.com/p/gmaps-api-issues/issues/detail?id=7696

			// As v7.3 binding is not yet released (only private RC for now), we cannot update, so we will use work arround 
			// proposed in message #21 of link upper.

			var assetManager = Android.App.Application.Context.Assets;

			try
			{
				var inputStream = assetManager.Open(assetName.TrimStart("Assets", StringComparison.OrdinalIgnoreCase).TrimStart('/', '\\'));
				var image = BitmapFactory.DecodeStream(inputStream);

				return BitmapDescriptorFactory.FromBitmap(image);
			}
			catch
			{
				return null;
			}
		}

		private void PushpinIconsMarkerUpdater(Pushpin pushpin, Marker marker)
		{
			var icon = pushpin.IsSelected
				? _selectedIcon
				: _icon;

			marker.SetIcon(icon ?? _icon ?? BitmapDescriptorFactory.DefaultMarker());

			UpdateMarker(pushpin, marker);
		}
#endregion

#region Selected pushpins
		private void Map_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
		{
			_logger.LogDebug("Clicking on a pin.");

			var pushPin = _pushpins.FindPushPin(e.Marker);

			if (pushPin != null)
			{
				pushPin.IsSelected = !pushPin.IsSelected;

				if (!AllowMultipleSelection)
				{
					_pushpins
						.Items
						.Where(i => i != pushPin)
						.ForEach(p => p.IsSelected = false);
				}

				var selectedContent = GetSelectedAnnotationsContent();

				_logger.LogInformation($"Clicked on '{selectedContent.Length}' pins.");

				_selectedPushpins.OnNext(selectedContent);
			}
		}

		void Map_MapClick(object sender, GoogleMap.MapClickEventArgs e)
		{
			_logger.LogDebug("Clicking on the map.");

			if (!AllowMultipleSelection)
			{
				_pushpins
					.Items
					.ForEach(p => p.IsSelected = false);

				_selectedPushpins.OnNext(new IGeoLocated[0]);
			}

			OnMapTapped(new Geocoordinate(e.Point.Latitude, e.Point.Longitude, 0, DateTime.Now, null, null, null, null, null, default));

			_logger.LogInformation("Clicked on the map.");
		}


		private IGeoLocated[] GetSelectedAnnotationsContent()
		{
			return _pushpins
				.Items
				.Where(p => p.IsSelected)
				.Select(p => p.Content)
				.ToArray();
		}

		private void UpdateMapSelectedPushpins(IGeoLocated[] newlySelected)
		{
			_pushpins.UpdateSelection(newlySelected);
		}
#endregion

		private void UpdateMapPushpinOnCameraIdle()
		{
			_map.SetOnCameraIdleListener(new MapOnCameraIdleListener(this));
		}

		private class MapOnCameraIdleListener : Java.Lang.Object, GoogleMap.IOnCameraIdleListener
		{
			private readonly MapControlBase _parent;

			public MapOnCameraIdleListener(MapControlBase parent)
			{
				_parent = parent;
			}

			public void OnCameraIdle()
			{
				var selectedContent = _parent.GetSelectedAnnotationsContent();
				_parent._selectedPushpins.OnNext(selectedContent);
			}
		}

		private class MapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
		{
			private readonly Action<GoogleMap> _mapAvailable;

			public MapReadyCallback(Action<GoogleMap> mapAvailable)
			{
				_mapAvailable = mapAvailable;
			}

			public void OnMapReady(GoogleMap googleMap)
			{
				_mapAvailable(googleMap);
			}
		}

		private class MapCancellableCallback : Java.Lang.Object, GoogleMap.ICancelableCallback
		{
			private readonly TaskCompletionSource<Unit> _source = new TaskCompletionSource<Unit>();
			private readonly IDisposable _cancelSubscription;

			public MapCancellableCallback(CancellationToken ct)
			{
				_cancelSubscription = ct.Register(() => _source.TrySetCanceled());
			}

			void GoogleMap.ICancelableCallback.OnCancel()
			{
				_source.TrySetCanceled();
			}

			void GoogleMap.ICancelableCallback.OnFinish()
			{
				_source.TrySetResult(Unit.Default);
			}

			public TaskAwaiter<Unit> GetAwaiter()
			{
				return _source.Task.GetAwaiter();
			}

			protected override void Dispose(bool disposing)
			{
				_cancelSubscription.Dispose();

				base.Dispose(disposing);
			}
		}

		private class MapLifeCycleCallBacks : Java.Lang.Object, global::Android.App.Application.IActivityLifecycleCallbacks
		{
			private readonly Action _onPause;
			private readonly Action _onResume;

			public MapLifeCycleCallBacks(Action onPause, Action onResume)
			{
				_onResume = onResume;
				_onPause = onPause;
			}

			public void OnActivityResumed(Activity activity)
			{
				_onResume();
			}

			public void OnActivityPaused(Activity activity)
			{
				_onPause();
			}

#region Not implemented

			public void OnActivityCreated(Activity activity, global::Android.OS.Bundle savedInstanceState)
			{
			}

			public void OnActivityDestroyed(Activity activity)
			{
			}

			public void OnActivitySaveInstanceState(Activity activity, global::Android.OS.Bundle outState)
			{
			}

			public void OnActivityStarted(Activity activity)
			{
			}

			public void OnActivityStopped(Activity activity)
			{
			}

#endregion
		}


		TaskCompletionSource<bool> _viewLayedOut = new TaskCompletionSource<bool>();

		protected override void OnLayoutCore(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayoutCore(changed, left, top, right, bottom);

			_viewLayedOut.TrySetResult(true);
		}

		partial void UpdateAutolocateButtonVisibility(Visibility visibility)
		{
			_logger.LogDebug("Updating the autolocate button's visibility.");

			if (_map != null)
			{
				_map.UiSettings.MyLocationButtonEnabled = visibility == Visibility.Visible;

				_logger.LogInformation("Updated the autolocate button's visibility.");
			}
			else
			{
				_logger.LogError("Could not update the autolocate button's visibility .");
			}
		}

		partial void UpdateCompassButtonVisibility(Visibility visibility)
		{
			_logger.LogDebug("Updating the compass button's visibility.");

			if (_map != null)
			{
				_map.UiSettings.CompassEnabled = visibility == Visibility.Visible;

				_logger.LogInformation("Updated the autolocate button's visibility.");
			}
			else
			{
				_logger.LogError("Could not update the compass button's visibility.");
			}
		}

		partial void UpdateIsRotateGestureEnabled(bool isRotateGestureEnabled)
		{
			_logger.LogDebug($"{(isRotateGestureEnabled ? "Enabling" : "Disabling")} the gesture rotation.");

			if (_map != null)
			{
				_map.UiSettings.RotateGesturesEnabled = isRotateGestureEnabled;

				_logger.LogDebug($"{(isRotateGestureEnabled ? "Enabled" : "Disabled")} the gesture rotation.");
			}
			else
			{
				_logger.LogError($" Could not {(isRotateGestureEnabled ? "enable" : "disable")} the gesture rotation.");
			}
		}

		partial void UpdateMapStyleJson(string mapStyleJson)
		{
			if (_map != null)
			{
				var newStyle = mapStyleJson.HasValueTrimmed() ? mapStyleJson : "[]";

				_map.SetMapStyle(new MapStyleOptions(newStyle));
			}
		}
	}
}
#endif
