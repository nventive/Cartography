using Umbrella.Location.LocationService;
using Windows.UI.Xaml.Controls;

namespace Umbrella.Location.Samples.Uno
{
	public sealed partial class DynamicMap_MoveSearchPage : Page
	{
		public DynamicMap_MoveSearchPage()
		{
			this.InitializeComponent();

			var locationService = LocationServiceHelper.CreateLocationService(Dispatcher);

			ViewModelInitializer.InitializeViewModel(this, () => new DynamicMap_MoveSearchPageViewModel(locationService));
		}
	}
}
