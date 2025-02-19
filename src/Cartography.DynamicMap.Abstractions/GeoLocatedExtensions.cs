using System.Linq;

namespace Cartography.DynamicMap;

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

		// Why was this commented out?
		return new Geopoint(new BasicGeoposition(
			coordinates.Select(coordinate => coordinate.Position.Latitude).Average(),
			coordinates.Select(coordinate => coordinate.Position.Longitude).Average(),
			coordinates.Select(coordinate => coordinate.Position.Altitude).Average())
		);
	}

	public static bool IsGrouping(this IGeoLocated item)
	{
		return item is IGeoLocatedGrouping;
	}
}
