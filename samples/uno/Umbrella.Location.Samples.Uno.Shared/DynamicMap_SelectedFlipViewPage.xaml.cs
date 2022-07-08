using Umbrella.Location.LocationService;
using Windows.UI.Xaml.Controls;

namespace Umbrella.Location.Samples.Uno
{
	public sealed partial class DynamicMap_SelectedFlipViewPage : Page
	{
		public DynamicMap_SelectedFlipViewPage()
		{
			this.InitializeComponent();

			var locationService = LocationServiceHelper.CreateLocationService(Dispatcher);

			ViewModelInitializer.InitializeViewModel(this, () => new DynamicMap_SelectedFlipViewPageViewModel(locationService));
		}
	}
}
