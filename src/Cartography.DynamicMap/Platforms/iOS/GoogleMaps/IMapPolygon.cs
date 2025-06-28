using System.Collections.Generic;

namespace Cartography.DynamicMap
{
	/// <summary>
	/// Represents a polygon that can be displayed on a map.
	/// </summary>
	public interface IMapPolygon : IEnumerable<IGeoLocated>
	{
		/// <summary>
		/// Gets the resource key of the color to use to fill a circle or closed polygon.
		/// </summary>
		string FillColorKey { get; }

		/// <summary>
		/// Gets the resource key of the color to use to outline a circle or closed polygon, or to draw a polyline.
		/// </summary>
		string StrokeColorKey { get; }

		/// <summary>
		/// Gets the style of the polygon to display.
		/// </summary>
		PolygonStyle Style { get; }

		/// <summary>
		/// Gets the line width of the outline of a circle or closed polygon, or line width of a polyline.
		/// </summary>
		float LineWidth { get; }

		/// <summary>
		/// Gets the size of each drawn element for making a dotted line, or null to have a normal line.
		/// </summary>
		float? DotWidth { get; }

		/// <summary>
		/// Gets the size of the spacing between drawn element for making a dotted line, or null to have a normal line.
		/// </summary>
		float? DotSpacingWidth { get; }
	}
}
