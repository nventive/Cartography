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

namespace Cartography.MapService
{
	/// <summary>
	/// Implementation of <see href="IMapService" />
	/// </summary>
	public class MapServiceiOS : IMapService
	{
		private readonly IDispatcherScheduler _dispatcherScheduler;
		private readonly IMapServiceProvider[] _mapServiceProviders;
		private readonly MapServiceTextProvider _mapServiceTextProvider;
		/// <summary>
		/// Initializes a new instance of the <see cref="MapServiceiOS"/> class.
		/// </summary>
		/// <param name="dispatcherScheduler">Dispatcher scheduler</param>
		/// <param name="mapServiceTextProvider">Text provider, if null the default ios providor will be use</param>
		/// <param name="mapServiceProviders">MapServiceProvides for iOS</param>
		public MapServiceiOS(
			IDispatcherScheduler dispatcherScheduler,
			MapServiceTextProvider mapServiceTextProvider = null,
			MapServiceiOSProvider[] mapServiceProviders = null
		)
		{
			_dispatcherScheduler = dispatcherScheduler;
			_mapServiceTextProvider = mapServiceTextProvider;
			_mapServiceProviders = MapServiceProviderFactory.Create(mapServiceProviders);
		}

		/// <inheritdoc/>
		public async Task ShowDirections(CancellationToken ct, MapRequest request)
		{
			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug("Showing directions.");
			}

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
				if (this.Log().IsEnabled(LogLevel.Error))
				{
					this.Log().Error("Directions not shown because the coordinates or the location's name are null.");
				}

				return;
			}

			if (this.Log().IsEnabled(LogLevel.Information))
			{
				this.Log().Info("Directions shown.");
			}
		}

		/// <inheritdoc/>
		public async Task ShowLocation(CancellationToken ct, MapRequest request)
		{
			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug("Showing location.");
			}

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
				if (this.Log().IsEnabled(LogLevel.Error))
				{
					this.Log().Error("Location not shown because the coordinates or the location's name are null.");
				}

				return;
			}

			if (this.Log().IsEnabled(LogLevel.Information))
			{
				this.Log().Info("Location shown.");
			}
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
					if (this.Log().IsEnabled(LogLevel.Error))
					{
						this.Log().Error("Maps is not supported on this device.");
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

				if (this.Log().IsEnabled(LogLevel.Information))
				{
					this.Log().Info($"Opening {installedNavigationApps.First().Key} with following query : '{installedNavigationApps.First().Value}'");
				}

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

					if (this.Log().IsEnabled(LogLevel.Information))
					{
						this.Log().Info($"Opened {app.Key} with following query : '{app.Value}'");
					}
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
}
#endif
