using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chinook.DynamicMvvm;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cartography.Sample
{
	/// <summary>
	/// This class is used for view model configuration.
	/// - Configures the dynamic properties.
	/// - Configures the dynamic commands.
	/// - Configures the data loaders.
	/// </summary>
	public static class ViewModelConfiguration
	{
		/// <summary>
		/// Adds the mvvm services to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">Service collection.</param>
		/// <returns><see cref="IServiceCollection"/>.</returns>
		public static IServiceCollection AddMvvm(this IServiceCollection services)
		{
			return services
				.AddDynamicProperties()
				.AddDynamicCommands();
		}

		private static IServiceCollection AddDynamicCommands(this IServiceCollection services)
		{
			return services
				.AddSingleton<IDynamicCommandBuilderFactory>(s =>
					new DynamicCommandBuilderFactory(c => c
						.WithLogs(s.GetRequiredService<ILogger<IDynamicCommand>>())
						.DisableWhileExecuting()
						.OnBackgroundThread()
						.CancelPrevious()
					)
				);
		}

		private static IServiceCollection AddDynamicProperties(this IServiceCollection services)
		{
			return services.AddSingleton<IDynamicPropertyFactory, DynamicPropertyFactory>();
		}
	}
}
