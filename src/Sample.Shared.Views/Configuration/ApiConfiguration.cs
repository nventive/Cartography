using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Views;

public static class ApiConfiguration
{
	public static IServiceCollection AddApi(this IServiceCollection services)
	{
		return services
			.AddMainHandler();
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
