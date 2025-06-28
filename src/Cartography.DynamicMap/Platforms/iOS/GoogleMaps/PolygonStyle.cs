namespace Cartography.DynamicMap;

/// <summary>
/// Polygon styles that can be displayed on a map.
/// </summary>
public enum PolygonStyle
{
	/// <summary>
	/// A closed figure will be drawn from all coordinates, filled and outlined according to properties.
	/// </summary>
	ClosedPolygon = 0,

	/// <summary>
	/// A multi-line path will be drawn from coordinates, with the provided stroke color and line width.
	/// </summary>
	Polyline = 1,

	/// <summary>
	/// A circle will be drawn for each coordinate, filled and outlined according to properties.
	/// </summary>
	Circles = 2,
}
