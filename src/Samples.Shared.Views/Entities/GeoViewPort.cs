using Cartography.DynamicMap;

namespace Samples.Entities
{
    public class GeoViewPort
    {
		public Cartography.DynamicMap.Geopoint Geopoint;
		public ZoomLevel ZoomLevel;

		public GeoViewPort(Cartography.DynamicMap.Geopoint geopoint, ZoomLevel zoomLevel)
		{
			Geopoint = geopoint;
			ZoomLevel = zoomLevel;
		}
    }
}
