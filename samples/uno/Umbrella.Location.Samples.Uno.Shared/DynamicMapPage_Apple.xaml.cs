using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using nVentive.Umbrella.Client.Commands;
using nVentive.Umbrella.Presentation.Light;
using Umbrella.Location.MapService;
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
	public sealed partial class DynamicMapPage_Apple : Page
	{
		public DynamicMapPage_Apple()
		{
			this.InitializeComponent();

			ViewModelInitializer.InitializeViewModel(this, () => new DynamicMapPageViewModel());
		}

		public class DynamicMapPageViewModel : PageViewModel
		{
			public DynamicMapPageViewModel()
			{
				Build(b => b
					.Properties(pb => pb
						.AttachCommand(nameof(NavigateToDynamicMap_FeaturesPage), cb => cb.Execute(NavigateToDynamicMap_FeaturesPage))
						.AttachCommand(nameof(NavigateToDynamicMap_MoveSearchPage), cb => cb.Execute(NavigateToDynamicMap_MoveSearchPage))
						.AttachCommand(nameof(NavigateToDynamicMap_ZoomPoiPage), cb => cb.Execute(NavigateToDynamicMap_ZoomPoiPage))
						.AttachCommand(nameof(NavigateToDynamicMap_SelectedFlipViewPage), cb => cb.Execute(NavigateToDynamicMap_SelectedFlipViewPage))
						.AttachCommand(nameof(NavigateToPretty_PushpinSelectionPage), cb => cb.Execute(NavigateToPretty_PushpinSelectionPage))
					)
				);
			}

			private Task NavigateToDynamicMap_FeaturesPage(CancellationToken ct) => NavigateTo(ct, typeof(DynamicMap_FeaturesPage));

			private Task NavigateToDynamicMap_SelectedFlipViewPage(CancellationToken ct) => NavigateTo(ct, typeof(DynamicMap_SelectedFlipViewPage));

			private Task NavigateToDynamicMap_MoveSearchPage(CancellationToken ct) => NavigateTo(ct, typeof(DynamicMap_MoveSearchPage));

			private Task NavigateToDynamicMap_ZoomPoiPage(CancellationToken ct) => NavigateTo(ct, typeof(DynamicMap_ZoomPoiPage));

			private Task NavigateToPretty_PushpinSelectionPage(CancellationToken ct) => NavigateTo(ct, typeof(Pretty_PushpinSelectionPage));
		}
	}
}
