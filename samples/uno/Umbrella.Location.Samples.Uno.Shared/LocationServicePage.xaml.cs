using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using nVentive.Umbrella.Client.Commands;
using nVentive.Umbrella.Concurrency;
using nVentive.Umbrella.Presentation.Light;
using Umbrella.Location.Core;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Umbrella.Location.Samples.Uno
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LocationServicePage : Page
	{
		public LocationServicePage()
		{
			this.InitializeComponent();

			// note: Given that the LocationServiceEx is recreated, instead of being re-used.
			// It is normal that the prompt is shown again upon revisiting the page, even without restarting the app.
			// This is the same for "denied", or "only in-use" (lacking background access).
			var locationService = CreateLocationService();

			ViewModelInitializer.InitializeViewModel(this, () => new LocationServicePageViewModel(locationService));
		}

		private ILocationServiceEx CreateLocationService()
		{
			var dispatcher = new CoreDispatcherScheduler(Dispatcher, Windows.UI.Core.CoreDispatcherPriority.High);

#if NETFX_CORE
			return new LocationServiceEx(dispatcher);
#elif __ANDROID__
			return new LocationServiceEx(
				observeIsApplicationActive: () => Observable.Never<bool>().StartWith(true),
				permissions: new Umbrella.Environment.Permissions.PermissionsService(
                    async ct => Droid.MainActivity.Current,
                    async ct => Droid.MainActivity.Current.PermissionResults
                ),
				resultScheduler: Schedulers.Default,
				dispatcherScheduler: dispatcher,
				startsWithLastKnownLocation: true,
				locationPrecision: GeoCoordinatePrecision.Meters,
				onlyWhenApplicationIsActive: false
			);
#elif __IOS__
			return new LocationServiceEx(Schedulers.Default, dispatcher, onlyWhileAppInUse: false);
#endif
		}

		public class LocationServicePageViewModel : PageViewModel
		{
			private ILocationServiceEx _locationService;

			public LocationServicePageViewModel(ILocationServiceEx locationService)
			{
				_locationService = locationService;

				Build(b => b
					.Properties(pb => pb
						.Attach("Status", () => _locationService.GetAndObserveStatus())
						.Attach(CurrentLocation, ct => _locationService.GetValidLocation(ct))
						.Attach("CurrentLocationAndStatus", () => _locationService.CurrentLocationAndStatus.Select(e => e))
						.AttachCommand("GetLocation", commandBuilder: cb => cb.Execute(GetLocation))
					)
				);
			}

			private IDynamicProperty<GeoPosition<GeoCoordinate>> CurrentLocation => this.GetProperty<GeoPosition<GeoCoordinate>>();

			private async Task GetLocation(CancellationToken ct)
			{
				var location = await _locationService.GetValidLocation(ct);

				CurrentLocation.Value.OnNext(location);
			}
		}
	}
}
