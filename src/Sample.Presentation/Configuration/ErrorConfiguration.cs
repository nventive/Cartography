using System;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using MessageDialogService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Uno.Extensions;

namespace Sample;

/// <summary>
/// This class is used for error configuration.
/// - Handles unhandled exceptions.
/// - Handles command exceptions.
/// </summary>
public static class ErrorConfiguration
{
	/// <summary>
	/// Adds the error handlers to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">Service collection.</param>
	/// <returns><see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddErrorHandling(this IServiceCollection services)
	{
		return services.AddSingleton<IDynamicCommandErrorHandler>(s => new DynamicCommandErrorHandler(
			(ct, command, exception) => HandleCommandException(ct, command, exception, s)
		));
	}

	public static void OnUnhandledException(Exception exception, bool isTerminating, IServiceProvider services)
	{
		if (exception == null)
		{
			return;
		}

		var logger = services.GetRequiredService<ILogger<CoreStartup>>();
		logger.LogError(exception, "An unhandled exception occurred. StackTrace: {StackTrace}", exception.StackTrace);
	}

	private static async Task HandleCommandException(CancellationToken ct, IDynamicCommand command, Exception exception, IServiceProvider services)
	{
		if (exception?.IsOrContainsExceptionType<OperationCanceledException>() ?? false)
		{
			return;
		}

		var messageDialogService = services.GetRequiredService<IMessageDialogService>();
		var stringLocalizer = services.GetRequiredService<IStringLocalizer>();

		var titleResourceKey = string.Empty;
		var bodyResourceKey = string.Empty;

		var title = stringLocalizer[titleResourceKey];
		var body = stringLocalizer[bodyResourceKey];

		title = title.ResourceNotFound ? stringLocalizer["Default_Error_DialogTitle"] : title;
		body = body.ResourceNotFound ? stringLocalizer["Default_Error_DialogBody"] : body;

		await messageDialogService.ShowMessage(ct, m => m
			.Title(title)
			.Content(body)
		);
	}
}
