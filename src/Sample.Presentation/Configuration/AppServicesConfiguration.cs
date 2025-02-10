using System.Reactive.Concurrency;
using Sample.Presentation;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;

namespace Sample;

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
			.AddSingleton<IMessageDialogService, AcceptOrDefaultMessageDialogService>()
			.AddSingleton<IBackgroundScheduler>(s => TaskPoolScheduler.Default.ToBackgroundScheduler());
	}
}
