#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreLocation;
using Google.Maps;
using Microsoft.Extensions.Logging;
using UIKit;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GeolocatorService;
using System.Reactive.Concurrency;

namespace Cartography.DynamicMap;

/// <summary>
/// iOS-only MapControl which uses Google Maps.
/// </summary>
public partial class GoogleMapControl : MapControlBase
{
	private MapView _internalMapView;
	private ILogger _logger;
	private readonly IDispatcherScheduler _dispatcherScheduler;
	private GoogleMapLayer _pushpinsLayer;
	private bool _isViewPortInitialized;

	private CameraPosition Position => _internalMapView?.Camera;
	private bool UseIcons => _icon != null;

	private static readonly nfloat DefaultPushpinsBoundsPadding = 30;

	public GoogleMapControl() : base()
	{
		Initialize();
	}

	private void Initialize()
	{
		_logger = this.Log();

		Loaded += (sender, args) => OnLoaded();
		Unloaded += (sender, args) => OnUnloaded();

		_internalMapView = new MapView();
		AddDragGestureRecognizer(_internalMapView);
		_pushpinsLayer = new GoogleMapLayer(_internalMapView);
		Padding = Thickness.Empty;

		Template = new ControlTemplate(() => _internalMapView);//TODO use templates

		// Set so that the pins are not too close to the edges.
		_pushpinsLayer = new GoogleMapLayer(_internalMapView);

		_internalMapView.TappedMarker = TappedMarker;
		_internalMapView.CoordinateTapped += OnMapViewCoordinateTapped;
	}

	private bool _isLoaded;
	private void OnLoaded()
	{
		_isLoaded = true;
	}

	private void OnUnloaded()
	{
		_isLoaded = false;
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
		return new Geopoint(new BasicGeoposition(Position.Target.Latitude, Position.Target.Longitude));
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
		yield return Observable
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

	protected override bool GetInitializationStatus()
	{
		return _isViewPortInitialized;
	}

	protected override async Task SetViewPort(CancellationToken ct, MapViewPort viewPort)
	{
		//https://developers.google.com/maps/documentation/ios-sdk/views
		var animating = ObserveCameraPositionIdle()
			.ObserveOn(_dispatcherScheduler)
			.FirstAsync(ct);

		CameraUpdate update;
		if (viewPort.PointsOfInterest?.Any() ?? false)
		{
			//Rubber-band bounds to PointsOfInterest

			var bounds = viewPort
				.PointsOfInterest
				.Aggregate(new CoordinateBounds(), (currentBounds, coord) =>
				{
					var cllCoordinate = new CLLocationCoordinate2D(coord.Position.Latitude, coord.Position.Longitude);

					return currentBounds.Including(cllCoordinate);
				});

			if (viewPort.Center != default(Geopoint))
			{
				//Stretch bounds to center around Center
				var latDiff = Math.Max(Math.Abs(bounds.NorthEast.Latitude - viewPort.Center.Position.Latitude), Math.Abs(viewPort.Center.Position.Latitude - bounds.SouthWest.Latitude));
				var lonDiff = Math.Max(Math.Abs(bounds.NorthEast.Longitude - viewPort.Center.Position.Longitude), Math.Abs(viewPort.Center.Position.Longitude - bounds.SouthWest.Longitude));

				var northEast = new CLLocationCoordinate2D(viewPort.Center.Position.Latitude - latDiff, viewPort.Center.Position.Longitude - lonDiff);
				var southwest = new CLLocationCoordinate2D(viewPort.Center.Position.Latitude + latDiff, viewPort.Center.Position.Longitude + lonDiff);
				bounds = new CoordinateBounds(northEast, southwest);
			}
			var padding = viewPort.PointsOfInterestPadding.HorizontalPadding ?? DefaultPushpinsBoundsPadding;
			update = CameraUpdate.FitBounds(bounds, (nfloat)padding);
		}
		else if (viewPort.ZoomLevel.HasValue)
		{
			var target = new CLLocationCoordinate2D(viewPort.Center.Position.Latitude, viewPort.Center.Position.Longitude);
			update = CameraUpdate.SetTarget(target, (float)viewPort.ZoomLevel.Value);
		}
		else
		{
			var target = new CLLocationCoordinate2D(viewPort.Center.Position.Latitude, viewPort.Center.Position.Longitude);
			update = CameraUpdate.SetTarget(target);
		}
		//TODO: pitch, heading

		if (!viewPort.IsAnimationDisabled)
		{
			_internalMapView.Animate(update);
			await animating;
		}
		else
		{
			_internalMapView.MoveCamera(update);
		}

		_isViewPortInitialized = true;
	}

	protected override void UpdateIcon(object icon)
	{
		if (_icon != null)
		{
			if (_logger.IsEnabled(LogLevel.Error))
			{
				_logger.LogError("Pushpins icons cannot be changed.");
			}

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

	protected override void UpdateMapUserLocation(LocationResult locationResult)
	{
		_internalMapView.MyLocationEnabled = locationResult.IsSuccessful;
	}

	protected override void UpdateSelectedIcon(object icon)
	{
		if (_selectedIcon != null)
		{
			if (_logger.IsEnabled(LogLevel.Error))
			{
				_logger.LogError("Pushpins icons cannot be changed.");
			}

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

		var location = new Geocoordinate(e.Coordinate.Latitude, e.Coordinate.Longitude, DateTimeOffset.Now, new Geopoint(new BasicGeoposition(e.Coordinate.Latitude, e.Coordinate.Longitude)));

		OnMapTapped(location);
	}

	private void UnselectAllPushpins()
	{
		if (_logger.IsEnabled(LogLevel.Debug))
		{
			_logger.LogDebug($"Unselecting all the '{_pushpinsLayer?.Items?.Count()}' pushpins.");
		}

		foreach (var pushpin in _pushpinsLayer.Items)
		{
			pushpin.IsSelected = false;
		}

		if (_logger.IsEnabled(LogLevel.Information))
		{
			_logger.LogInformation($"Unselected all the '{_pushpinsLayer?.Items?.Count()}' pushpins.");
		}
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
			).Select(_ => Unit.Default);

	}

	private void PushpinIconsMarkerUpdater(GooglePushpin pushpin, Marker marker)
	{
		if (_logger.IsEnabled(LogLevel.Debug))
		{
			_logger.LogDebug("Updating the pushpin.");
		}

		var icon = pushpin.IsSelected
			? _selectedIcon
			: _icon;

		if (icon != null)
		{
			marker.Icon = icon;
		}

		UpdateMarker(pushpin, marker);

		if (_logger.IsEnabled(LogLevel.Information))
		{
			_logger.LogInformation("Updated the pushpins.");
		}
	}
}
#endif
