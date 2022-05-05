using Cartography.Core;

namespace Cartography.StaticMap.Provider
{
	/// <summary>
	/// Parameters to get a map from a StaticMapProvider.
	/// </summary>
	internal class StaticMapParameters
	{
		//private static readonly Func<StaticMapParameters, IEnumerable<object>> Fields = obj => new object[] { obj.Scale, obj.Width, obj.Height, obj.ViewPort };

		/// <summary>
		/// Gets or sets the scale factor of the device.
		/// </summary>
		public double Scale { get; set; }

		/// <summary>
		/// Gets or sets the width of the map.
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// Gets or sets the height of the map.
		/// </summary>
		public int Height { get; set; }

		//TODO
		/// <summary>
		/// Gets or sets the viewport of the map.
		/// </summary>
		public MapViewPort ViewPort { get; set; }
	}
}
