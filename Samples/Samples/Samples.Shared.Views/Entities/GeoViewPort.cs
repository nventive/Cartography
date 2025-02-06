using Cartography.DynamicMap;
using Windows.Devices.Geolocation;

namespace Samples.Entities
{
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
}
