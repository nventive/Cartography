using Uno.Extensions;
using Windows.Devices.Geolocation;

namespace Cartography.StaticMap;

/// <summary>
/// Represents all information about the view port of a map.
/// </summary>
public class StaticMapViewPort
{
	private const int MAXIMUM_LONGITUDE = 180;
	private const int MINIMUM_LONGITUDE = -180;
	private const int MAXIMUM_LATITUDE = 90;

	/// <summary>
	/// Default ctor
	/// </summary>
	public StaticMapViewPort()
	{
	}

	/// <summary>
	/// Instanciate a MapViewPort with its center
	/// </summary>
	/// <param name="center">The center</param>
	public StaticMapViewPort(Geopoint center)
	{
		Center = center;
	}

	/// <summary>
	/// Copy constructor
	/// </summary>
	/// <param name="source"></param>
	public StaticMapViewPort(StaticMapViewPort source)
	{
		Center = source.Center;
		ZoomLevel = source.ZoomLevel;
	}

	/// <summary>
	/// The center of the map.
	/// </summary>
	public Geopoint Center { get; set; }

	/// <summary>
	/// The zoom level to be set on the MapControl. This property should not be set, or should be ignored if <see cref="PointsOfInterest"/> is set.
	/// Please see <see cref="ZoomLevels"/> for more information.
	/// </summary>
	public ZoomLevel? ZoomLevel { get; set; }

	/// <summary>
	/// Defines an optional list of Points of Interest to be used by the MapControl to change the displayed map viewport ( auto zoom ).
	/// </summary>
	public Geopoint[] PointsOfInterest { get; set; }

	/// <inherit />
	//According to original dev, there was a problem in using the EqualityExtension. But there is no
	//traces why, and he can't recall.
	public override int GetHashCode()
	{
		return Center.GetHashCode()
			^ ZoomLevel.GetHashCode()
			^ this.Equality().GetHashCode(PointsOfInterest.Safe());
	}

	/// <inherit />
	//According to original dev, there was a problem in using the EqualityExtension. But there is no
	//traces why, and he can't recall.
	public override bool Equals(object obj)
	{
		var other = obj as StaticMapViewPort;

		return other != null
			&& Center.Equals(other.Center)
			&& ZoomLevel.GetValueOrDefault((ZoomLevel)double.NaN).Equals(other.ZoomLevel.GetValueOrDefault((ZoomLevel)double.NaN))
			&& PointsOfInterest.SafeSequenceEqual(other.PointsOfInterest);
	}

	public override string ToString()
	{
		return "[MapViewPort] Center: {0}, Zoom: {1}, POIs: {2}".InvariantCultureFormat(
			Center,
			ZoomLevel,
			string.Join(",", PointsOfInterest.Safe()));
	}
}
