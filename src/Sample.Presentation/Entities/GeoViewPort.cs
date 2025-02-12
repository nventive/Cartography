using Cartography.DynamicMap;

namespace Sample.Entities;

public class GeoViewPort
{
	public Geopoint Geopoint;
	public ZoomLevel ZoomLevel;

	public GeoViewPort(Geopoint geopoint, ZoomLevel zoomLevel)
	{
		Geopoint = geopoint;
		ZoomLevel = zoomLevel;
	}
}
