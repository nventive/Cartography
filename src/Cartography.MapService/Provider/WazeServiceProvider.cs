#if __IOS__
using Foundation;
using Microsoft.Extensions.Logging;
using Uno.Logging;

namespace Cartography.MapService;

/// <summary>
/// Service that provide locations and directions commands to Waze
/// </summary>
internal class WazeServiceProvider : IMapServiceProvider
{
	private readonly ILogger _logger;

	/// <inheritdoc />
	public string Name => NavigationAppConstants.NavigationAppName.Waze;

	/// <inheritdoc />
	public NSUrl Url => NavigationAppConstants.NavigationAppNSUrl.WazeUrl;

	public WazeServiceProvider()
	{
		_logger = this.Log();
	}

    /// <inheritdoc />
    public NSUrl GetDirectionsUrl(MapRequest mapRequest)
	{
		_logger.Debug(() => $"Showing directions using {nameof(WazeServiceProvider)}.");

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
		_logger.Debug(() => $"Showing location using {nameof(WazeServiceProvider)}.");

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
		return NavigationAppConstants.NavigationAppNSUrl.WazeUrl.AbsoluteString
			+ (isSearch
				? $"{NavigationAppConstants.WazeParameters.Search}{input.Replace(" ", "%20")}"
				: $"{NavigationAppConstants.WazeParameters.Coordinates}{input}"
			)
			+ (isNavigating ? NavigationAppConstants.WazeParameters.Navigate : string.Empty);
	}
}
#endif
