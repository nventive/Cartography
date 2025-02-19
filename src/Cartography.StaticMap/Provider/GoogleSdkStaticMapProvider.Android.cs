#if __ANDROID__
using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Views;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using static Android.Gms.Maps.GoogleMap;

namespace Cartography.StaticMap.Provider;

/// <summary>
/// Provides a static map created using the Google Maps SDK for Android.
/// The resulting static map is a googe map view.
/// </summary>
internal class GoogleSdkStaticMapProvider : IStaticMapProvider
{
	private MapView _internalMapView;
	private GoogleMap _map;

	/// <summary>
	/// Get a map with the specified parameters.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <param name="parameters">Map parameters</param>
	/// <returns>The map as a MapView</returns>
	public async Task<object> GetMap(CancellationToken ct, StaticMapParameters parameters)
	{
		if (this.Log().IsEnabled(LogLevel.Debug))
		{
			this.Log().Debug($"Getting a Google Sdk map with the scale: '{parameters?.Scale}', the width: '{parameters?.Width}', the height: '{parameters?.Height}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}'.");
		}

		GoogleMapOptions options = new GoogleMapOptions().InvokeLiteMode(true);
		_internalMapView = new MapView(Android.App.Application.Context, options);
		MapsInitializer.Initialize(Android.App.Application.Context);

		_internalMapView.OnCreate(null); // This is required otherwise the map does not appear
		_internalMapView.LayoutParameters = new ViewGroup.LayoutParams(parameters.Width, parameters.Height);

		var tcs = new TaskCompletionSource<Unit>();
		_internalMapView.GetMapAsync(new MapReadyCallback(async map => await OnMapReady(tcs, map, parameters)));
		await tcs.Task;

		_internalMapView.Measure(parameters.Width, parameters.Height);

		if (this.Log().IsEnabled(LogLevel.Information))
		{
			this.Log().Info($"Return a Google Sdk map with the scale: '{parameters?.Scale}', the width: '{parameters?.Width}', the height: '{parameters?.Height}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}'.");
		}

		return _internalMapView;
	}

	private async Task OnMapReady(TaskCompletionSource<Unit> tcs, GoogleMap map, StaticMapParameters parameters)
	{
		_map = map;
		_map.UiSettings.MapToolbarEnabled = false;
		_map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(parameters.ViewPort.Center.Position.Latitude, parameters.ViewPort.Center.Position.Longitude), (float)parameters.ViewPort.ZoomLevel.Value));

		// "By default when a user taps the map, the API starts the Google Maps mobile app. You can override this by using GoogleMap.setOnMapClickListener() to set your own listener"
		// Reference: https://developers.google.com/maps/documentation/android-sdk/lite#intents_to_launch_a_map_view_or_directions_request
		map.MapClick += Map_MapClick;

		tcs.TrySetResult(Unit.Default);
	}

	private void Map_MapClick(object sender, MapClickEventArgs e)
	{
		return;
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
}
#endif
