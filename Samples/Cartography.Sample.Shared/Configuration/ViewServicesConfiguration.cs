using System.Reactive.Concurrency;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Core;

namespace Cartography.Sample.Views
{
	/// <summary>
	/// This class is used for view services.
	/// - Configures view services.
	/// </summary>
	public static class ViewServicesConfiguration
	{
		public static IServiceCollection AddViewServices(this IServiceCollection services)
		{
			return services
				.AddSingleton(s => App.Instance.NavigationMultiFrame.Dispatcher)
				.AddSingleton<IDispatcherScheduler>(s => new MainDispatcherScheduler(
					s.GetRequiredService<CoreDispatcher>(),
					CoreDispatcherPriority.Normal
				));
		}

	}
}
