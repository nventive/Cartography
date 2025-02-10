﻿using System.Reactive.Concurrency;
using Sample.Business;
using Sample.DataAccess;
using Sample.Presentation;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Cartography.MapService;

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
			.AddSingleton<IConnectivityRepository, MockedConnectivityRepository>()
			.AddSingleton<IBackgroundScheduler>(s => TaskPoolScheduler.Default.ToBackgroundScheduler())
			.AddSingleton<IApplicationSettingsRepository, ApplicationSettingsRepository>()
			.AddSingleton<IPostService, PostService>()
			.AddSingleton<IDadJokesService, DadJokesService>()
			.AddSingleton<IAuthenticationService, AuthenticationService>()
			.AddSingleton<IUserProfileService, UserProfileService>()
			.AddSingleton<IUpdateRequiredService, UpdateRequiredService>()
			.AddSingleton<IKillSwitchService, KillSwitchService>()
			.AddSingleton<DiagnosticsCountersService>();
	}
}
