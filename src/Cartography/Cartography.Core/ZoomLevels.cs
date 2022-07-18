using System;
using System.Collections.Generic;
using System.Text;

namespace Cartography.Core
{
	/// <summary>
	/// Implements a set of predefined ZoomLevel.
	/// </summary>
	public static class ZoomLevels
	{
		/// <summary>
		/// The minial zoom level (i.e. map is fully zommed out)
		/// </summary>
		public static readonly ZoomLevel Minimum = new ZoomLevel(1);

		/// <summary>
		/// The maximum zoom level (i.e. maps is fully zommed in)
		/// </summary>
		public static readonly ZoomLevel Maximum = new ZoomLevel(20);

		/// <summary>
		/// See the whole planisphere.
		/// </summary>
		public static readonly ZoomLevel Earth = Minimum;

		/// <summary>
		/// Are you crazy ?
		/// </summary>
		public static readonly ZoomLevel Mars = Minimum;

		/// <summary>
		/// See about a whole state / country.
		/// </summary>
		public static readonly ZoomLevel State = new ZoomLevel(6);

		/// <summary>
		/// See a whole big city and its surroundings.
		/// </summary>
		public static readonly ZoomLevel Region = new ZoomLevel(9);

		/// <summary>
		/// See a about a whole big city.
		/// </summary>
		public static readonly ZoomLevel City = new ZoomLevel(12);

		/// <summary>
		/// See a specific place with its neighborhood.
		/// </summary>
		public static readonly ZoomLevel District = new ZoomLevel(15);

		/// <summary>
		/// Zoom to see details of a specific pushpin on the map.
		/// </summary>
		public static readonly ZoomLevel SinglePushpinDetails = Maximum;
	}
}
