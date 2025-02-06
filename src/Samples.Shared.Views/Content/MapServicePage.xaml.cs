﻿// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapServicePage : Page
    {
        public MapServicePage()
        {
            this.InitializeComponent();
        }

        private void MapServiceToMenu(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
