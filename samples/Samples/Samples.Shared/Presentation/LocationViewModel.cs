using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using GeolocatorService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Samples.Presentation
{
    [Windows.UI.Xaml.Data.Bindable]
    public partial class LocationViewModel : ViewModel
    {
        private readonly ISectionsNavigator _sectionsNavigator;
        private IGeolocatorService _geolocatorService;

        /*This is only to show available data in GeoLocatorService.
         * Depending on your phone, you may encounter deferents results.
         * Latitude and Longitude are the only stable data from phone to phone.
         */
        public LocationViewModel()
        {
            _sectionsNavigator = this.GetService<ISectionsNavigator>();
            _geolocatorService = this.GetService<IGeolocatorService>();
        }

        public Geocoordinate CurrentLocation
        {
            get => this.Get<Geocoordinate>();
            set => this.Set(value);
        }

        public bool Status
        {
            get => this.Get<bool>();
            set => this.Set(value);
        }

        private async Task getCurrentLocation()
        {
            CurrentLocation = await _geolocatorService.GetLocation(CancellationToken);
            Status = await _geolocatorService.GetIsPermissionGranted(CancellationToken);
        }

        public IDynamicCommand NavigateToMenu => this.GetCommandFromTask(async ct =>
        {   
            await _sectionsNavigator.Navigate(ct, () => new MainPageViewModel());
        });

        public IDynamicCommand GetLocation => this.GetCommandFromTask(async ct =>
        {
            await getCurrentLocation();
        });
    }
}
