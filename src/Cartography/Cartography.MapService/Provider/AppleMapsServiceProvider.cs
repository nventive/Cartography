#if __IOS__
using Foundation;
using Uno.Extensions;
using Uno.Logging;

namespace Nventive.Location.MapService
{
	/// <summary>
	/// Service that provide locations and directions commands to Apple Maps
	/// </summary>
	internal class AppleMapsServiceProvider : IMapServiceProvider
	{
		/// <inheritdoc />
		public string Name => NavigationAppConstants.NavigationAppName.AppleMaps;


		/// <inheritdoc />
		public NSUrl Url => NavigationAppConstants.NavigationAppNSUrl.AppleMapsUrl;
		

		/// <inheritdoc />
		public NSUrl GetDirectionsUrl(MapRequest mapRequest)
		{
			this.Log().Debug(() => $"Showing directions using {nameof(AppleMapsServiceProvider)}.");

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
			this.Log().Debug(() => $"Showing location using {nameof(AppleMapsServiceProvider)}.");

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
}
#endif
