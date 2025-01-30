#if WINDOWS && false
using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using GeolocatorService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Uno.Extensions;
using Uno.Logging;
using Windows.System;

namespace Cartography.MapService
{
	/// <summary>
	/// Implementation of <see href="IMapService" />
	/// </summary>
	public class MapServiceWindows : IMapService
	{
		private readonly IGeolocatorService _locationService;
		private readonly IDispatcherScheduler _dispatcherScheduler;
		private readonly ILogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="MapServiceUWP"/> class.
		/// </summary>
		/// <param name="dispatcherScheduler">Dispatcher</param>
		/// <param name="locationService">Location service</param>
		/// <param name="logger">logger</param>
		public MapServiceWindows(IDispatcherScheduler dispatcherScheduler, IGeolocatorService locationService, ILogger logger = null)
		{
			_dispatcherScheduler = dispatcherScheduler;
			_locationService = locationService;
			_logger = logger ?? NullLogger.Instance;
		}

		/// <inheritdoc/>
		public async Task ShowDirections(CancellationToken ct, MapRequest request)
		{
			_logger.Debug("Showing directions.");

			if (request.IsCoordinatesSet)
			{
				var currentLocation = (await _locationService.GetLocation(ct)).Point.Position;

				var url = "bingmaps:?rtp=pos.{0}_{1}~pos.{2}_{3}_{4}".InvariantCultureFormat(
					currentLocation.Latitude,
					currentLocation.Longitude,
					request.Coordinates.Latitude,
					request.Coordinates.Longitude,
					request.Label);

				await _dispatcherScheduler.Run(async _ => await Launcher.LaunchUriAsync(new Uri(url)), ct);

				_logger.Info("Directions shown.");
			}
			else
			{
				_logger.Error("Directions not shown because the coordinates are null.");
			}
		}

		/// <inheritdoc/>
		public async Task ShowLocation(CancellationToken ct, MapRequest request)
		{
			_logger.Debug("Showing location.");

			if (request.IsCoordinatesSet)
			{
				var url = "bingmaps:?cp={0}~{1}"
					.InvariantCultureFormat(
						request.Coordinates.Latitude,
						request.Coordinates.Longitude);

				await _dispatcherScheduler.Run(async _ => await Launcher.LaunchUriAsync(new Uri(url)), ct);

				_logger.Info("The location is shown.");
			}
			else
			{
				_logger.Error("Location not shown because the coordinates are null.");
			}
		}
	}
}
#endif
