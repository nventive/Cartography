#if __ANDROID__
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Uno.Extensions;
using Uno.Logging;

namespace Cartography.MapService;

/// <summary>
/// Implementation of <see href="IMapService" />
/// </summary>
public class MapServiceAndroid : IMapService
{
	// GoogleMap api urls
	private const string DirectionsGeoCoordinatesUrlFormat = "http://maps.google.com/maps?daddr={0},{1}";
	private const string DirectionsNameUrlFormat = "http://maps.google.com/maps?daddr={0}";
	private const string LocationGeoCoordinatesUrlFormat = "geo:0,0?q={0},{1}";
	private const string LocationNameUrlFormat = "geo:0,0?q={0}";

	private readonly Android.Content.Context _contextProvider;
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="MapServiceAndroid"/> class.
	/// </summary>
	/// <param name="contextProvider">Android context provider</param>
	/// <param name="logger">Logger</param>
	public MapServiceAndroid(Android.Content.Context contextProvider, ILogger logger = null)
	{
		_contextProvider = contextProvider.Validation().NotNull("contextProvider");
		_logger = logger ?? NullLogger.Instance;
	}

	/// <inheritdoc/>
	public async Task ShowDirections(CancellationToken ct, MapRequest request)
	{
		_logger.Debug("Showing directions.");

		string url = null;

		if (request.IsCoordinatesSet)
		{
			// Label is not supported for GeoCoordinates in Android see : https://developers.google.com/maps/documentation/urls/guide#directions-action
			// And https://developers.google.com/maps/documentation/urls/android-intents
			url = DirectionsGeoCoordinatesUrlFormat.InvariantCultureFormat(request.Coordinates.Latitude, request.Coordinates.Longitude);
		}
		else if (request.LocationName.HasValueTrimmed())
		{
			url = DirectionsNameUrlFormat.InvariantCultureFormat(request.LocationName);
		}
		else
		{
			_logger.Error("Directions not shown because the coordinates or the location's name are null.");

			return;
		}

		await LaunchIntent(ct, url);

		_logger.Info("Directions shown.");
	}

	/// <inheritdoc/>
	public async Task ShowLocation(CancellationToken ct, MapRequest request)
	{
		_logger.Debug("Showing location.");

		string url = null;

		if (request.IsCoordinatesSet)
		{
			url = LocationGeoCoordinatesUrlFormat.InvariantCultureFormat(request.Coordinates.Latitude, request.Coordinates.Longitude);

			if (request.Label.HasValueTrimmed())
			{
				var encodedName = Android.Net.Uri.Encode(request.Label);

				url += $"({encodedName})";
			}
		}
		else if (request.LocationName.HasValueTrimmed())
		{
			url = LocationNameUrlFormat.InvariantCultureFormat(request.LocationName);
		}
		else
		{
			_logger.Error($"Location not shown because the coordinates or the location's name are null.");

			return;
		}

		await LaunchIntent(ct, url);

		_logger.Info("Location shown.");
	}

	private async Task LaunchIntent(CancellationToken ct, string url)
	{
		var intent = new global::Android.Content.Intent(
			Android.Content.Intent.ActionView,
			Android.Net.Uri.Parse(url)
		);

		intent = intent.AddFlags(Android.Content.ActivityFlags.NewTask);

		_contextProvider.StartActivity(intent);
	}
}
#endif
