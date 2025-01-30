using System;
using Windows.Devices.Geolocation;

namespace Cartography.DynamicMap;

public static partial class GeoCoordinateExtensions
{
	/// <summary>
	/// Gets a bool which indicates if the corrdinates are valid, i.e. latitude and longitude are neither infinite nor NaN
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	public static bool IsValid(this Geopoint coordinates)
	{
		var latitude = coordinates.Position.Latitude;
		var longitude = coordinates.Position.Longitude;

		return !double.IsNaN(latitude)
			   && !double.IsInfinity(latitude)
			   && !double.IsNaN(longitude)
			   && !double.IsInfinity(longitude);
	}

	public static double GetDistanceTo(this BasicGeoposition b, BasicGeoposition other)
	{
		return GetDistance(b.Latitude, b.Longitude, other.Latitude, other.Longitude);
	}

	internal static double GetDistance(double thisLatitude, double thisLongitude, double otherLatitude, double otherLongitude)
	{
		double R = 6371000; // earth radius in metres
		double dLat = ToRadians(otherLatitude - thisLatitude);
		double dLon = ToRadians(otherLongitude - thisLongitude);
		double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
			Math.Cos(ToRadians(thisLatitude)) * Math.Cos(ToRadians(otherLatitude)) *
			Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
		double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
		double d = R * c;
		return d;
	}

	// converts from degrees to radians
	private static double ToRadians(double degrees)
	{
		return (Math.PI / 180) * degrees;
	}
}
