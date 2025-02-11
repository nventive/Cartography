using System;
using System.Globalization;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sample;

/// <summary>
/// This class is used for API configuration.
/// - Configures API clients.
/// - Configures HTTP handlers.
/// </summary>
public static class ApiConfiguration
{
	/// <summary>
	/// Adds the API services to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/>.</param>
	/// <param name="configuration">The <see cref="IConfiguration"/>.</param>
	/// <returns>The updated <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
	{
		// TODO: Configure your HTTP clients here.

		services
			.AddMainHandler();

		return services;
	}

	private static IServiceCollection AddMainHandler(this IServiceCollection services)
	{
		return services.AddTransient<HttpMessageHandler, HttpClientHandler>();
	}

	private static void AddDefaultHeaders(HttpClient client, IServiceProvider serviceProvider)
	{
		client.DefaultRequestHeaders.Add("Accept-Language", CultureInfo.CurrentCulture.Name);
		client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "DadJokesApp/1.0.0");
	}
}
