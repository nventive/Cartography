using System.Reactive.Concurrency;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.UI.Dispatching;
using ReviewService;
using Cartography.MapService;
using GeolocatorService;

namespace Sample.Views;

/// <summary>
/// This class is used for view services.
/// - Configures view services.
/// </summary>
public static class ViewServicesConfiguration
{
	public static IServiceCollection AddViewServices(this IServiceCollection services)
	{
		return services
			.AddSingleton(s => App.Instance.NavigationMultiFrame.DispatcherQueue)
			.AddSingleton(s => Shell.Instance.ExtendedSplashScreen)
			.AddSingleton<IDispatcherScheduler>(s => new MainDispatcherScheduler(
				s.GetRequiredService<DispatcherQueue>(),
				DispatcherQueuePriority.Normal
			))
			.AddSingleton<IDispatcherFactory, DispatcherFactory>()
			.AddSingleton<ILauncherService>(s => new LauncherService(s.GetRequiredService<DispatcherQueue>()))
			.AddSingleton<IVersionProvider, VersionProvider>()
			.AddSingleton<IDeviceInformationProvider, DeviceInformationProvider>()
			.AddSingleton<IExtendedSplashscreenController, ExtendedSplashscreenController>(s => new ExtendedSplashscreenController(Shell.Instance.DispatcherQueue))
			.AddSingleton<IMemoryProvider, MemoryProvider>()
			.AddSingleton<IReviewPrompter, ReviewPrompter>()
			.AddMapService()
			.AddGeolocatorService()
			.AddMessageDialog();
	}

	private static IServiceCollection AddMessageDialog(this IServiceCollection services)
	{
		return services.AddSingleton<IMessageDialogService>(s =>
#if __WINDOWS__ || __IOS__ || __ANDROID__
			new MessageDialogService.MessageDialogService(
				s.GetRequiredService<DispatcherQueue>(),
#if __WINDOWS__
				new MessageDialogBuilderDelegate(
					key => s.GetRequiredService<IStringLocalizer>()[key],
					WinRT.Interop.WindowNative.GetWindowHandle(App.Instance.CurrentWindow)
				)
#else
				new MessageDialogBuilderDelegate(
					key => s.GetRequiredService<IStringLocalizer>()[key]
				)
#endif
			)
#else
			new AcceptOrDefaultMessageDialogService()
#endif
		);
	}

	private static IServiceCollection AddGeolocatorService(this IServiceCollection services)
	{
		return services.AddSingleton<IGeolocatorService>(new GeolocatorService.GeolocatorService());
	}

	public static IServiceCollection AddMapService(this IServiceCollection services)
	{
#if __ANDROID__ || __IOS__ || WINDOWS
		services.AddSingleton<IMapService>(c =>
#if __ANDROID__
			new MapServiceAndroid(Uno.UI.ContextHelper.Current));
#elif __IOS__
			new MapServiceiOS(
					c.GetRequiredService<IDispatcherScheduler>(),
					null,
					[MapServiceiOSProvider.AppleMap, MapServiceiOSProvider.GoogleMap, MapServiceiOSProvider.Waze]));
#elif WINDOWS
			new MapServiceWindows());
#endif
#endif
		return services;
	}
}
