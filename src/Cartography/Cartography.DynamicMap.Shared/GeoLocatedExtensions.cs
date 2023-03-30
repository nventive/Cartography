using System;
using System.Linq;
using Windows.Devices.Geolocation;

namespace Cartography.DynamicMap
{
	public static class GeoLocatedExtensions
	{
		public static Geopoint Center<T>(this T[] items)
			where T : IGeoLocated
		{
			if (items == null || !items.Any())
			{
				return null;
			}

			var coordinates = items.Select(item => item.Coordinates);

			return new Geopoint(new BasicGeoposition());

			//return new Geopoint(
			//	coordinates.Select(coordinate => coordinate.Position.Latitude).Average(),
			//	coordinates.Select(coordinate => coordinate.Position.Longitude).Average(),
			//	coordinates.Select(coordinate => coordinate.Position.Altitude).Average()
			//);
		}

		public static bool IsGrouping(this IGeoLocated item)
		{
			return item is IGeoLocatedGrouping;
		}
	}
}
