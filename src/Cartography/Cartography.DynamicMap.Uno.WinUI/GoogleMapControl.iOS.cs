#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using CoreLocation;
using GeolocatorService;
using Google.Maps;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UIKit;
using Uno.Logging;
using Windows.Devices.Geolocation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Cartography.DynamicMap
{
	/// <summary>
	/// iOS-only MapControl which uses Google Maps.
	/// </summary>
	public partial class GoogleMapControl : MapControlBase
	{
		private readonly ILogger<GoogleMapControl> _logger;
		private MapView _internalMapView;
		private IMapLayer<GooglePushpin> _pushpinsLayer;
		private readonly List<Overlay> _polygons = new List<Overlay>();

		private CameraPosition Position => _internalMapView?.Camera;
		private bool UseIcons => _icon != null;

		private bool _isViewPortInitialized;

		public GoogleMapControl(ILogger<GoogleMapControl> logger = null)
		{

			_internalMapView = new MapView();
			AddDragGestureRecognizer(_internalMapView);

			Template = new ControlTemplate(() => _internalMapView);//TODO use templates

			_pushpinsLayer = new GoogleMapLayer(_internalMapView);

			_internalMapView.TappedMarker = TappedMarker;
			_internalMapView.CoordinateTapped += OnMapViewCoordinateTapped;
			_logger = logger ?? NullLogger<GoogleMapControl>.Instance;
		}

		/// <summary>
		/// Sets the selector that will be used to update the marker. This will get 
		/// called when the DataContext changes, the position changes and the selected 
		/// state changes for a marker.
		/// </summary>
		/// <remarks>
		/// The call chain for the update are the following:
		/// (UseIcons && PushpinIconsMarkerUpdater)? -> UpdateMarker -> MarkerUpdater?
		/// </remarks>
		public Action<GooglePushpin, Marker> MarkerUpdater { get; set; }

		protected override Geopoint GetCenter()
		{
			return null;
		}

		protected override MapViewPortCoordinates GetViewPortCoordinates()
		{
			var visibleRegion = _internalMapView.Projection.VisibleRegion;
			return new MapViewPortCoordinates
			(
				northWest: new BasicGeoposition
				{
					// Far = top
					Latitude = visibleRegion.FarLeft.Latitude,
					Longitude = visibleRegion.FarLeft.Longitude,
				},
				northEast: new BasicGeoposition
				{
					// Far = top
					Latitude = visibleRegion.FarRight.Latitude,
					Longitude = visibleRegion.FarRight.Longitude,
				},
				southWest: new BasicGeoposition
				{
					// Near = bottom
					Latitude = visibleRegion.NearLeft.Latitude,
					Longitude = visibleRegion.NearLeft.Longitude,
				},
				southEast: new BasicGeoposition
				{
					// Near = bottom
					Latitude = visibleRegion.NearRight.Latitude,
					Longitude = visibleRegion.NearRight.Longitude,
				}
			);
		}

		protected override IEnumerable<IObservable<Unit>> GetViewPortChangedTriggers()
		{
			yield return System.Reactive.Linq.Observable
				.FromEventPattern<GMSCameraEventArgs>(
					h => _internalMapView.CameraPositionChanged += h,
					h => _internalMapView.CameraPositionChanged -= h)
				.Where(_ => !IsAnimating)
				.Select(_ => Unit.Default);
		}

		protected override ZoomLevel GetZoomLevel()
		{
			return (ZoomLevel)Position.Zoom;
		}

		protected override async Task SetViewPort(CancellationToken ct, MapViewPort viewPort)
		{
			//https://developers.google.com/maps/documentation/ios-sdk/views
			var animating = ObserveCameraPositionIdle()
				.ObserveOn(GetDispatcherScheduler())
				.FirstAsync(ct);

			CameraUpdate cameraBounds;
			if (viewPort.PointsOfInterest?.Any() ?? false)
			{
				//Rubber-band bounds to PointsOfInterest
				var bounds = viewPort
					.PointsOfInterest
					.Aggregate(new CoordinateBounds(), (currentBounds, coord) =>
						currentBounds.Including(new CLLocationCoordinate2D(coord.Position.Latitude, coord.Position.Longitude)));

				if (viewPort.Center != default(Geopoint))
				{
					bounds = AddPushpinPaddingToBounds(viewPort);
				}

				cameraBounds = CameraUpdate.FitBounds(bounds, 0);
			}
			else if (viewPort.ZoomLevel.HasValue)
			{
				var center = new CLLocationCoordinate2D(viewPort.Center.Position.Latitude, viewPort.Center.Position.Longitude);
				cameraBounds = CameraUpdate.SetTarget(center, (float)viewPort.ZoomLevel.Value);
			}
			else
			{
				var center = new CLLocationCoordinate2D(viewPort.Center.Position.Latitude, viewPort.Center.Position.Longitude);
				cameraBounds = CameraUpdate.SetTarget(center);
			}
			//TODO: pitch, heading

			if (!viewPort.IsAnimationDisabled)
			{
				_internalMapView.Animate(cameraBounds);
				await animating;
			}

			_internalMapView.MoveCamera(cameraBounds);

			_isViewPortInitialized = true;
		}

		protected override bool GetInitializationStatus()
		{
			return _isViewPortInitialized;
		}

		private CoordinateBounds AddPushpinPaddingToBounds(MapViewPort viewPort)
		{
			var frontiers = viewPort.GetBounds();

			// create ViewPort with calculated dimensions
			var northEastCorner = new CLLocationCoordinate2D(frontiers.EastFrontier, frontiers.NorthFrontier);
			var southWestCorner = new CLLocationCoordinate2D(frontiers.WestFrontier, frontiers.SouthFrontier);

			return new CoordinateBounds(northEastCorner, southWestCorner);
		}

		protected override void UpdateIcon(object icon)
		{
			if (_icon != null)
			{
				_logger.Error("Pushpins icons cannot be changed.");

				throw new InvalidOperationException("Pushpins icons cannot be changed.");
			}

			UpdateIcon(ref _icon, icon);
		}
		protected override void UpdateCompassButtonVisibilityInner(Visibility visibility)
		{
			_internalMapView.Settings.CompassButton = visibility == Visibility.Visible;
		}

		protected override void UpdateIsRotateGestureEnabledInner(bool isRotateGestureEnabled)
		{
			_internalMapView.Settings.RotateGestures = isRotateGestureEnabled;
		}

		protected override void UpdateMapPushpins(IGeoLocated[] items, IGeoLocated[] selectedItems)
		{
			_pushpinsLayer.Update(items,
				selectedItems,
				_ => new GooglePushpin
				{
					MarkerUpdater = UseIcons
						? PushpinIconsMarkerUpdater
						: (Action<GooglePushpin, Marker>)UpdateMarker
				}
			);
		}

		private void UpdateMarker(GooglePushpin pushpin, Marker marker)
		{
			// update z-index
			marker.ZIndex = pushpin.ZIndex;

			// call injected updater
			MarkerUpdater?.Invoke(pushpin, marker);
		}

		protected override void UpdateMapSelectedPushpins(IGeoLocated[] items)
		{
			_pushpinsLayer.UpdateSelection(items);
		}

		protected override void UpdateMapUserLocation(LocationResult locationAndStatus)
		{
			if (locationAndStatus != null)
				_internalMapView.MyLocationEnabled = locationAndStatus.IsSuccessful;
		}

		protected override void UpdateSelectedIcon(object icon)
		{
			if (_selectedIcon != null)
			{
				_logger.Error("Pushpins icons cannot be changed.");

				throw new InvalidOperationException("Pushpins icons cannot be changed.");
			}

			UpdateIcon(ref _selectedIcon, icon);
		}

		private bool TappedMarker(MapView mapView, Marker marker)
		{
			if (SelectionMode == MapSelectionMode.None)
			{
				return true;
			}

			var pushpin = _pushpinsLayer.Items.FirstOrDefault(p => p.Marker == marker);

			// Only unselect all pushpins if the selection mode is single AND if the clicked pushpin wasn't already
			// selected, otherwise it will just be set as selected again below.
			if (!AllowMultipleSelection && (!(pushpin?.IsSelected) ?? true))
			{
				UnselectAllPushpins();
			}

			if (pushpin != null)
			{
				pushpin.IsSelected = !pushpin.IsSelected;

				var selectedContent = GetSelectedAnnotationsContent();
				OnPushpinsSelected(selectedContent);
			}
			return true;
		}

		private void OnMapViewCoordinateTapped(object sender, GMSCoordEventArgs e)
		{
			if (!AllowMultipleSelection)
			{
				UnselectAllPushpins();
				OnPushpinsSelected(new IGeoLocated[0]);
			}

			//OnMapTapped(e.Coordinate);
		}

		private void UnselectAllPushpins()
		{
			_logger.Debug($" Unselecting all the '{_pushpinsLayer?.Items?.Count()}' pushpins.");

			foreach (var pushpin in _pushpinsLayer.Items)
			{
				pushpin.IsSelected = false;
			}

			_logger.Info($"Unselected all the '{_pushpinsLayer?.Items?.Count()}' pushpins.");
		}

		private IGeoLocated[] GetSelectedAnnotationsContent()
		{
			return _pushpinsLayer
				.Items
				.Where(p => p.IsSelected)
				.Select(p => p.Item)
				.ToArray();
		}

		private IObservable<Unit> ObserveCameraPositionIdle()
		{
			return Observable
				.FromEventPattern<GMSCameraEventArgs>(
					h => _internalMapView.CameraPositionIdle += h,
					h => _internalMapView.CameraPositionIdle -= h
				)
				.Select(_ => Unit.Default);
		}

		private void PushpinIconsMarkerUpdater(GooglePushpin pushpin, Marker marker)
		{
			_logger.Debug("Updating the pushpin.");

			var icon = pushpin.IsSelected
				? _selectedIcon
				: _icon;

			if (icon != null)
			{
				marker.Icon = icon;
			}

			UpdateMarker(pushpin, marker);

			_logger.Info("Updated the pushpins.");
		}
	}
}
#endif
