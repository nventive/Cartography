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
	public sealed partial class ImplementationSelectionPage : Page
	{
		public ImplementationSelectionPage()
		{
			this.InitializeComponent();

			ViewModelInitializer.InitializeViewModel(this, () => new ImplementationSelectionPageViewModel());
		}

		public class ImplementationSelectionPageViewModel : PageViewModel
		{
			public ImplementationSelectionPageViewModel()
			{
				Build(b => b
					.Properties(pb => pb
						.AttachCommand(nameof(NavigateToGoogleSamplesPage), cb => cb.Execute(NavigateToGoogleSamplesPage))
						.AttachCommand(nameof(NavigateToAppleSamplesPage), cb => cb.Execute(NavigateToAppleSamplesPage))
						.AttachCommand(nameof(NavigateToUwpSamplesPage), cb => cb.Execute(NavigateToUwpSamplesPage))
					)
				);
			}

			private Task NavigateToGoogleSamplesPage(CancellationToken ct) => NavigateTo(ct, typeof(DynamicMapPage_Google));

			private Task NavigateToAppleSamplesPage(CancellationToken ct) => NavigateTo(ct, typeof(DynamicMapPage_Apple));

			private Task NavigateToUwpSamplesPage(CancellationToken ct) => NavigateTo(ct, typeof(DynamicMapPage_Uwp));
		}
	}
}
