using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using nVentive.Umbrella.Presentation.Light;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Umbrella.Location.Samples.Uno
{
	[TemplateVisualState(GroupName = "Banner", Name = BANNER_VISIBLE)]
	[TemplateVisualState(GroupName = "Banner", Name = BANNER_COLLAPSED)]
	[TemplateVisualState(GroupName = "LocationStates", Name = LOCATION_STATE_MAP)]
	[TemplateVisualState(GroupName = "LocationStates", Name = LOCATION_STATE_LIST)]
	public sealed partial class Pretty_PushpinSelectionPage : Page
	{
		public const string BANNER_VISIBLE = "Visible";
		public const string BANNER_COLLAPSED = "Collapsed";

		public const string LOCATION_STATE_MAP = "Map";
		public const string LOCATION_STATE_LIST = "List";

		public Pretty_PushpinSelectionPage()
		{
			this.InitializeComponent();
			var locationService = LocationServiceHelper.CreateLocationService(Dispatcher);

			ViewModelInitializer.InitializeViewModel(
				this,
				() => new Pretty_PushpinSelectionPageViewModel(
					locationService,
					async (state) => await UpdateVisual(state)
				)
			);
		}

		private async Task UpdateVisual(string state)
		{
			await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
			{
				VisualStateManager.GoToState(this, state, false);
			});
		}
	}
}
