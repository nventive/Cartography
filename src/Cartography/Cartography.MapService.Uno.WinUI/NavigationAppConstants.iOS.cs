#if __IOS__
using Foundation;

namespace Cartography.MapService
{
	/// <summary>
	/// This class aggregates maps constants.
	/// </summary>
	internal static partial class NavigationAppConstants
	{
		/// <summary>
		/// This class aggregates alert constants.
		/// </summary>
		public class NavigationAppName
		{
			/// <summary>
			/// This class aggregates alert constants.
			/// </summary>
			public const string AppleMaps = "Apple Maps";

			/// <summary>
			/// This class aggregates alert constants.
			/// </summary>
			public const string GoogleMaps = "Google Maps";

			/// <summary>
			/// This class aggregates alert constants.
			/// </summary>
			public const string Waze = "Waze";
		}

		/// <summary>
		/// This class represents the navigation apps url.
		/// </summary>
		public class NavigationAppNSUrl
		{
			/// <summary>
			/// This is the Apple Maps NSUrl.
			/// To check if the app is installed and to be used with URL Scheme.
			/// </summary>
			public static readonly NSUrl AppleMapsUrl = new NSUrl("maps://");

			/// <summary>
			/// This is the Google Maps NSUrl.
			/// To check if the app is installed and to be used with URL Schemes.
			/// </summary>
			public static readonly NSUrl GoogleMapsUrl = new NSUrl("comgooglemaps://");

			/// <summary>
			/// This is the Waze NSUrl.
			/// To check if the app is installed.
			/// </summary>
			public static readonly NSUrl WazeUrl = new NSUrl("waze://");
		}

		/// <summary>
		/// This class represents the Google Maps URL Scheme parameters.
		/// https://developer.apple.com/library/archive/featuredarticles/iPhoneURLScheme_Reference/MapLinks/MapLinks.html
		/// </summary>
		public class AppleMapsParameters
		{
			/// <summary>
			/// This is the Waze Deep Links URL.
			/// To be used with URL Scheme.
			/// </summary>
			public const string UrlScheme = "http://maps.apple.com/?";

			/// <summary>
			/// This is the destination address.
			/// Can be coordinates (latitude,longitude)
			/// Example : "-41.0123,75.456"
			/// Or an address
			/// Example : "Google,+1600+Amphitheatre+Parkway,+Mountain+View"
			/// </summary>
			public const string DestinationAddress = "daddr=";

			/// <summary>
			/// This is the location to search.
			/// Must be a string where space are replaced by "+"
			/// Example : "Mexico+Restaurant"
			/// </summary>
			public const string Search = "q=";

			/// <summary>
			/// This add a pin with the specified name at the specified location
			/// </summary>
			public const string PinName = "&q=";

			/// <summary>
			/// Represent the location the user is looking for.
			/// Must be coordinates (latitude,longitude)
			/// Example : "-41.0123,75.456"
			/// </summary>
			public const string Coordinates = "ll=";

			/// <summary>
			/// Direction given using a vehicule.
			/// </summary>
			public const string Driving = "&dirflg=d";

			/// <summary>
			/// Direction given using the public transportation
			/// </summary>
			public const string Transit = "&dirflg=r";

			/// <summary>
			/// Direction given using your legs
			/// </summary>
			public const string Walking = "&dirflg=w";
		}

		/// <summary>
		/// This class represents the Google Maps URL Scheme parameters.
		/// https://developers.google.com/maps/documentation/urls/ios-urlscheme#universal_links_and_google_maps
		/// </summary>
		public class GoogleMapsParameters
		{
			/// <summary>
			/// This is the destination address.
			/// Can be coordinates (latitude,longitude)
			/// Example : "-41.0123,75.456"
			/// Or an address
			/// Example : "Google,+1600+Amphitheatre+Parkway,+Mountain+View"
			/// </summary>
			public const string DestinationAddress = "?daddr=";

			/// <summary>
			/// This is the address to search.
			/// Must be a string where whitespace are replaced by "+"
			/// Example : "Google,+1600+Amphitheatre+Parkway,+Mountain+View"
			/// </summary>
			public const string Search = "?q=";

			/// <summary>
			/// This is the center of the viewport.
			/// Must be coordinates (latitude,longitude)
			/// Example : "-41.0123,75.456"
			/// </summary>
			public const string Center = "?center=";

			/// <summary>
			/// Direction given using a vehicule.
			/// </summary>
			public const string Driving = "&directionsmode=driving";

			/// <summary>
			/// Direction given using the public transportation
			/// </summary>
			public const string Transit = "&directionsmode=transit";

			/// <summary>
			/// Direction given using a bicycle
			/// </summary>
			public const string Bicycling = "&directionsmode=bicycling";

			/// <summary>
			/// Direction given using your legs
			/// </summary>
			public const string Walking = "&directionsmode=walking";
		}

		/// <summary>
		/// This class represents the Waze Deep Links parameters.
		/// https://developers.google.com/waze/deeplinks
		/// </summary>
		public class WazeParameters
		{
			/// <summary>
			/// Represent the location the user is looking for.
			/// Must be coordinates (latitude,longitude)
			/// Example : "-41.0123,75.456"
			/// </summary>
			public const string Coordinates = "?ll=";

			/// <summary>
			/// Represent the location the user is looking for.
			/// Must be a string were whitespaces are replaced by "%20"
			/// Example : "Google,%201600%20Amphitheatre%20Parkway,%20Mountain%20View"
			/// </summary>
			public const string Search = "?q=";

			/// <summary>
			/// To get direction from current user location to destination location
			/// </summary>
			public const string Navigate = "&navigate=yes";
		}
	}
}
#endif
