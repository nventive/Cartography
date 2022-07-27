using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace Samples.Views
{
	public static class ViewConfiguration
	{
		/// <summary>
		/// Adds the services to the <see cref="IHostBuilder"/>.
		/// </summary>
		/// <param name="hostBuilder">Host builder.</param>
		/// <returns><see cref="IHostBuilder"/>.</returns>
		public static IHostBuilder AddViewServices(this IHostBuilder hostBuilder)
		{
			if (hostBuilder is null)
			{
				throw new ArgumentNullException(nameof(hostBuilder));
			}

			return hostBuilder
				.ConfigureServices(s => s
					.AddMvvm()
					.AddNavigation()
					.AddViewServices()
				);
		}
	}
}
