using Windows.Devices.Geolocation;

namespace Cartography.MapService;

/// <summary>
/// This class aggregates map parameters.
/// </summary>
public class MapRequest
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MapRequest"/> class.
	/// </summary>
	/// <param name="coordinates">Coordinates</param>
	/// <param name="label">Label</param>
	public MapRequest(BasicGeoposition coordinates, string label = null)
	{
		Coordinates = coordinates;
		IsCoordinatesSet = true;
		Label = label;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MapRequest"/> class.
	/// </summary>
	/// <param name="locationName">LocationName</param>
	public MapRequest(string locationName)
	{
		LocationName = locationName;
	}

	/// <summary>
	/// Gets the coordinates.
	/// </summary>
	public BasicGeoposition Coordinates { get; }

	internal bool IsCoordinatesSet { get; }

	/// <summary>
	/// Gets the label.
	/// </summary>
	public string Label { get; }

	/// <summary>
	/// Gets the location name.
	/// </summary>
	public string LocationName { get; }
}
