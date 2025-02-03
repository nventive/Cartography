namespace Cartography;

public class Geopoint
{
	public Geopoint(BasicGeoposition position)
	{
		Position = position;
	}

	public Geopoint(BasicGeoposition position, GeoshapeType geoshapeType)
	{
		Position = position;
		GeoshapeType = geoshapeType;
	}

	/// <summary>
	/// The position of a geographic point.
	/// </summary>
	public BasicGeoposition Position { get; set; }

	/// <summary>
	/// The type of geographic shape. (Point, Circle, Path, Box)
	/// </summary>
	public GeoshapeType GeoshapeType { get; set; }
}
