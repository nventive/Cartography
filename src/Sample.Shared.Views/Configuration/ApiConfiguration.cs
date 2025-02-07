using System.Net.Http;
using System.Threading.Tasks;
using Sample.DataAccess;
using MallardMessageHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Views;

public static class ApiConfiguration
{
	public static IServiceCollection AddApi(this IServiceCollection services)
	{
		return services
			.AddMainHandler()
			.AddSingleton<INetworkAvailabilityChecker>(serviceProvider =>
			{
				var connectivityProvider = serviceProvider.GetRequiredService<IConnectivityRepository>();
				return new NetworkAvailabilityChecker(_ => Task.FromResult(connectivityProvider.State is ConnectivityState.Internet));
			});
	}

	private static IServiceCollection AddMainHandler(this IServiceCollection services)
	{
		return services.AddTransient<HttpMessageHandler>(s =>
#if __IOS__
			new NSUrlSessionHandler()
#elif __ANDROID__
			new Xamarin.Android.Net.AndroidMessageHandler()
#else
			new HttpClientHandler()
#endif
		);
	}
}
