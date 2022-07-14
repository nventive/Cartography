using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using GeolocatorService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocationPage : Page
    {
        private readonly IGeolocatorService _geolocatorService;
        private Geocoordinate currentLocation;
        private bool status;

        public LocationPage()
        {
            _geolocatorService = new GeolocatorService.GeolocatorService();
            _geolocatorService.RequestPermission(CancellationToken.None);
            this.InitializeComponent();
        }


        public Geocoordinate CurrentLocation
        {
            get { return currentLocation; }
            set { currentLocation = value; }
        }

        public bool Status
        {
            get { return status; }
            set { status = value; }
        }

        private async void getCurrentLocation()
        {
            CurrentLocation = await _geolocatorService.GetLocation(CancellationToken.None);
            txtAccuracy.Text = CurrentLocation.Accuracy.ToString();
            txtAltitude.Text = CurrentLocation.Point.Position.Altitude.ToString();
            txtLatitude.Text = CurrentLocation.Point.Position.Latitude.ToString();
            txtLongitude.Text = CurrentLocation.Point.Position.Longitude.ToString();
            txtSpeed.Text = CurrentLocation.Speed.ToString();
            txtHeading.Text = CurrentLocation.Heading.ToString();
            Status = await _geolocatorService.GetIsPermissionGranted(CancellationToken.None);
        }

        private void LocationToMenu(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void GetLocation(object sender, RoutedEventArgs e)
        {
            getCurrentLocation();
        }
    }
}
