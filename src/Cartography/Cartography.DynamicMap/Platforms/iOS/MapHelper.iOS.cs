#if __IOS__
using System;
using System.Collections.Generic;
using System.Text;
using CoreGraphics;
using CoreLocation;
using MapKit;
using Windows.Devices.Geolocation;

namespace Cartography.DynamicMap
{
	public static class MapHelper
	{
		private const double MercatorRadius = 85445659.44705395;
		private const double MercatorOffset = 268435456;

		public static MKCoordinateRegion CreateRegion(BasicGeoposition centerCoordinate, ZoomLevel zoomLevel, CGSize size)
		{
			// convert center coordiate to pixel space 
			double centerPixelX = LongitudeToPixelSpaceX(centerCoordinate.Longitude);
			double centerPixelY = LatitudeToPixelSpaceY(centerCoordinate.Latitude);

			// determine the scale value from the zoom level 
			var zoomExponent = 20 - zoomLevel.Value;
			double zoomScale = Math.Pow(2, zoomExponent);

			// scale the map’s size in pixel space 
			var mapSizeInPixels = size;
			double scaledMapWidth = mapSizeInPixels.Width * zoomScale;
			double scaledMapHeight = mapSizeInPixels.Height * zoomScale;

			// figure out the position of the top-left pixel 
			double topLeftPixelX = centerPixelX - (scaledMapWidth / 2);
			double topLeftPixelY = centerPixelY - (scaledMapHeight / 2);

			// find delta between left and right longitudes 
			var minLng = PixelSpaceXToLongitude(topLeftPixelX);
			var maxLng = PixelSpaceXToLongitude(topLeftPixelX + scaledMapWidth);
			var longitudeDelta = maxLng - minLng;

			// find delta between top and bottom latitudes 
			var minLat = PixelSpaceYToLatitude(topLeftPixelY);
			var maxLat = PixelSpaceYToLatitude(topLeftPixelY + scaledMapHeight);
			var latitudeDelta = -1 * (maxLat - minLat);

			// create and return the lat/lng span 
			var span = new MKCoordinateSpan(latitudeDelta, longitudeDelta);
			var center = new CLLocationCoordinate2D();
			center.Latitude = centerCoordinate.Latitude;
			center.Longitude = centerCoordinate.Longitude;
			var region = new MKCoordinateRegion(center, span);

			return region;
		}

		public static double PixelSpaceXToLongitude(double pixelX)
		{
			return ((Math.Round(pixelX) - MercatorOffset) / MercatorRadius) * 180.0 / Math.PI;
		}

		public static double PixelSpaceYToLatitude(double pixelY)
		{
			return (Math.PI / 2.0 - 2.0 * Math.Atan(Math.Exp((Math.Round(pixelY) - MercatorOffset) / MercatorRadius))) * 180.0 / Math.PI;
		}

		public static double LongitudeToPixelSpaceX(double longitude)
		{
			return Math.Round(MercatorOffset + MercatorRadius * longitude * Math.PI / 180.0);
		}

		public static double LatitudeToPixelSpaceY(double latitude)
		{
			if (latitude == 90.0)
			{
				return 0;
			}
			else if (latitude == -90.0)
			{
				return MercatorOffset * 2;
			}
			else
			{
				return Math.Round(MercatorOffset - MercatorRadius * Math.Log((1 + Math.Sin(latitude * Math.PI / 180.0)) / (1 - Math.Sin(latitude * Math.PI / 180.0))) / 2.0);
			}
		}
	}
}
#endif
