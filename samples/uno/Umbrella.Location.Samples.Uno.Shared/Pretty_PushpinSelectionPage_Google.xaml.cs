using nVentive.Umbrella.Concurrency;
using nVentive.Umbrella.Presentation.Light;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Location.Core;
using Umbrella.Location.DynamicMap;
using Umbrella.Location.LocationService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Umbrella.Location.Samples.Uno
{
	[TemplateVisualState(GroupName = "Banner", Name = BANNER_VISIBLE)]
	[TemplateVisualState(GroupName = "Banner", Name = BANNER_COLLAPSED)]
	[TemplateVisualState(GroupName = "LocationStates", Name = LOCATION_STATE_MAP)]
	[TemplateVisualState(GroupName = "LocationStates", Name = LOCATION_STATE_LIST)]
	public sealed partial class Pretty_PushpinSelectionPage_Google : Page
	{
		public const string BANNER_VISIBLE = "Visible";
		public const string BANNER_COLLAPSED = "Collapsed";

		public const string LOCATION_STATE_MAP = "Map";
		public const string LOCATION_STATE_LIST = "List";

		public Pretty_PushpinSelectionPage_Google()
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
