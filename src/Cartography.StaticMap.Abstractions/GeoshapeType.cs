namespace Cartography.StaticMap;

public enum GeoshapeType
{
	/// <summary>
	/// The geographic region is a point.
	/// </summary>
	Geopoint,

	/// <summary>
	/// The geographic region is a circle with a center point and a radius.
	/// </summary>
	Geocircle,

	/// <summary>
	/// The geographic region is an order series of points.
	/// </summary>
	Geopath,

	/// <summary>
	/// The geographic region is a rectangular region.
	/// </summary>
	GeoboundingBox,
}
