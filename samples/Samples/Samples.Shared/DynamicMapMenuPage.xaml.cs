using GeolocatorService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Cartography.DynamicMap;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Uno;
using Uno.Extensions;
using Cartography.Core;
using Windows.Devices.Geolocation;
using Samples.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DynamicMapMenuPage : Page
    {

        public DynamicMapMenuPage()
        {
            this.InitializeComponent();
        }

        private void DynamicToMenu(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void GotoDynamicMap_FeaturesPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DynamicMap_FeaturesPage));
        }

        private void GotoDynamicMap_MoveSearchPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DynamicMap_MoveSearchPage));
        }

        private void GotoDynamicMap_SelectedFlipViewPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DynamicMap_SelectedFlipViewPage));
        }

        private void GotoDynamicMap_ZoomPoiPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DynamicMap_ZoomPoiPage));
        }
        private void GotoPretty_PushpinSelectionPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DynamicMap_Pretty_PushpinSelectionPage));
        }
    }
}
