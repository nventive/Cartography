using System;
using System.Globalization;
using System.Net.Http;
using MallardMessageHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Uno.Extensions;

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
			.AddMainHandler()
			.AddExceptionHubHandler();

		return services;
	}

	private static IServiceCollection AddMainHandler(this IServiceCollection services)
	{
		return services.AddTransient<HttpMessageHandler, HttpClientHandler>();
	}


	private static IServiceCollection AddExceptionHubHandler(this IServiceCollection services)
	{
		return services
			.AddSingleton<IExceptionHub>(new ExceptionHub())
			.AddTransient<ExceptionHubHandler>();
	}

	private static void AddDefaultHeaders(HttpClient client, IServiceProvider serviceProvider)
	{
		client.DefaultRequestHeaders.Add("Accept-Language", CultureInfo.CurrentCulture.Name);
		client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "DadJokesApp/1.0.0");
	}

	/// <summary>
	/// Adds a Refit client to the service collection.
	/// </summary>
	/// <typeparam name="T">The type of the Refit interface.</typeparam>
	/// <param name="services">The service collection.</param>
	/// <param name="settings">Optional. The settings to configure the instance with.</param>
	/// <returns>The updated IHttpClientBuilder.</returns>
	private static IHttpClientBuilder AddRefitHttpClient<T>(this IServiceCollection services, Func<IServiceProvider, RefitSettings> settings = null)
		where T : class
	{
		services.AddSingleton(serviceProvider => RequestBuilder.ForType<T>(settings?.Invoke(serviceProvider)));

		return services
			.AddHttpClient(typeof(T).FullName)
			.AddTypedClient((client, serviceProvider) => RestService.For(client, serviceProvider.GetService<IRequestBuilder<T>>()));
	}
}
