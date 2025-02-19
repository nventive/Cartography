#if __IOS__
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using CoreLocation;
using GeolocatorService;
using MapKit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using UIKit;
using Uno.Extensions;

namespace Cartography.DynamicMap;

// See http://developer.xamarin.com/guides/ios/platform_features/ios_maps_walkthrough/ for more details.

partial class MapControl
{
	private MKMapView _internalMapView;

	private const double MINIMUM_ZOOM_ARC = 0.014; // approximately 1 miles (1 degree of arc ~= 69 miles)
	private const double MAX_DEGREES_ARC = 360.0;
	private const double MAX_GOOGLE_LEVELS = 20;
	private const double ZOOM_LEVEL_COEFFICIENT = 1.15; // Used to align apple map zoom scale to google map scale

	private readonly List<UIView> _pushPins = new List<UIView>();
	private IMapLayer<Pushpin> _pushpinsLayer;
	private readonly ILogger<MapControl> _logger = NullLogger<MapControl>.Instance;

	private readonly Dictionary<IMKOverlay, MKOverlayRenderer> _overlayRenderers = new Dictionary<IMKOverlay, MKOverlayRenderer>();

	private double? _animationDurationSeconds;
	private bool _isViewPortInitialized;

	/// <summary>
	/// Defines the amount of space that is created around the items on which the map is zooming.
	/// </summary>
	public double AutoZoomModifyer { get; set; }

	/// <summary>
	/// Sets the selector that will be used to create Pin view templates. The first parameter
	/// is the instance of the native annotation.
	/// </summary>
	public Func<MKAnnotationView, UIView> PinTemplate { get; set; }

	/// <summary>
	/// Sets the selector that will be used to create Pin group view templates. The first parameter
	/// is the instance of the native annotation.
	/// </summary>
	public Func<MKAnnotationView, UIView> PinGroupTemplate { get; set; }

	/// <summary>
	/// Enables or disables the zoom animations globally.
	/// </summary>
	public bool EnableZoomAnimations { get; set; }

