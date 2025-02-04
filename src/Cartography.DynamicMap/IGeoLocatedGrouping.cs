using System.Collections;
using System.Linq;
using Windows.Devices.Geolocation;

namespace Cartography.DynamicMap;

/// <summary>
/// Represents a geolocated pushpin grouping
/// </summary>
/// <typeparam name="T">The type of pushpin</typeparam>
public interface IGeoLocatedGrouping<T> : IGeoLocatedGrouping, IGrouping<Geocoordinate, T>
{
}

/// <summary>
/// Represents a geolocated pushpin grouping
/// </summary>
public interface IGeoLocatedGrouping : IGeoLocated, IEnumerable
{
}
