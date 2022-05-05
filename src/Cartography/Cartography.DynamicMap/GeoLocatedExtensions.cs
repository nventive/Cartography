using System;
using System.Linq;

namespace Nventive.Location.DynamicMap
{
	public static class GeoLocatedExtensions
	{
		//public static GeoCoordinate Center<T>(this T[] items)
		//	where T : IGeoLocated
		//{
		//	if (items == null || !items.Any())
		//	{
		//		return null;
		//	}

		//	var coordinates = items.Select(item => item.Coordinates);

		//	return new GeoCoordinate(
		//		coordinates.Select(coordinate => coordinate.Latitude).Average(),
		//		coordinates.Select(coordinate => coordinate.Longitude).Average(),
		//		coordinates.Select(coordinate => coordinate.Altitude).Average()
		//	);
		//}

		public static bool IsGrouping(this IGeoLocated item)
		{
			return item is IGeoLocatedGrouping;
		}
	}
}
