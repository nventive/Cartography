#if __IOS__
using CoreLocation;
using GeolocatorService;
using Google.Maps;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace Cartography.DynamicMap.GoogleMap.iOS;

/// <summary>
/// iOS-only MapControl which uses Google Maps.
/// </summary>
public sealed partial class GoogleMapControl : MapControlBase
{
	private readonly MapView _internalMapView;
	private readonly ILogger _logger;
	private readonly GoogleMapLayer _pushpinsLayer;
	private bool _isViewPortInitialized;

	private CameraPosition Position => _internalMapView?.Camera;
	private bool UseIcons => _icon != null;

	/// <summary>
	/// Initializes a new instance of <see cref="GoogleMapControl"/>.
	/// </summary>
	public GoogleMapControl() : base()
	{
		_logger = this.Log();
		_internalMapView = new MapView();
		_pushpinsLayer = new GoogleMapLayer(_internalMapView);

		Padding = Thickness.Empty;
		Template = new ControlTemplate(() => _internalMapView); // TODO: Use Templates.

		AddDragGestureRecognizer(_internalMapView);

		_internalMapView.TappedMarker = TappedMarker;
		_internalMapView.CoordinateTapped += OnMapViewCoordinateTapped;
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

	/// <inheritdoc />
	protected override Geopoint GetCenter()
	{
		return new Geopoint(new BasicGeoposition(Position.Target.Latitude, Position.Target.Longitude));
	}

	/// <inheritdoc />
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

	/// <inheritdoc />
	protected override IEnumerable<IObservable<Unit>> GetViewPortChangedTriggers()
	{
		yield return Observable
			.FromEventPattern<GMSCameraEventArgs>(
				h => _internalMapView.CameraPositionIdle += h,
				h => _internalMapView.CameraPositionIdle -= h)
			.Where(_ => !IsAnimating)
			.Select(_ => Unit.Default)
			.Do(_ =>
			{
				LoggerMessages.ViewPortChangedIdle(_logger);
			});
	}

	/// <inheritdoc />
	protected override ZoomLevel GetZoomLevel()
	{
		return (ZoomLevel)Position.Zoom;
	}

	/// <inheritdoc />
	protected override bool GetInitializationStatus()
	{
		return _isViewPortInitialized;
	}

	/// <inheritdoc />
	protected override async Task SetViewPort(CancellationToken ct, MapViewPort viewPort)
	{
		LoggerMessages.SettingViewPort(_logger, viewPort);

		var dispatcherScheduler = GetDispatcherScheduler();

		// Await first idle event after our camera update (used for animated path).
		var animating = ObserveCameraPositionIdle()
			.ObserveOn(dispatcherScheduler)
			.FirstAsync(ct);

		CameraUpdate update;
		if (viewPort.PointsOfInterest is not null && viewPort.PointsOfInterest.Length is not 0)
		{
			LoggerMessages.FittingToPointsOfInterest(_logger);

			var bounds = viewPort
				.PointsOfInterest
				.Aggregate(new CoordinateBounds(), (currentBounds, coord) =>
				{
					var cllCoordinate = new CLLocationCoordinate2D(coord.Position.Latitude, coord.Position.Longitude);
					return currentBounds.Including(cllCoordinate);
				});

			if (viewPort.Center != default(Geopoint))
			{
				LoggerMessages.IncludingCenterInBounds(_logger, viewPort.Center);

				var viewPostBounds = viewPort.GetBounds();
				var northEastCorner = new CLLocationCoordinate2D(viewPostBounds.EastFrontier, viewPostBounds.NorthFrontier);
				var southWestCorner = new CLLocationCoordinate2D(viewPostBounds.WestFrontier, viewPostBounds.SouthFrontier);
				bounds = new CoordinateBounds(southWestCorner, northEastCorner);
			}

			update = CameraUpdate.FitBounds(bounds);
		}
		else if (viewPort.ZoomLevel.HasValue)
		{
			LoggerMessages.SettingZoomLevel(_logger, viewPort.ZoomLevel);

			var target = new CLLocationCoordinate2D(viewPort.Center.Position.Latitude, viewPort.Center.Position.Longitude);
			update = CameraUpdate.SetTarget(target, (float)viewPort.ZoomLevel.Value);
		}
		else
		{
			LoggerMessages.SettingCenter(_logger, viewPort.Center);

			var target = new CLLocationCoordinate2D(viewPort.Center.Position.Latitude, viewPort.Center.Position.Longitude);
			update = CameraUpdate.SetTarget(target);
		}

		if (!viewPort.IsAnimationDisabled)
		{
			LoggerMessages.AnimatingToNewPosition(_logger);

			_internalMapView.Animate(update);
			await animating;
		}
		else
		{
			LoggerMessages.MovingWithoutAnimation(_logger);

			_internalMapView.MoveCamera(update);
		}

		_isViewPortInitialized = true;

		LoggerMessages.SuccessfullySetViewPort(_logger, viewPort);
	}

	/// <inheritdoc />
	protected override void UpdateIcon(object icon)
	{
		if (_icon != null)
		{
			LoggerMessages.PushpinIconsCannotBeChanged(_logger);
			throw new InvalidOperationException("Pushpins icons cannot be changed.");
		}

		UpdateIcon(ref _icon, icon);
	}

	/// <inheritdoc />
	protected override void UpdateCompassButtonVisibilityInner(Visibility visibility)
	{
		_internalMapView.Settings.CompassButton = visibility == Visibility.Visible;
	}

	/// <inheritdoc />
	protected override void UpdateIsRotateGestureEnabledInner(bool isRotateGestureEnabled)
	{
		_internalMapView.Settings.RotateGestures = isRotateGestureEnabled;
	}

	/// <inheritdoc />
	protected override void UpdateMapPushpins(IGeoLocated[] items, IGeoLocated[] selectedItems)
	{
		_pushpinsLayer.Update(items,
			selectedItems,
			_ => new GooglePushpin
			{
				MarkerUpdater = UseIcons
					? PushpinIconsMarkerUpdater
					: UpdateMarker
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

	/// <inheritdoc />
	protected override void UpdateMapSelectedPushpins(IGeoLocated[] items)
	{
		_pushpinsLayer.UpdateSelection(items);
	}

	/// <inheritdoc />
	protected override void UpdateMapUserLocation(LocationResult locationResult)
	{
		_internalMapView.MyLocationEnabled = locationResult.IsSuccessful;
	}

	/// <inheritdoc />
	protected override void UpdateSelectedIcon(object icon)
	{
		if (_selectedIcon != null)
		{
			LoggerMessages.PushpinIconsCannotBeChanged(_logger);
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
			OnPushpinsSelected(Array.Empty<IGeoLocated>());
		}

		var location = new Geocoordinate(e.Coordinate.Latitude, e.Coordinate.Longitude, DateTimeOffset.Now, new Geopoint(new BasicGeoposition(e.Coordinate.Latitude, e.Coordinate.Longitude)));

		OnMapTapped(location);
	}

	private void UnselectAllPushpins()
	{
		var count = _pushpinsLayer?.Items?.Count() ?? 0;
		LoggerMessages.UnselectingAllPushpins(_logger, count);

		foreach (var pushpin in _pushpinsLayer.Items)
		{
			pushpin.IsSelected = false;
		}

		LoggerMessages.UnselectedAllPushpins(_logger, count);
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
		LoggerMessages.UpdatingPushpin(_logger);

		var icon = pushpin.IsSelected
			? _selectedIcon
			: _icon;

		if (icon != null)
		{
			marker.Icon = icon;
		}

		UpdateMarker(pushpin, marker);

		LoggerMessages.UpdatedPushpins(_logger);
	}
}
#endif
