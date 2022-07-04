using System;
using GeolocatorService;
using Uno.Extensions;
using Windows.Devices.Geolocation;

namespace Cartography.DynamicMap
{
	public class MapViewPortCoordinates : IEquatable<MapViewPortCoordinates>
	{
		public MapViewPortCoordinates(BasicGeoposition northWest, BasicGeoposition northEast, BasicGeoposition southWest, BasicGeoposition southEast)
		{
			NorthWest = northWest;
			NorthEast = northEast;
			SouthWest = southWest;
			SouthEast = southEast;
		}

		/// <summary>
		/// Gets the coordinate of the point to the northwest extreme of the Region
		/// </summary>
		public BasicGeoposition NorthWest { get; }

		/// <summary>
		/// Gets the coordinate of the point to the northeast extreme of the Region
		/// </summary>
		public BasicGeoposition NorthEast { get;  }

		/// <summary>
		/// Gets the coordinate of the point to the southwest extreme of the Region
		/// </summary>
		public BasicGeoposition SouthWest { get; }

		/// <summary>
		/// Gets the coordinate of the point to the southwest extreme of the Region
		/// </summary>
		public BasicGeoposition SouthEast { get; }

		public bool Equals(MapViewPortCoordinates other)
		{
			return NorthEast.Equals(other?.NorthEast)
				&& SouthWest.Equals(other?.SouthWest)
				&& NorthWest.Equals(other?.NorthWest)
				&& SouthEast.Equals(other?.SouthEast);
		}

		public override string ToString()
		{
			return "[MapRegion] NW: {0}, NE: {1}, SW: {2}, SE: {3}".InvariantCultureFormat(
				NorthWest,
				NorthEast,
				SouthWest,
				SouthEast);
		}

		/// <summary>
		/// Returns true when the coordinates surround (inclusively) the given coordinate
		/// </summary>
		public bool IsSurrounding(Geocoordinate coordinate)
		{
			return coordinate.Latitude >= SouthEast.Latitude
				&& coordinate.Latitude <= NorthEast.Latitude
				&& coordinate.Longitude >= SouthWest.Longitude
				&& coordinate.Longitude <= SouthEast.Longitude;
		}
	}
}
