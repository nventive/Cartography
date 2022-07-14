  using Cartography.Core;
using Cartography.StaticMap;
using Chinook.DynamicMvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Core;

namespace Samples.ViewModel
{
    class StaticMapPageViewModel : ViewModel
    {
        private static IDispatcherScheduler DispatcherScheduler;
        private MapViewPort mapViewPort;
        private string latitude;
        private string longitude;
        private double zoomLevel;
        private Size mapSize;
        private string height;
        private string width;

        public StaticMapPageViewModel(CoreDispatcher dispatcher)
        {
            MapViewPort = GetMapViewPort();
            Latitude = "45.504071";
            Longitude = "- 73.558709";
            zoomLevel = 2.0;
            MapSize = new Size(300, 300);
            Height = "300";
            Width = "300";

            DispatcherScheduler = new MainDispatcherScheduler(dispatcher, CoreDispatcherPriority.Normal);
#if WINDOWS_UWP
            StaticMapInitializer.Initialize(DispatcherScheduler, Constants.BingMaps.ApiKey);
#elif __ANDROID__ || __IOS__
            StaticMapInitializer.Initialize(DispatcherScheduler, string.Empty);
#endif
        }

        public MapViewPort MapViewPort
        {
            get { return mapViewPort; }
            set { mapViewPort = value; }
        }

        public string Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        public string Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        public double ZoomLevel
        {
            get { return zoomLevel; }
            set { zoomLevel = value; }
        }

        public Size MapSize
        {
            get { return mapSize; }
            set { mapSize = value; }
        }

        public string Height
        {
            get { return height; }
            set { height = value; }
        }

        public string Width
        {
            get { return width; }
            set { width = value; }
        }

        private MapViewPort GetMapViewPort()
        {
            var coordinate = new Geopoint(CreateGeoposition());

            var mapViewPort = new MapViewPort(coordinate);
            mapViewPort.ZoomLevel = ZoomLevels.District;

            return mapViewPort;
        }

        private BasicGeoposition CreateGeoposition(double latitude = 45.504071, double longitude = -73.558709)
        {
            return new BasicGeoposition
            {
                Latitude = latitude,
                Longitude = longitude
            };
        }

#if __Mobile__
        public IDynamicCommand ShowStaticMap => this.GetCommand(() =>
        {
            double latitude = double.Parse(Latitude, NumberStyles.Any, CultureInfo.InvariantCulture);
            double longitude = double.Parse(Longitude, NumberStyles.Any, CultureInfo.InvariantCulture);
            double height = double.Parse(Height, NumberStyles.Any, CultureInfo.InvariantCulture);
            double width = double.Parse(Width, NumberStyles.Any, CultureInfo.InvariantCulture);
            double zoomLevel = ZoomLevel;

            MapViewPort mapViewPort = new MapViewPort(new Geopoint(CreateGeoposition(latitude, longitude)));
            mapViewPort.ZoomLevel = new ZoomLevel(Math.Round(zoomLevel));

            MapSize = new Size(width, height);

            MapViewPort = (mapViewPort);
        });
#endif
    }
}
