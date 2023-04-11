using System;
using System.Collections.Generic;
using GeolocatorService;
using Uno;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace Cartography.StaticMap
{
	/// <summary>
	/// Interface that define the required property for the MapComponent
	/// </summary>
	public interface IStaticMapComponent
	{
		/// <summary>
		/// Gets or sets the size of the static map
		/// If the MapSize is null, the static map's size will be equal to the actual StaticMapControl size.
		/// If the MapSize is not null, the static map's size will be equal to the MapSize.
		/// Note that the MapSize is subject to size constraints from the StaticMapProvider.
		/// </summary>
		Size MapSize { get; set; }

		/// <summary>
		/// ViewPort of the map - READ / WRITE - First viewport MUST be provided by VM
		/// </summary>
		StaticMapViewPort ViewPort { get; set; }

		/// <summary>
		/// Gets or sets the Pushpin
		/// </summary>
		object Pushpin { get; set; }
	}
}
