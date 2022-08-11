using System;
using System.Collections.Generic;
using System.Text;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Microsoft.Extensions.DependencyInjection;

namespace Samples
{
	/// <summary>
	/// This class is used for navigation configuration.
	/// - Configures the navigator.
	/// </summary>
	public static class NavigationCoreConfiguration
	{
		/// <summary>
		/// Adds the core navigation services to the <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">Service collection.</param>
		/// <returns><see cref="IServiceCollection"/>.</returns>
		public static IServiceCollection AddNavigationCore(this IServiceCollection services)
		{
#if NETFRAMEWORK
			services.AddSingleton<ISectionsNavigator>(s => new BlindSectionsNavigator("Home", "Posts", "Settings"));
#endif
			return services
				.AddSingleton<IStackNavigator>(s => new SectionsNavigatorToStackNavigatorAdapter(s.GetService<ISectionsNavigator>()));
		}
	}
}
