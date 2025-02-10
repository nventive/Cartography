using System;
using System.Reactive.Concurrency;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Nventive.Persistence;
using ReviewService;

namespace Sample.Views;

/// <summary>
/// This class is used for persistence configuration.
/// - Configures the application settings.
/// </summary>
public static class PersistenceConfiguration
{
	private static IObservableDataPersister<T> CreateSecureDataPersister<T>(IServiceProvider services, T defaultValue = default(T))
	{
#if __ANDROID__
		return new SettingsStorageObservableDataPersisterAdapter<T>(
			storage: new KeyStoreSettingsStorage(
				services.GetRequiredService<ISettingsSerializer>(),
				Uno.UI.ContextHelper.Current.GetFileStreamPath(typeof(T).Name).AbsolutePath
			),
			key: typeof(T).Name,
			comparer: null,
			concurrencyProtection: false
		);
#elif __IOS__
		return new SettingsStorageObservableDataPersisterAdapter<T>(
			storage: new KeychainSettingsStorage(services.GetRequiredService<ISettingsSerializer>()),
			key: typeof(T).Name,
			comparer: null,
			concurrencyProtection: false
		);
#else
		return CreateObservableDataPersister(services, defaultValue);
#endif
	}

	private static IDataPersister<T> CreateDataPersister<T>(IServiceProvider services, T defaultValue = default(T), JsonSerializerOptions jsonSerializerOptions = null)
	{
		return UnoDataPersister.CreateFromFile<T>(
			FolderType.WorkingData,
			typeof(T).Name + ".json",
			async (ct, s) => await JsonSerializer.DeserializeAsync<T>(s, options: jsonSerializerOptions ?? services.GetService<JsonSerializerOptions>(), ct),
			async (ct, v, s) => await JsonSerializer.SerializeAsync<T>(s, v, options: jsonSerializerOptions ?? services.GetService<JsonSerializerOptions>(), ct)
		);
	}

	private static IObservableDataPersister<T> CreateObservableDataPersister<T>(IServiceProvider services, T defaultValue = default(T))
	{
		return CreateDataPersister(services, defaultValue)
			.ToObservablePersister(services.GetRequiredService<IBackgroundScheduler>());
	}
}
