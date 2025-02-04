#if __IOS__
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using Microsoft.Extensions.Logging;
using Cartography.MapService.Provider;
using UIKit;
using Uno.Extensions;
using Uno.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cartography.MapService;

/// <summary>
/// Implementation of <see href="IMapService" />
/// </summary>
public class MapServiceiOS : IMapService
{
	private readonly IDispatcherScheduler _dispatcherScheduler;
	private readonly IMapServiceProvider[] _mapServiceProviders;
	private readonly MapServiceTextProvider _mapServiceTextProvider;
	private readonly ILogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="MapServiceiOS"/> class.
	/// </summary>
	/// <param name="dispatcherScheduler">Dispatcher scheduler</param>
	/// <param name="mapServiceTextProvider">Text provider, if null the default ios providor will be use</param>
	/// <param name="mapServiceProviders">MapServiceProvides for iOS</param>
	/// /// <param name="logger">logger</param>
	public MapServiceiOS(
		IDispatcherScheduler dispatcherScheduler,
		MapServiceTextProvider mapServiceTextProvider = null,
		MapServiceiOSProvider[] mapServiceProviders = null,
		ILogger logger = null
	)
	{
		_dispatcherScheduler = dispatcherScheduler;
		_mapServiceTextProvider = mapServiceTextProvider;
		_mapServiceProviders = MapServiceProviderFactory.Create(mapServiceProviders);
		_logger = logger ?? NullLogger.Instance;
	}

	/// <inheritdoc/>
	public async Task ShowDirections(CancellationToken ct, MapRequest request)
	{
			_logger.Debug("Showing directions.");

		if (request.IsCoordinatesSet)
		{
			await _dispatcherScheduler.Run(async _ => OpenMaps(request, true), ct);
		}
		else if (request.LocationName.HasValue())
		{
			await _dispatcherScheduler.Run(async _ => OpenMaps(request, true), ct);
		}
		else
		{
			_logger.Error("Directions not shown because the coordinates or the location's name are null.");

			return;
		}

		_logger.Info("Directions shown.");
	}

	/// <inheritdoc/>
	public async Task ShowLocation(CancellationToken ct, MapRequest request)
	{
		_logger.Debug("Showing location.");

		if (request.IsCoordinatesSet)
		{
			await _dispatcherScheduler.Run(async _ => OpenMaps(request, false), ct);
		}
		else if (request.LocationName.HasValue())
		{
			await _dispatcherScheduler.Run(async _ => OpenMaps(request, false), ct);
		}
		else
		{
			_logger.Error("Location not shown because the coordinates or the location's name are null.");

			return;
		}

		_logger.Info("Location shown.");
	}


	#region Open Maps

	private void OpenMaps(MapRequest request, bool withDirections = false)
	{
		if (_mapServiceTextProvider == null)
		{
			var appleProvider = new AppleMapsServiceProvider();

			var url = withDirections ? appleProvider.GetDirectionsUrl(request) : appleProvider.GetLocationUrl(request);

			if (UIApplication.SharedApplication.CanOpenUrl(url))
			{
				UIApplication.SharedApplication.OpenUrl(url);
			}
			else
			{
				if (_logger.IsEnabled(LogLevel.Error))
				{
					_logger.Error("Maps is not supported on this device.");
				}
			}
		}
		else
		{
			OpenMapsWithAProvider(request, withDirections);
		}
	}

	private void OpenMapsWithAProvider(MapRequest request, bool isNavigating = false)
	{
		var installedNavigationApps = GetProviderInformation(request, isNavigating);

		ShowChoiceAlertOrOpenTheProvider(installedNavigationApps);
	}

	private Dictionary<string, NSUrl> GetProviderInformation(MapRequest request, bool isNavigating = false)
	{
		var installedNavigationApps = new Dictionary<string, NSUrl>();

		foreach (var provider in _mapServiceProviders)
		{
			if (UIApplication.SharedApplication.CanOpenUrl(provider.Url))
			{
				installedNavigationApps.Add(provider.Name, isNavigating ? provider.GetDirectionsUrl(request) : provider.GetLocationUrl(request));
			}

		}

		return installedNavigationApps;
	}


	private void ShowChoiceAlertOrOpenTheProvider(Dictionary<string, NSUrl> installedNavigationApps)
	{
		// We don't want to show the alert if we only have one navigation app
		if (installedNavigationApps.Count == 1)
		{
			UIApplication.SharedApplication.OpenUrl(installedNavigationApps.First().Value);

			_logger.Info($"Opening {installedNavigationApps.First().Key} with following query : '{installedNavigationApps.First().Value}'");

			return;
		}

		var alert = new UIAlertController()
		{
			Title = _mapServiceTextProvider.Title,
			Message = _mapServiceTextProvider.Description
		};

		foreach (var app in installedNavigationApps)
		{
			var button = UIAlertAction.Create(app.Key, UIAlertActionStyle.Default, (action) =>
			{
				UIApplication.SharedApplication.OpenUrl(app.Value);

				_logger.Info($"Opened {app.Key} with following query : '{app.Value}'");
			});

			alert.AddAction(button);
		}

		var cancelButton = UIAlertAction.Create(_mapServiceTextProvider.CancelTitle, UIAlertActionStyle.Cancel, (action) => { });
		alert.AddAction(cancelButton);

		UIViewController viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
		viewController.PresentViewController(alert, true, null);
	}

	#endregion
}
#endif
