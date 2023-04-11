#if __IOS__
using System.Threading;
using System.Threading.Tasks;
using Google.Maps;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;

namespace Cartography.StaticMap.Provider
{
	/// <summary>
	/// Provides a static map created using the Google Maps SDK for iOS.
	/// The resulting static map is a google map view.
	/// </summary>
	internal class GoogleSdkStaticMapProvider : IStaticMapProvider
	{
		private MapView _mapView;

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

			var camera = CameraPosition.FromCamera(
				parameters.ViewPort.Center.Position.Latitude,
				parameters.ViewPort.Center.Position.Longitude,
				(float)parameters.ViewPort.ZoomLevel.Value);

			_mapView = MapView.FromCamera(CoreGraphics.CGRect.Empty, camera);

			_mapView.UserInteractionEnabled = false;

			if (this.Log().IsEnabled(LogLevel.Information))
			{
				this.Log().Info($"Return a Google Sdk map with the scale: '{parameters?.Scale}', the width: '{parameters?.Width}', the height: '{parameters?.Height}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}'.");
			}

			return _mapView;
		}
	}
}
#endif
