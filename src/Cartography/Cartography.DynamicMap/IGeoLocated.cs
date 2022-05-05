using System;
using Windows.Devices.Geolocation;

namespace Nventive.Location.DynamicMap
{
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
}
