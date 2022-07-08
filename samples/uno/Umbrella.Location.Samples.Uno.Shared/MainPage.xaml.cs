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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Umbrella.Location.Samples.Uno
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();

			ViewModelInitializer.InitializeViewModel(this, () => new MainPageViewModel());
		}

		public class MainPageViewModel : PageViewModel
		{
			public MainPageViewModel()
			{
				Build(b => b
					.Properties(pb => pb
						.AttachCommand("NavigateToMapServicePage", cb => cb.Execute(NavigateToMapServicePage))
						.AttachCommand("NavigateToLocationServicePage", cb => cb.Execute(NavigateToLocationServicePage))
						.AttachCommand("NavigateToDynamicMapPage", cb => cb.Execute(NavigateToDynamicMapPage))
						.AttachCommand("NavigateToStaticMapPage", cb => cb.Execute(NavigateToStaticMapPage))
					)
				);
			}

			private Task NavigateToMapServicePage(CancellationToken ct) => NavigateTo(ct, typeof(MapServicePage));

			private Task NavigateToLocationServicePage(CancellationToken ct) => NavigateTo(ct, typeof(LocationServicePage));

			private Task NavigateToDynamicMapPage(CancellationToken ct) => NavigateTo(ct, typeof(ImplementationSelectionPage));

			private Task NavigateToStaticMapPage(CancellationToken ct) => NavigateTo(ct, typeof(StaticMapPage));
		}
	}
}
