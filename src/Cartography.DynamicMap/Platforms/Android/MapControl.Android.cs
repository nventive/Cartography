#if __ANDROID__
using System;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Cartography.DynamicMap;

partial class MapControl
{
	private const string COMPASS_TAG = "GoogleMapCompass";

	private GoogleMapView _internalMapView;

	private MapLifeCycleCallBacks _callbacks;
	private Android.App.Application _application;
	private MapReadyCallback _callback;
	private View _compass;

	private ILogger<MapControlBase> _logger = NullLogger<MapControlBase>.Instance;

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
	partial void Initialize()
	{
		Loaded += (sender, args) => OnLoaded();
		Unloaded += (sender, args) => OnUnloaded();

		_internalMapView = new GoogleMapView(Android.App.Application.Context, new GoogleMapOptions());

		Template = new ControlTemplate(() => _internalMapView);//TODO support templateing

		MapsInitializer.Initialize(Android.App.Application.Context);

		_internalMapView.GetMapAsync(_callback = new MapReadyCallback(OnMapReady));

		_internalMapView.OnCreate(null); // This otherwise the map does not appear
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
		//_isUserDragging.OnNext(e.Action == MotionEventActions.Move);
	}

	private GoogleMap _map;

	/// <summary>
	/// Idea is to register to the LifeCycleCallbacks and properly call the OnResume and OnPause methods when needed.
	/// This will release the GPS while the application is in the background
	/// </summary>
	private void HandleActivityLifeCycle()
	{
		_callbacks = new MapLifeCycleCallBacks(onPause: _internalMapView.OnPause, onResume: _internalMapView.OnResume);

		_application = (Android.App.Application)Context.ApplicationContext;
		if (_application != null)
		{
			_application.RegisterActivityLifecycleCallbacks(_callbacks);
		}
		else
		{
			_logger.LogInformation("ApplicationContext is invalid, could not RegisterActivityLifecycleCallbacks to release GPS when application is paused.");
		}
	}
}
#endif
