﻿#if __IOS__
using Foundation;
using Uno.Extensions;
using Uno.Logging;

namespace Nventive.Location.MapService
{
	/// <summary>
	/// Service that provide locations and directions commands to Waze
	/// </summary>
	internal class WazeServiceProvider : IMapServiceProvider
	{
		/// <inheritdoc />
		public string Name => NavigationAppConstants.NavigationAppName.Waze;

		/// <inheritdoc />
		public NSUrl Url => NavigationAppConstants.NavigationAppNSUrl.WazeUrl;
		

		/// <inheritdoc />
		public NSUrl GetDirectionsUrl(MapRequest mapRequest)
		{
			this.Log().Debug(() => $"Showing directions using {nameof(WazeServiceProvider)}.");

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
			this.Log().Debug(() => $"Showing location using {nameof(WazeServiceProvider)}.");

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
}
#endif
