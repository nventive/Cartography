using System;
using System.Collections.Generic;
using System.Text;
using Cartography.Core;
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
