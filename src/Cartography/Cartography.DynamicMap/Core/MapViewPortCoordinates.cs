using System;
using Uno.Extensions;
using wdg = Windows.Devices.Geolocation;

namespace Cartography.DynamicMap
{
	public class MapViewPortCoordinates : IEquatable<MapViewPortCoordinates>
	{
		public MapViewPortCoordinates(wdg.BasicGeoposition northWest, wdg.BasicGeoposition northEast, wdg.BasicGeoposition southWest, wdg.BasicGeoposition southEast)
		{
			NorthWest = northWest;
			NorthEast = northEast;
			SouthWest = southWest;
			SouthEast = southEast;
		}

		/// <summary>
		/// Gets the coordinate of the point to the northwest extreme of the Region
		/// </summary>
		public wdg.BasicGeoposition NorthWest { get; }

		/// <summary>
		/// Gets the coordinate of the point to the northeast extreme of the Region
		/// </summary>
		public wdg.BasicGeoposition NorthEast { get;  }

		/// <summary>
		/// Gets the coordinate of the point to the southwest extreme of the Region
		/// </summary>
		public wdg.BasicGeoposition SouthWest { get; }

		/// <summary>
		/// Gets the coordinate of the point to the southwest extreme of the Region
		/// </summary>
		public wdg.BasicGeoposition SouthEast { get; }

		public bool Equals(MapViewPortCoordinates other)
		{
			return NorthEast.Equals(other?.NorthEast)
				&& SouthWest.Equals(other?.SouthWest)
				&& NorthWest.Equals(other?.NorthWest)
				&& SouthEast.Equals(other?.SouthEast);
		}

		public override string ToString()
		{
			return "[MapRegion] NW: {0}; {1}, NE: {2}; {3}, SW: {4}; {5}, SE: {6}; {7}".InvariantCultureFormat(
				Math.Round(NorthWest.Latitude, 4),
				Math.Round(NorthWest.Longitude, 4),
				Math.Round(NorthEast.Latitude, 4),
				Math.Round(NorthEast.Longitude, 4),
				Math.Round(SouthWest.Latitude, 4),
				Math.Round(SouthWest.Longitude, 4),
				Math.Round(SouthEast.Latitude, 4),
				Math.Round(SouthEast.Longitude, 4));
		}

		/// <summary>
		/// Returns true when the coordinates surround (inclusively) the given coordinate
		/// </summary>
		public bool IsSurrounding(wdg.Geopoint coordinate)
		{
			return coordinate.Position.Latitude >= SouthEast.Latitude
				&& coordinate.Position.Latitude <= NorthEast.Latitude
				&& coordinate.Position.Longitude >= SouthWest.Longitude
				&& coordinate.Position.Longitude <= SouthEast.Longitude;
		}
	}
}
