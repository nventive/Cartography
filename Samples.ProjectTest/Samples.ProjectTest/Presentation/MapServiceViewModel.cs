using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Cartography.MapService;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace Samples.ProjectTest.Presentation
{
    public partial class MapServiceViewModel : ViewModel
    {

        private ISectionsNavigator _sectionsNavigator;
        private IMapService _mapService;

        public MapServiceViewModel()
        {
            _sectionsNavigator = this.GetService<ISectionsNavigator>();
            _mapService = this.GetService<IMapService>();
        }

        public IDynamicCommand MapServiceToMenu => this.GetCommandFromTask(async ct =>
        {
            await _sectionsNavigator.Navigate(ct, () => new MainPageViewModel());
        });

        // Open the phone map application on the device. Show the location.
        public IDynamicCommand ShowLocation => this.GetCommandFromTask(async ct =>
        {
            await _mapService.ShowLocation(ct, new MapRequest(
                new BasicGeoposition()
                {
                    Latitude = 45.5016889,
                    Longitude = -73.56725599999999
                },
                "Montreal"
                ));
        });

        // Open the phone map application on the device. Create direction from user position to the location.
        public IDynamicCommand ShowDirections => this.GetCommandFromTask(async ct =>
        {
            await _mapService.ShowDirections(ct, new MapRequest(
                new BasicGeoposition()
                {
                    Latitude = 45.5016889,
                    Longitude = -73.56725599999999,
                },
                "Montreal"
            ));
        });
    }
}
