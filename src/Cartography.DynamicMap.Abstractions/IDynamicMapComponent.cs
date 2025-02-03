using System;
using System.Collections.Generic;

namespace Cartography;

/// <summary>
/// Interface that define the required property for the MapComponent
/// </summary>
public interface IDynamicMapComponent
{
	/// <summary>
	/// Pushpins to display on map
	/// </summary>
	IGeoLocated[] Pushpins { get; set; }

	/// <summary>
	/// Selected pushpin
	/// </summary>
	IGeoLocated[] SelectedPushpins { get; set; }

	/// <summary>
	/// Groups to display on map
	/// </summary>
	IGeoLocatedGrouping<IGeoLocated[]> Groups { get; set; }

	/// <summary>
	/// Min delay to wait between two map viewport update
	/// </summary>
	TimeSpan ViewPortUpdateMinDelay { get; set; }

	/// <summary>
	/// Equality comparer to use to filter the map viewport updates
	/// </summary>
	IEqualityComparer<MapViewPort> ViewPortUpdateFilter { get; set; }

	/// <summary>
	/// Informs the view-model that an empty area of the map was tapped.
	/// </summary>
	Action<Geocoordinate> OnMapTapped { get; set; }

	/// <summary>
	/// Defines whether user tracking is enabled - READ / WRITE
	/// </summary>
	bool IsUserTrackingCurrentlyEnabled { get; set; }

	/// <summary>
	/// Whether or not the user is currently dragging the map
	/// </summary>
	bool IsUserDragging { get; set; }

    /// <summary>
    /// User location if any and if display requested, else empty value
    /// </summary>
    BasicGeoposition UserLocation { get; set; }

	/// <summary>
	/// VisibleRegion of the map - READ ONLY
	/// </summary>
	MapViewPortCoordinates ViewPortCoordinates { get; set; }

	/// <summary>
	/// ViewPort of the map - READ / WRITE - First viewport MUST be provided by VM
	/// </summary>
	MapViewPort ViewPort { get; set; }

	/// <summary>
	/// Sets the desired animation duration
	/// </summary>
	int? AnimationDurationSeconds { get; set; }
}

/// <summary>
/// The MapComponent default value for some field
/// </summary>
public static class MapComponentDefaultValue
{
	/// <summary>
	/// Get the Default ViewPortUpdateMinDelay
	/// </summary>
	public static TimeSpan DefaultViewPortUpdateMinDelay = TimeSpan.FromMilliseconds(250);

	/// <summary>
	/// Get the Default ViewPortUpdateFilter
	/// </summary>
	public static IEqualityComparer<MapViewPort> DefaultViewPortUpdateFilter =
		new PrettyMapViewPortEqualityComparer();
}
