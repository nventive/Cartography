using System.Reactive.Concurrency;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using ReviewService;
using Samples;

namespace Samples.Views;

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
			.AddSingleton<IDispatcherScheduler>(s => new MainDispatcherScheduler(
				s.GetRequiredService<DispatcherQueue>(),
				DispatcherQueuePriority.Normal
			))
			.AddSingleton<IDispatcherFactory, DispatcherFactory>()
			.AddSingleton<IReviewPrompter, ReviewPrompter>();
	}
}
