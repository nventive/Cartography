using Cartography.StaticMap;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using System;
using System.Drawing;
using System.Globalization;
using System.Reactive.Concurrency;

namespace Sample.Presentation
{
	public partial class StaticMapPageViewModel : ViewModel, IStaticMapComponent
	{
        private ISectionsNavigator _sectionsNavigator;
        private readonly IDispatcherScheduler _dispatcherScheduler;

        public StaticMapPageViewModel()
        {
            _sectionsNavigator = this.GetService<ISectionsNavigator>();
            _dispatcherScheduler = this.GetService<IDispatcherScheduler>();

		}

        public StaticMapViewPort ViewPort
        {
            get => this.Get<StaticMapViewPort>( initialValue: GetMapViewPort());
            set => this.Set(value);
        }
        
        public Size MapSize
        {
            get => this.Get<Size>(initialValue: new Size(300, 300));
            set => this.Set(value);
        }

        public object Pushpin
        {
            get => this.Get<object>();
            set => this.Set(value);
        }

        public string Latitude
        {
            get => this.Get<string>(initialValue: "45.504071");
            set => this.Set(value);
        }

        public string Longitude
        {
            get => this.Get<string>(initialValue: "-73.558709");
            set => this.Set(value);
        }

        public double ZoomLevel
        {
            get => this.Get<double>(initialValue: 12.0);
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

        public IDynamicCommand StaticToMenu => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new MainPageViewModel());
        });

        private StaticMapViewPort GetMapViewPort()
        {
            var coordinate = new Geopoint(CreateGeoposition());

            var viewPort = new StaticMapViewPort(coordinate);
            viewPort.ZoomLevel = ZoomLevels.District;

            return viewPort;
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
            var height = int.Parse(Height, NumberStyles.Any, CultureInfo.InvariantCulture);
            height = Math.Min(height, 400);
            Height = height.ToString();
            var width = int.Parse(Width, NumberStyles.Any, CultureInfo.InvariantCulture);
            width = Math.Min(width, 400);
            Width = width.ToString();
            double zoomLevel = ZoomLevel;

            StaticMapViewPort viewPort = new StaticMapViewPort(new Geopoint(CreateGeoposition(latitude, longitude)));
            viewPort.ZoomLevel = new ZoomLevel(Math.Round(zoomLevel));

            MapSize = new Size(width, height);

            ViewPort = (viewPort);
        });

        
    }
}
