#if !NETSTANDARD2_0
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
#if WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Cartography.StaticMap.Provider
{
	/// <summary>
	/// Provides a static map created using the Bing Imagery web API.
	/// The resulting static map is a uri.
	/// </summary>
	internal class BingStaticMapProvider : IStaticMapProvider
	{
		private const string ApiUrl = "http://dev.virtualearth.net/REST/v1/Imagery/Map/";

		// The API doc says 900x837, but these values are the real limits.
		private const int MaxWidth = 1440;
		private const int MaxHeight = 937;

		private readonly string _apiKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="BingStaticMapProvider"/> class.
		/// </summary>
		/// <param name="apiKey">Bing Imagery API key</param>
		public BingStaticMapProvider(string apiKey)
		{
			_apiKey = apiKey;
		}

		/// <summary>
		/// Get a map with the specified parameters.
		/// </summary>
		/// <param name="ct">Cancellation token</param>
		/// <param name="parameters">Map parameters</param>
		/// <returns>The map as a image url.</returns>
		public async Task<object> GetMap(CancellationToken ct, StaticMapParameters parameters)
		{
			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug($"Getting a Bing map with the scale: '{parameters?.Scale}', the width: '{parameters?.Width}', the height: '{parameters?.Height}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}'.");
			}

			var b = new StringBuilder(ApiUrl);

			b.AppendFormatInvariant("Road/");

			b.AppendFormatInvariant("{0},{1}", parameters.ViewPort.Center.Position.Latitude, parameters.ViewPort.Center.Position.Longitude);

			b.AppendFormatInvariant("/{0}", (double)parameters.ViewPort.ZoomLevel);

			if (parameters.Width > MaxWidth)
			{
				parameters.Width = MaxWidth;
			}

			if (parameters.Height > MaxHeight)
			{
				parameters.Height = MaxHeight;
			}

			b.AppendFormatInvariant("?mapSize={0},{1}", parameters.Width, parameters.Height);

			b.AppendFormatInvariant("&key={0}", _apiKey);

			var uri = new Uri(b.ToString(), UriKind.Absolute);

			if (this.Log().IsEnabled(LogLevel.Information))
			{
				this.Log().Info($"Return a Bing map with the scale: '{parameters?.Scale}', the width: '{parameters?.Width}', the height: '{parameters?.Height}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}'.");
			}

#if WINDOWS_UWP
			return new BitmapImage(uri);
#else
			return uri;
#endif
		}
	}
}
#endif
