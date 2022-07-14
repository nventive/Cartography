using Chinook.DynamicMvvm;
using Chinook.StackNavigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private void GotoMapService(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MapServicePage));
        }
        private void GotoLocation(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LocationPage));
        }
        private void GotoDynamicMap(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DynamicMapMenuPage));
        }

        private void GotoStaticMap(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StaticMapPage));
        }


    }
}
