using System;

namespace Cartography.DynamicMap;

public static class BasicGeopositionExtension
{
	public static double GetHorizontalDistanceTo(this BasicGeoposition fromCoordinate, BasicGeoposition toCoordinate)
	{
		var centerToDestination = Math.Abs(toCoordinate.Latitude - fromCoordinate.Latitude);
		var destinationToCenter = Math.Abs(fromCoordinate.Latitude - toCoordinate.Latitude);

		return Math.Min(centerToDestination, destinationToCenter);
	}

	public static double GetVerticalDistanceTo(this BasicGeoposition fromCoordinate, BasicGeoposition toCoordinate)
	{
		return Math.Abs(toCoordinate.Longitude - fromCoordinate.Longitude);
	}
}
