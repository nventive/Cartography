namespace Cartography.DynamicMap;

public class ViewPortBounds
{
	public ViewPortBounds(double north, double south, double east, double west)
	{
		NorthFrontier = north;
		SouthFrontier = south;
		EastFrontier = east;
		WestFrontier = west;
	}

	public double NorthFrontier { get; }

	public double SouthFrontier { get; }

	public double EastFrontier { get; }

	public double WestFrontier { get; }
}
