#if WINDOWS || __IOS__ || __ANDROID__
using Cartography.StaticMap.Provider;
using System.Reactive.Concurrency;

namespace Cartography.StaticMap;

public static class StaticMapInitializer
{
    internal static IStaticMapProvider StaticMapProvider { get; set; }

    internal static IDispatcherScheduler DispatcherScheduler { get; set; }

	/// <summary>
	/// Initialize the required element for <see cref="StaticMapControl"/>.
	/// </summary>
	/// <param name="dispatcherScheduler"><see cref="IDispatcherScheduler"/></param>
	/// <param name="bingMapKey">The BingMap Key required for UWP</param>
	/// <param name="useAppleMapProvider">Whether the app uses apple or google as a map provider </param>
	public static void Initialize(IDispatcherScheduler dispatcherScheduler, string bingMapKey, bool useAppleMapProvider = true)
    {
        DispatcherScheduler = dispatcherScheduler;

#if WINDOWS
		StaticMapProvider = new BingStaticMapProvider(bingMapKey);
#elif __ANDROID__
		StaticMapProvider = new GoogleSdkStaticMapProvider();
#elif __IOS__
		if (useAppleMapProvider)
		{
			StaticMapProvider = new AppleStaticMapProvider(dispatcherScheduler);
		} else
		{
			StaticMapProvider = new GoogleSdkStaticMapProvider();
		}
#endif
	}
}

#endif
