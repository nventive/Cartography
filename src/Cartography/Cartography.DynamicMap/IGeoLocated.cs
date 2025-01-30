using Windows.Devices.Geolocation;

namespace Cartography.DynamicMap;

/// <summary>
/// An item which exposed its location
/// </summary>
public interface IGeoLocated
{
	/// <summary>
	/// Coordinate of the item
	/// </summary>
	Geopoint Coordinates { get; }
}
