using nVentive.Umbrella.Concurrency;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using Umbrella.Location.Core;
using Umbrella.Location.LocationService;
using Windows.UI.Core;

namespace Umbrella.Location.Samples.Uno
{
	public class LocationServiceHelper
	{
		public static ILocationServiceEx CreateLocationService(CoreDispatcher coreDispatcher)
		{
			var dispatcher = new CoreDispatcherScheduler(coreDispatcher, Windows.UI.Core.CoreDispatcherPriority.High);

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
				locationPrecision: GeoCoordinatePrecision.Meters
			);
#elif __IOS__
			return new LocationServiceEx(Schedulers.Default, dispatcher, onlyWhileAppInUse:false);
#endif
		}
	}
}
