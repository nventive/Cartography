#if WINDOWS_UWP || __IOS__ || __ANDROID__
using System.Reactive.Concurrency;
using Cartography.StaticMap.Provider;

namespace Cartography.StaticMap
{
	public static class StaticMapInitializer
	{
		internal static IStaticMapProvider StaticMapProvider { get; set; }

		internal static IDispatcherScheduler DispatcherScheduler { get; set; }

		/// <summary>
		/// Initialize the required element for <see cref="StaticMapControl"/>.
		/// </summary>
		/// <param name="dispatcherScheduler"><see cref="IDispatcherScheduler"/></param>
		/// <param name="bingMapKey">The BingMap Key required for UWP</param>
		public static void Initialize(IDispatcherScheduler dispatcherScheduler, string bingMapKey)
		{
			DispatcherScheduler = dispatcherScheduler;

#if WINDOWS_UWP
			StaticMapProvider = new BingStaticMapProvider(bingMapKey);
#elif __ANDROID__
			StaticMapProvider = new GoogleSdkStaticMapProvider();
#elif __IOS__
			StaticMapProvider = new AppleStaticMapProvider(dispatcherScheduler);
#endif
		}
	}
}

#endif
