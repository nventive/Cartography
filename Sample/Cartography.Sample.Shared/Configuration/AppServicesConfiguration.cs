using System.Reactive.Concurrency;
using GeolocatorService;
using Microsoft.Extensions.DependencyInjection;
using Cartography.MapService;

namespace Cartography.Sample
{
    /// <summary>
    /// This class is used for application services configuration.
    /// - Configures business services.
    /// - Configures platform services.
    /// </summary>
    public static class AppServicesConfiguration
    {
        /// <summary>
        /// Adds the application services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            return services
                .AddGeolocatorService()
                .AddMapService()
                .AddSingleton<IBackgroundScheduler>(s => TaskPoolScheduler.Default.ToBackgroundScheduler());
        }

        private static IServiceCollection AddGeolocatorService(this IServiceCollection services)
        {
            return services.AddSingleton<IGeolocatorService>(new GeolocatorService.GeolocatorService());
        }

        public static IServiceCollection AddMapService(this IServiceCollection services)
        {

#if __ANDROID__ || __IOS__ || WINDOWS_UWP
            services.AddSingleton<IMapService>(c =>
#if __ANDROID__
				new MapServiceAndroid(Uno.UI.ContextHelper.Current)
#elif __IOS__
				new MapServiceiOS(
						c.GetRequiredService<IDispatcherScheduler>(),
						null,
						new MapServiceiOSProvider[] { MapServiceiOSProvider.AppleMap, MapServiceiOSProvider.GoogleMap, MapServiceiOSProvider.Waze })
#else
                new MapServiceUWP(c.GetRequiredService<IDispatcherScheduler>(), c.GetRequiredService<IGeolocatorService>())
#endif
            );
#endif
            return services;
        }
    }
}
