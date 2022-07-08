using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommonServiceLocator;
using nVentive.Umbrella.Client.Commands;
using nVentive.Umbrella.Presentation.Light;
using Umbrella.Location.LocationService;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Umbrella.Location.Samples.Uno
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MapServicePage : Page
	{
		public MapServicePage()
		{
			this.InitializeComponent();

			var mapService = CreateMapService();

			ViewModelInitializer.InitializeViewModel(this, () => new MapServicePageViewModel(mapService));
		}

		private MapService.MapService CreateMapService()
		{
			var dispatcher = new CoreDispatcherScheduler(Dispatcher, Windows.UI.Core.CoreDispatcherPriority.High);

#if NETFX_CORE
			var locationService = new LocationServiceEx(dispatcher);

			return new MapService.MapService(
				dispatcher,
				() => locationService
			);
#elif __ANDROID__
			return new MapService.MapService(async ct => this.Context);
#elif __IOS__
			return new MapService.MapService(dispatcher);
#endif
		}

		public class MapServicePageViewModel : PageViewModel
		{
			private IMapService _mapService;

			public MapServicePageViewModel(IMapService mapService)
			{
				_mapService = mapService;

				Build(b => b
					.Properties(pb => pb
						.AttachCommand("ShowLocation", cb => cb.Execute(ShowLocation))
						.AttachCommand("ShowDirections", cb => cb.Execute(ShowDirections))
					)
				);
			}

			private async Task ShowLocation(CancellationToken ct)
			{
				await _mapService.ShowLocation(ct, new MapRequest(
					new GeoCoordinate(
						45.5016889,
						-73.56725599999999
					),
					"Montreal")
				);
			}

			private async Task ShowDirections(CancellationToken ct)
			{
				await _mapService.ShowDirections(ct, new MapRequest(
					new GeoCoordinate(
						45.5016889,
						-73.56725599999999
					),
					"Montreal")
				);
			}
		}
	}
}
