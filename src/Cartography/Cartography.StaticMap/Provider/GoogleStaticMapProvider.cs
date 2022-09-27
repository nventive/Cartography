#if WINDOWS_UWP || __IOS__ || __ANDROID__
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using Windows.UI.Xaml.Media;
#if WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace Cartography.StaticMap.Provider
{
	/// <summary>
	/// Provides a static map created using the Google Maps web API.
	/// The resulting static map is a url.
	/// </summary>
	internal class GoogleStaticMapProvider : IStaticMapProvider
	{
		private const string ApiUrl = "http://maps.googleapis.com/maps/api/staticmap";
		private const int MaxWidth = 640;
		private const int MaxHeight = 640;

		private readonly string _apiKey;
		private readonly string _urlSigningSecretKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="GoogleStaticMapProvider"/> class.
		/// </summary>
		/// <param name="apiKey">Google Maps API key</param>
		/// <param name="urlSigningSecretKey">Google Maps URL signing secret key</param>
		public GoogleStaticMapProvider(string apiKey, string urlSigningSecretKey)
		{
			_apiKey = apiKey;
			_urlSigningSecretKey = urlSigningSecretKey;
		}

		/// <summary>
		/// Get a map with the specified parameters.
		/// </summary>
		/// <param name="ct">Cancellation token</param>
		/// <param name="parameters">Map parameters</param>
		/// <returns>The image's url.</returns>
		public async Task<object> GetMap(CancellationToken ct, StaticMapParameters parameters)
		{
			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug($"Getting a Google map with the scale: '{parameters?.Scale}', the width: '{parameters?.Width}', the height: '{parameters?.Height}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}'.");
			}

			var b = new StringBuilder(ApiUrl);

			b.Append("?");

			b.AppendFormatInvariant("center={0},{1}", parameters.ViewPort.Center.Position.Latitude, parameters.ViewPort.Center.Position.Longitude);

			b.AppendFormatInvariant("&zoom={0}", (double)parameters.ViewPort.ZoomLevel);

			b.AppendFormatInvariant("&size={0}x{1}", parameters.Width, parameters.Height);

			if (parameters.Scale > 1)
			{
				// highest scale available in the free tier (default is 1)
				b.AppendFormatInvariant("&scale=2");
			}

			if (_apiKey.HasValueTrimmed())
			{
				b.AppendFormatInvariant("&key={0}", _apiKey);
			}

			var signedUri = new Uri(
				GoogleSignedUrl.Sign(
					b.ToString(),
					_urlSigningSecretKey
				),
				UriKind.Absolute
			);

			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug($"Return a Google map with the scale: '{parameters?.Scale}', the width: '{parameters?.Width}', the height: '{parameters?.Height}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}'.");
			}

#if WINDOWS_UWP
			return new BitmapImage(signedUri);
#else
			return signedUri;
#endif
		}

		/// <summary>
		/// Struct to sign Google API URLs, mandatory to use the new billing system
		/// </summary>
		public struct GoogleSignedUrl
		{
			/// <summary>
			/// Sign an URL
			/// </summary>
			/// <param name="url">URL to sign</param>
			/// <param name="keyString">URL signing secret key</param>
			/// <returns>Signed URL</returns>
			public static string Sign(string url, string keyString)
			{
				var encoding = new ASCIIEncoding();

				// converting key to bytes will throw an exception, need to replace '-' and '_' characters first.
				string usablePrivateKey = keyString.Replace("-", "+").Replace("_", "/");
				byte[] privateKeyBytes = Convert.FromBase64String(usablePrivateKey);

				var uri = new Uri(url);
				byte[] encodedPathAndQueryBytes = encoding.GetBytes(uri.LocalPath + uri.Query);

				// compute the hash
				var algorithm = new HMACSHA1(privateKeyBytes);
				byte[] hash = algorithm.ComputeHash(encodedPathAndQueryBytes);

				// convert the bytes to string and make url-safe by replacing '+' and '/' characters
				string signature = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");

				// Add the signature to the existing URI.
				return uri.Scheme + "://" + uri.Host + uri.LocalPath + uri.Query + "&signature=" + signature;
			}
		}
	}
}
#endif
