using Umbrella.Location.LocationService;
using Windows.UI.Xaml.Controls;

namespace Umbrella.Location.Samples.Uno
{
	public sealed partial class DynamicMap_FeaturesPage_Google : Page
	{
		public DynamicMap_FeaturesPage_Google()
		{
			this.InitializeComponent();

			var locationService = LocationServiceHelper.CreateLocationService(Dispatcher);

			ViewModelInitializer.InitializeViewModel(this, () => new DynamicMap_FeaturesPageViewModel(locationService));
		}
	}
}
