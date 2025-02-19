#if __IOS__
using Foundation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Uno.Logging;

namespace Cartography.MapService;

/// <summary>
/// Service that provide locations and directions commands to Apple Maps
/// </summary>
internal class AppleMapsServiceProvider : IMapServiceProvider
{
	private readonly ILogger _logger;

	/// <inheritdoc />
	public string Name => NavigationAppConstants.NavigationAppName.AppleMaps;

	/// <inheritdoc />
	public NSUrl Url => NavigationAppConstants.NavigationAppNSUrl.AppleMapsUrl;
	
	public AppleMapsServiceProvider()
	{
		_logger = this.Log();
	}

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
		_logger.Debug(() => $"Showing location using {nameof(AppleMapsServiceProvider)}.");

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
		return NavigationAppConstants.AppleMapsParameters.UrlScheme
			+ (isNavigating
				? (isSearch
					? $"{NavigationAppConstants.AppleMapsParameters.DestinationAddress}{input.Replace(" ", "+")}"
					: $"{NavigationAppConstants.AppleMapsParameters.DestinationAddress}{input}"
				)
				: (isSearch
					? $"{NavigationAppConstants.AppleMapsParameters.Search}{input.Replace(' ', '+')}"
					: $"{NavigationAppConstants.AppleMapsParameters.Coordinates}{input}"
						+ $"{NavigationAppConstants.AppleMapsParameters.PinName}{input}"
				)
			)
			+ (isNavigating ? NavigationAppConstants.AppleMapsParameters.Driving : string.Empty);
	}
}
#endif
