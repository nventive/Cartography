﻿using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using GeolocatorService;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Sample.Presentation
{
    [Bindable(true)]
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
			Status = await _geolocatorService.GetIsPermissionGranted(CancellationToken);
			CurrentLocation = await _geolocatorService.GetLocation(CancellationToken);
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
