#if __IOS__
using Foundation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Uno.Logging;

namespace Cartography.MapService;

/// <summary>
/// Service that provide locations and directions commands to Google Maps
/// </summary>
internal class GoogleMapsServiceProvider : IMapServiceProvider
{
	private readonly ILogger _logger = NullLogger.Instance;

	/// <inheritdoc />
	public string Name => NavigationAppConstants.NavigationAppName.GoogleMaps;

	/// <inheritdoc />
	public NSUrl Url => NavigationAppConstants.NavigationAppNSUrl.GoogleMapsUrl;
	

	/// <inheritdoc />
	public NSUrl GetDirectionsUrl(MapRequest mapRequest)
	{
		_logger.Debug(() => $"Showing directions using {nameof(AppleMapsServiceProvider)}.");

		var latitude = $"{mapRequest.Coordinates.Latitude}".Replace(",", ".");
		var longitude = $"{mapRequest.Coordinates.Longitude}".Replace(",", ".");

		var isSearchMode = !mapRequest.IsCoordinatesSet;
		var input = isSearchMode
			? mapRequest.LocationName
			: $"{latitude},{longitude}";

		return new NSUrl(BuildQuery(input, isSearchMode, true));
	}

	/// <inheritdoc />
	public NSUrl GetLocationUrl(MapRequest mapRequest)
	{
		_logger.Debug(() => $"Showing location using {nameof(GoogleMapsServiceProvider)}.");

		var latitude = $"{mapRequest.Coordinates.Latitude}".Replace(",", ".");
		var longitude = $"{mapRequest.Coordinates.Longitude}".Replace(",", ".");

		var isSearchMode = !mapRequest.IsCoordinatesSet;
		var input = isSearchMode
			? mapRequest.LocationName
			: $"{latitude},{longitude}";

		return new NSUrl(BuildQuery(input, isSearchMode, false));
	}

	private string BuildQuery(string input, bool isSearch, bool isNavigating)
	{
		return NavigationAppConstants.NavigationAppNSUrl.GoogleMapsUrl.AbsoluteString
			+ (isNavigating
				? (isSearch
					? $"{NavigationAppConstants.GoogleMapsParameters.DestinationAddress}{input.Replace(" ", "+")}"
					: $"{NavigationAppConstants.GoogleMapsParameters.DestinationAddress}{input}"
				)
				: $"{NavigationAppConstants.GoogleMapsParameters.Search}{(isSearch ? input.Replace(' ', '+') : input)}"
			)
			+ (isNavigating ? NavigationAppConstants.GoogleMapsParameters.Driving : string.Empty);
	}
}
#endif
