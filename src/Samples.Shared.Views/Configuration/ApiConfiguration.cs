using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Samples.Views;

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
//-:cnd:noEmit
#if __IOS__
//+:cnd:noEmit
			new NSUrlSessionHandler()
//-:cnd:noEmit
#elif __ANDROID__
//+:cnd:noEmit
			new Xamarin.Android.Net.AndroidMessageHandler()
//-:cnd:noEmit
#else
//+:cnd:noEmit
			new HttpClientHandler()
//-:cnd:noEmit
#endif
//+:cnd:noEmit
		);
	}
}