	partial void Initialize()
	{
		Loaded += (sender, args) => OnLoaded();
		Unloaded += (sender, args) => OnUnloaded();

		_internalMapView = new MKMapView();
		AddDragGestureRecognizer(_internalMapView);
		_pushpinsLayer = new IosMapLayer(_internalMapView);
		Padding = Thickness.Empty;

		Template = new ControlTemplate(() => _internalMapView);//TODO use templates

		// Set so that the pins are not too close to the edges.
		AutoZoomModifyer = 1.15f;

		EnableZoomAnimations = true;

		_internalMapView.GetViewForAnnotation = OnGetViewForAnnotation;
		_internalMapView.DidDeselectAnnotationView += MapControl_DidDeselectAnnotationView;
		_internalMapView.DidSelectAnnotationView += MapControl_DidSelectAnnotationView;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Deprecated, please use the IsRotateGestureEnabled property instead.")]
	public bool RotateEnabled
	{
		get
		{
			return _internalMapView.RotateEnabled;
		}
		set
		{
			_internalMapView.RotateEnabled = value;
		}
	}

	public bool ShowPointsOfInterest
	{
		get
		{
			return _internalMapView.ShowsPointsOfInterest;
		}
		set
		{
			_internalMapView.ShowsPointsOfInterest = value;
		}
	}

	private bool _lastShowLocation;
	private bool _isLoaded;
	private void OnLoaded()
	{
		MonitorTapped();

		_internalMapView.ShowsUserLocation = _lastShowLocation;
		_isLoaded = true;
	}

	private void OnUnloaded()
	{
		_lastShowLocation = _internalMapView.ShowsUserLocation;
		_internalMapView.ShowsUserLocation = false;//Disable location tracking while unloaded
		_isLoaded = false;
	}

	/// <summary>
	/// Allows the map to be user interaction able.
	/// </summary>
	public override bool UserInteractionEnabled
	{
		get
		{
			return base.UserInteractionEnabled;
		}
		set
		{
			_internalMapView.ZoomEnabled = value;
			_internalMapView.ScrollEnabled = value;
			_internalMapView.PitchEnabled = value;
			_internalMapView.RotateEnabled = value;
			base.UserInteractionEnabled = value;
		}
	}

#region UserLocation
	protected override void UpdateMapUserLocation(LocationResult locationAndStatus)
	{
		if (locationAndStatus == null)
			return;

		_logger.LogDebug($"Updating the user's location on the map (status: '{locationAndStatus?.IsSuccessful}').");

		if (_isLoaded)
		{
			//TODO: Use a custom pin for current location ?
			_internalMapView.ShowsUserLocation = locationAndStatus.IsSuccessful;
		}
		else
		{
			_lastShowLocation = locationAndStatus.IsSuccessful;
		}

		_logger.LogInformation($"Updated the user's location on the map (status: '{locationAndStatus?.IsSuccessful}').");
	}
#endregion

#region ViewPort


	protected override IEnumerable<IObservable<Unit>> GetViewPortChangedTriggers()
	{
		yield return Observable
			.FromEventPattern<MKMapViewChangeEventArgs>(
				h => _internalMapView.RegionChanged += h,
				h => _internalMapView.RegionChanged -= h
			)
			.Where(ep => (!ep.EventArgs.Animated && !IsAnimating))
			.Select(_ => Unit.Default)
			.Do(_ =>
			{
				_logger.LogInformation("The view port changed.");
			});

		yield return Observable.Return(Unit.Default);
	}

	protected override ZoomLevel GetZoomLevel()
	{
		// https://github.com/jdp-global/MKMapViewZoom/blob/master/MKMapView%2BZoomLevel.m

		MKCoordinateRegion region = _internalMapView.Region;

		double centerPixelX = MapHelper.LongitudeToPixelSpaceX(region.Center.Longitude);
		double topLeftPixelX = MapHelper.LongitudeToPixelSpaceX(region.Center.Longitude - region.Span.LongitudeDelta / 2);

		double scaledMapWidth = (centerPixelX - topLeftPixelX) * 2;
		var mapSizeInPixels = Bounds.Size;
		double zoomScale = scaledMapWidth / mapSizeInPixels.Width;
		double zoomExponent = Math.Log(zoomScale) / Math.Log(2);
		double zoomLevel = 20 - zoomExponent;

		return new ZoomLevel(zoomLevel);
	}

	protected override async Task SetViewPort(CancellationToken ct, MapViewPort viewPort)
	{
		_logger.LogDebug($"Setting viewport with '{viewPort?.PointsOfInterest}' POIs and a zoom level of '{viewPort?.ZoomLevel}'.");

		if (viewPort.PointsOfInterest != null && viewPort.PointsOfInterest.Length > 0)
		{
			SetViewport(viewPort, preventAnimations: viewPort.IsAnimationDisabled);
		}
		else
		{
            //TODO
            // SetRegion(new MKCoordinateRegion(viewPort.Center, Region.Span), true);
            SetViewport(viewPort.Center.Position, viewPort.Heading, viewPort.Pitch, viewPort.ZoomLevel ?? ZoomLevel, preventAnimations: viewPort.IsAnimationDisabled);
		}

		_logger.LogInformation($"Viewport set with '{viewPort?.PointsOfInterest}' and a zoom level of '{viewPort?.ZoomLevel}'.");

		_isViewPortInitialized = true;
	}

	protected override bool GetInitializationStatus()
	{
		return _isViewPortInitialized;
	}

	private void SetViewport(MapViewPort viewPort, bool preventAnimations = false)
	{
		var padding = ApplyPaddingToCoordinate(new BasicGeoposition { Latitude = _internalMapView.CenterCoordinate.Latitude, Longitude = _internalMapView.CenterCoordinate.Longitude }, this.Padding, this.ZoomLevel);

		_internalMapView.CenterCoordinate = new CLLocationCoordinate2D(padding.Latitude, padding.Longitude);

		var locRect = ComputeBoundingRectangle(viewPort);

		bool animate = EnableZoomAnimations && !preventAnimations;

		if (animate && _animationDurationSeconds.HasValue)
		{
			var uiViewPropertyAnimator = new UIViewPropertyAnimator(_animationDurationSeconds.Value,
				UIViewAnimationCurve.EaseOut,
				() => _internalMapView.SetRegion(locRect, true));

			uiViewPropertyAnimator.StartAnimation();
		}
		else
		{
			_internalMapView.SetRegion(locRect, animate);
		}
	}

	private void SetViewport(BasicGeoposition centerCoordinate, double? heading, double? pitch, ZoomLevel zoomLevel, bool preventAnimations = false)
	{
		centerCoordinate = ApplyPaddingToCoordinate(centerCoordinate, this.Padding, zoomLevel);

		var region = MapHelper.CreateRegion(centerCoordinate, zoomLevel, Bounds.Size);

		bool animate = EnableZoomAnimations && !preventAnimations;

		if (animate && _animationDurationSeconds.HasValue)
		{

			var uiViewPropertyAnimator = new UIViewPropertyAnimator(_animationDurationSeconds.Value,
				UIViewAnimationCurve.EaseOut,
				() => _internalMapView.SetRegion(region, true));

			uiViewPropertyAnimator.StartAnimation();
		}
		else
		{
			_internalMapView.SetRegion(region, animate);
		}
	}
#endregion

#region Pushpins
	protected override void UpdateMapPushpins(IGeoLocated[] items, IGeoLocated[] selectedItems)
	{
		_logger.LogDebug($"Updating the '{items.Safe().Count()}' map pushpins (number of selected items: '{selectedItems?.Length}').");

		_pushpinsLayer.Update(items, selectedItems, CreatePushpin);

		_logger.LogInformation($"Updated the '{items.Safe().Count()}' map pushpins (number of selected items: '{selectedItems?.Length}').");
	}

	private Pushpin CreatePushpin(IGeoLocated item)
	{
		return item.IsGrouping()
			? new MapGroupAnnotation { Map = this }
			: new Pushpin { Map = this };
	}

	private MKAnnotationView OnGetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
	{
		if (annotation is MKUserLocation || PinTemplate == null)
			return null;

		var mapAnnotation = annotation as Pushpin;

		if (mapAnnotation != null)
		{
			// Determine the selector, fallback on standard template for groups if not set.
			var templateId = Pushpin.AnnotationId;
			var selector = annotation is MapGroupAnnotation ? PinGroupTemplate ?? PinTemplate : PinTemplate;

			var annotationView = mapView.DequeueReusableAnnotation(templateId);

			if (annotationView == null)
			{
				annotationView = new MKAnnotationView(mapAnnotation, templateId);
				annotationView.Add(selector(annotationView));

				_pushPins.Add(annotationView);

				// Set the frame size, or the pin will not be selectable.
				annotationView.Frame = annotationView.Subviews[0].Frame;

				if (_icon != null)
				{
					annotationView.CenterOffset = new CGPoint(
						PushpinIconsPositionOrigin.X * annotationView.Frame.Width,
						PushpinIconsPositionOrigin.Y * annotationView.Frame.Height);
				}

				if (annotationView.Frame.Width == 0 || annotationView.Frame.Height == 0)
				{
					_logger.LogDebug($"The frame for '{annotationView.Subviews[0]}' is '{annotationView.Frame}', which is too narrow. Set the frame for the Pin UIView.");
				}
			}

			var dataContextProvider = annotationView.Subviews.FirstOrDefault() as IDataContextProvider;

			if (dataContextProvider != null)
			{
				dataContextProvider.DataContext = mapAnnotation;
			}

			// We don't need callouts for this implementation.
			annotationView.CanShowCallout = false;

			//Refresh Pushin when view refresh
			var selectedContent = GetSelectedAnnotationsContent();

			OnPushpinsSelected(selectedContent);

			return annotationView;
		}

		return null;
	}
#endregion

#region Pushpins ICONS

	protected override void UpdateIcon(object icon)
	{
		if (_icon != null)
		{
			_logger.LogError($"Pushpins icons cannot be changed (_icon: '{_icon}')");

			throw new InvalidOperationException($"Pushpins icons cannot be changed (_icon: '{_icon}').");
		}

		UpdateIcon(ref _icon, icon);

		if (_icon != null)
		{
			PinTemplate = annotationView => new UIKit.UIImageView(_icon)
			{
				Frame = new CGRect(CGPoint.Empty, _icon.Size)
			};
		}
	}

	protected override void UpdateSelectedIcon(object icon)
	{
		if (_selectedIcon != null)
		{
			_logger.LogError($"Pushpins icons cannot be changed (_selectedIcon: '{_selectedIcon}')");

			throw new InvalidOperationException("Pushpins icons cannot be changed.");
		}

		UpdateIcon(ref _selectedIcon, icon);

		if (_selectedIcon != null)
		{
			_internalMapView.DidSelectAnnotationView += (snd, e) =>
			{
				var imageView = e.View.Subviews.FirstOrDefault() as UIImageView;
				if (imageView != null)
				{
					imageView.Image = _selectedIcon;
				}
			};

			_internalMapView.DidDeselectAnnotationView += (snd, e) =>
			{
				var imageView = e.View.Subviews.FirstOrDefault() as UIImageView;
				if (imageView != null)
				{
					imageView.Image = _icon;
				}
			};
		}
	}
#endregion

#region SelectedPushpins
	private void MapControl_DidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
	{
		_logger.LogDebug("Selecting the pushpins on the map.");

		GetDispatcherScheduler().Schedule(() =>
		{
			var mapAnnotation = e.View.Annotation as Pushpin;

			if (!AllowMultipleSelection)
			{
				_internalMapView.SelectedAnnotations
					.Safe()
					.Where(a => a.Handle != e.View.Annotation.Handle)
					.ForEach(a => _internalMapView.DeselectAnnotation(a, true));
			}

			if (mapAnnotation != null && !mapAnnotation.IsSelected && !mapAnnotation.IsSelectionChangeAlreadyHandled)
			{
				// Avoid infinite loops caused by this selection causing the native control to re-select
				mapAnnotation.IsSelectionChangeAlreadyHandled = true;

				mapAnnotation.IsSelected = true;

				mapAnnotation.IsSelectionChangeAlreadyHandled = false;

				var selectedContent = GetSelectedAnnotationsContent();

				OnPushpinsSelected(selectedContent);

				_logger.LogInformation("Selected the pushpins on the map.");
			}
		});
	}

	private void MapControl_DidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
	{
		_logger.LogDebug("Deselecting the pushpins on the map.");

		GetDispatcherScheduler().Schedule(() =>
		{
			var mapAnnotation = e.View.Annotation as Pushpin;

			if (mapAnnotation != null && mapAnnotation.IsSelected && !mapAnnotation.IsSelectionChangeAlreadyHandled)
			{
				// Avoid infinite loops caused by this selection causing the native control to deselect again
				mapAnnotation.IsSelectionChangeAlreadyHandled = true;

				mapAnnotation.IsSelected = false;
				_internalMapView.DeselectAnnotation(mapAnnotation, true);

				mapAnnotation.IsSelectionChangeAlreadyHandled = false;

				var selectedContent = GetSelectedAnnotationsContent();

				OnPushpinsSelected(selectedContent);

				_logger.LogInformation("Deselected the pushpins on the map.");
			}
		});
	}

	private IGeoLocated[] GetSelectedAnnotationsContent()
	{
		return _internalMapView.SelectedAnnotations
			.Safe()
			.OfType<Pushpin>()
			.Select(a => a.Item)
			.ToArray();
	}

	/// <summary>
	/// Update the whole list of select items.
	/// </summary>
	protected override void UpdateMapSelectedPushpins(IGeoLocated[] items)
	{
		_pushpinsLayer.UpdateSelection(items);
	}
#endregion

	private void MonitorTapped()
	{
		var tapRecognizer = new UITapGestureRecognizer(
			recognizer =>
			{
				var point = recognizer.LocationInView(_internalMapView);
				var coordinate = _internalMapView.ConvertPoint(point, _internalMapView);

				OnMapTapped(new Geocoordinate(coordinate.Latitude, coordinate.Longitude, DateTimeOffset.Now, new Geopoint(new BasicGeoposition(coordinate.Latitude, coordinate.Longitude))));
			});

		_internalMapView.AddGestureRecognizer(tapRecognizer);
	}

	private IObservable<Unit> ObserveRegionChanged()
	{
		return Observable
			.FromEventPattern<MKMapViewChangeEventArgs>(
				h => _internalMapView.RegionChanged += h,
				h => _internalMapView.RegionChanged -= h
			)
			.Select(_ => Unit.Default)
			.Do(_ =>
			{
				//OnPushpinsSelected(GetSelectedAnnotationsContent());

				_logger.LogInformation("The region changed.");
			});
	}

#region Helpers
	private MKCoordinateRegion ComputeBoundingRectangle(MapViewPort viewPort)
	{
		MKCoordinateRegion region;
		if (viewPort.Center == default(Geopoint))
		{
			var coordinates = viewPort.PointsOfInterest
				.Select(p => MKMapPoint.FromCoordinate(new CLLocationCoordinate2D { Latitude = p.Position.Latitude, Longitude = p.Position.Longitude }));

			var poly = MKPolygon.FromPoints(coordinates.ToArray());
			var mapRect = poly.BoundingMapRect;

			//convert MKCoordinateRegion from MKMapRect
			region = MKCoordinateRegion.FromMapRect(mapRect);
		}
		else //if the center is defined
		{
			//Set center
			region.Center.Latitude = viewPort.Center.Position.Latitude;
			region.Center.Longitude = viewPort.Center.Position.Longitude;

			var centerCoordinates = new BasicGeoposition { Latitude = region.Center.Latitude, Longitude = region.Center.Longitude };

			//Get the farthest pushpin 
			var farthestHorizontalPushpin = viewPort.PointsOfInterest
				.OrderBy(pushpin => centerCoordinates.GetHorizontalDistanceTo(pushpin.Position))
				.LastOrDefault();
			var farthestVerticalPushpin = viewPort.PointsOfInterest
				.OrderBy(pushpin => centerCoordinates.GetVerticalDistanceTo(pushpin.Position))
				.LastOrDefault();

			//Set the width and the height required to show the farthest pushpin while keeping the center centered.
			region.Span.LatitudeDelta = Math.Abs(viewPort.Center.Position.Latitude - farthestHorizontalPushpin.Position.Latitude) * 2;
			region.Span.LongitudeDelta = Math.Abs(viewPort.Center.Position.Longitude - farthestVerticalPushpin.Position.Longitude) * 2;
		}

		//apply padding to allow user to see the entire pushpin and not just a part of it.
		region.Span.LatitudeDelta *= viewPort.PointsOfInterestPadding.HorizontalPadding.Value;
		region.Span.LongitudeDelta *= viewPort.PointsOfInterestPadding.VerticalPadding.Value;

		//but padding can’t be bigger than the world
		if (region.Span.LatitudeDelta > MAX_DEGREES_ARC) { region.Span.LatitudeDelta = MAX_DEGREES_ARC; }
		if (region.Span.LongitudeDelta > MAX_DEGREES_ARC) { region.Span.LongitudeDelta = MAX_DEGREES_ARC; }

		//and don’t zoom in stupid-close on small samples
		if (region.Span.LatitudeDelta < MINIMUM_ZOOM_ARC) { region.Span.LatitudeDelta = MINIMUM_ZOOM_ARC; }
		if (region.Span.LongitudeDelta < MINIMUM_ZOOM_ARC) { region.Span.LongitudeDelta = MINIMUM_ZOOM_ARC; }

		return region;
	}

	private static double ZoomLevelToZoomScale(ZoomLevel zoomLevel)
	{
		var zoomExponent = 20 - zoomLevel.Value;
		double zoomScale = Math.Pow(2, zoomExponent);
		return zoomScale;
	}

	private static BasicGeoposition ApplyPaddingToCoordinate(BasicGeoposition coordinate, Thickness padding, ZoomLevel zoomLevel)
	{
		if (padding == Thickness.Empty)
		{
			return coordinate;
		}

		// determine the scale value from the zoom level 
		double zoomScale = ZoomLevelToZoomScale(zoomLevel);

		// convert center coordiate to pixel space 
		double centerPixelX = MapHelper.LongitudeToPixelSpaceX(coordinate.Longitude);
		double centerPixelY = MapHelper.LatitudeToPixelSpaceY(coordinate.Latitude);

		// offset center with padding
		centerPixelX += (((padding.Right - padding.Left) / 2) * zoomScale);
		centerPixelY += (((padding.Bottom - padding.Top) / 2) * zoomScale);

		return new BasicGeoposition
		{
			Longitude = MapHelper.PixelSpaceXToLongitude(centerPixelX),
			Latitude = MapHelper.PixelSpaceYToLatitude(centerPixelY)
		};
	}

	protected override Geopoint GetCenter()
	{
        var center = new BasicGeoposition
        {
            Latitude = _internalMapView.Region.Center.Latitude,
            Longitude = _internalMapView.Region.Center.Longitude
        };
        return new Geopoint(center);
	}

	protected override MapViewPortCoordinates GetViewPortCoordinates()
	{
		var center = _internalMapView.Region.Center;
		var latitudeDelta = _internalMapView.Region.Span.LatitudeDelta;
		var longitudeDelta = _internalMapView.Region.Span.LongitudeDelta;

		return new MapViewPortCoordinates(
		northWest: new BasicGeoposition
            {
			Latitude = GetLatitude(center.Latitude, latitudeDelta, isNorth: true),
			Longitude = GetLongitude(center.Longitude, longitudeDelta, isEast: false)
		},
		northEast: new BasicGeoposition
            {
			Latitude = GetLatitude(center.Latitude, latitudeDelta, isNorth: true),
			Longitude = GetLongitude(center.Longitude, longitudeDelta, isEast: true)
		},
		southWest: new BasicGeoposition
            {
			Latitude = GetLatitude(center.Latitude, latitudeDelta, isNorth: false),
			Longitude = GetLongitude(center.Longitude, longitudeDelta, isEast: false)
		},
		southEast: new BasicGeoposition
            {
			Latitude = GetLatitude(center.Latitude, latitudeDelta, isNorth: false),
			Longitude = GetLongitude(center.Longitude, longitudeDelta, isEast: true)
		}
		);
	}

	private double GetLatitude(double centerLatitude, double latitudeDelta, bool isNorth)
	{
		var factor = isNorth ? 1 : -1;

		// Need to bound the values because sometimes the delta exceeds the possible range
		return Math.Max(
			-90,
			Math.Min(
				90,
				centerLatitude + (latitudeDelta * factor / 2)
			));
	}

	private double GetLongitude(double centerLongitude, double longitudeDelta, bool isEast)
	{
		var factor = isEast ? 1 : -1;

		// Need to bound the values because sometimes the delta exceeds the possible range
		return Math.Max(
			-180,
			Math.Min(
				180,
				centerLongitude + (longitudeDelta * factor / 2)
			));
	}
#endregion

	internal void SelectAnnotation(Pushpin pushpin, bool animated)
	{
		_internalMapView.SelectAnnotation(pushpin, animated);
	}

	internal void DeselectAnnotation(Pushpin pushpin, bool animated)
	{
		_internalMapView.DeselectAnnotation(pushpin, animated);
	}

	protected override void UpdateCompassButtonVisibilityInner(Visibility visibility)
	{
		if (_internalMapView != null)
		{
			_internalMapView.ShowsCompass = visibility == Visibility.Visible;
		}
	}

	protected override void UpdateIsRotateGestureEnabledInner(bool isRotateGestureEnabled)
	{
		if (_internalMapView != null)
		{
			_internalMapView.RotateEnabled = isRotateGestureEnabled;
		}
	}

	protected override void SetAnimationDuration(double? animationDurationSeconds)
	{
		_animationDurationSeconds = animationDurationSeconds;
	}
}
#endif
