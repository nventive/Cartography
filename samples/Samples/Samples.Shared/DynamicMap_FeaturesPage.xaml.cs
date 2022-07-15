using Samples.ViewModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DynamicMap_FeaturesPage : Page
    {
        private DynamicMap_FeaturesPageViewModel DynamicMap_FeaturesPageViewModel;
        public DynamicMap_FeaturesPage()
        {
            DynamicMap_FeaturesPageViewModel = new DynamicMap_FeaturesPageViewModel();
            DataContext = DynamicMap_FeaturesPageViewModel;
            this.InitializeComponent();
        }

        private void FeatureToDynamicMapMenu(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DynamicMapMenuPage));
        }
    }
}
