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

namespace Samples.ViewModel
{
    class StaticMapPageViewModel : ViewModel
    {
        private static IDispatcherScheduler DispatcherScheduler = DispatcherScheduler.ToDispatcherScheduler();       
        public StaticMapPageViewModel()
        {

#if WINDOWS_UWP
            StaticMapInitializer.Initialize(DispatcherScheduler, Constants.BingMaps.ApiKey);
#elif __ANDROID__ || __IOS__
            StaticMapInitializer.Initialize(this.GetService<IDispatcherScheduler>(), string.Empty);
#endif
        }

        public MapViewPort MapViewPort
        {
            get => this.Get<MapViewPort>(initialValue: GetMapViewPort());
            set => this.Set(value);
        }

        public string Latitude
        {
            get => this.Get<string>(initialValue: "45.504071");
            set => this.Set(value);
        }

        public string Longitude
        {
            get => this.Get<string>(initialValue: "- 73.558709");
            set => this.Set(value);
        }

        public double ZoomLevel
        {
            get => this.Get<double>(initialValue: 12.0);
            set => this.Set(value);
        }

        public Size MapSize
        {
            get => this.Get<Size>(initialValue: new Size(300, 300));
            set => this.Set(value);
        }

        public string Height
        {
            get => this.Get<string>(initialValue: "300");
            set => this.Set(value);
        }

        public string Width
        {
            get => this.Get<string>(initialValue: "300");
            set => this.Set(value);
        }

        private MapViewPort GetMapViewPort()
        {
            var coordinate = new Geopoint(CreateGeoposition());

            var mapViewPort = new MapViewPort(coordinate);
            mapViewPort.ZoomLevel = ZoomLevels.District;
            mapViewPort.Center = coordinate;

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
    }
}
