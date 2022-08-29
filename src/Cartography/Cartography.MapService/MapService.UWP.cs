#if NETFX_CORE
using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Uno.Extensions;
using Windows.System;

namespace Cartography.MapService
{
	/// <summary>
	/// Implementation of <see href="IMapService" />
	/// </summary>
	public class MapServiceUWP : IMapService
	{
		private readonly IDispatcherScheduler _dispatcherScheduler;
		private readonly ILogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="MapServiceUWP"/> class.
		/// </summary>
		/// <param name="dispatcherScheduler">Dispatcher</param>
		/// <param name="logger">logger</param>
		public MapServiceUWP(IDispatcherScheduler dispatcherScheduler, ILogger logger = null)
		{
			_dispatcherScheduler = dispatcherScheduler;
			_logger = logger ?? NullLogger.Instance;
		}

		/// <inheritdoc/>
		public async Task ShowDirections(CancellationToken ct, MapRequest request)
		{
			_logger.LogDebug("Showing directions.");

			if (request.IsCoordinatesSet & request.IsUserLocationSet)
			{
				var url = "bingmaps:?rtp=pos.{0}_{1}~pos.{2}_{3}_{4}".InvariantCultureFormat(
					request.UserLocation.Latitude,
					request.UserLocation.Longitude,
					request.Coordinates.Latitude,
					request.Coordinates.Longitude,
					request.Label);

				await _dispatcherScheduler.Run(async _ => await Launcher.LaunchUriAsync(new Uri(url)), ct);

				_logger.LogInformation("Directions shown.");
			}
			else
			{
				_logger.LogError("Directions not shown because the coordinates are null.");
			}
		}

		/// <inheritdoc/>
		public async Task ShowLocation(CancellationToken ct, MapRequest request)
		{
			_logger.LogDebug("Showing location.");

			if (request.IsCoordinatesSet)
			{
				var url = "bingmaps:?cp={0}~{1}"
					.InvariantCultureFormat(
						request.Coordinates.Latitude,
						request.Coordinates.Longitude);

				await _dispatcherScheduler.Run(async _ => await Launcher.LaunchUriAsync(new Uri(url)), ct);

				_logger.LogInformation("The location is shown.");
			}
			else
			{
				_logger.LogError("Location not shown because the coordinates are null.");
			}
		}
	}
}
#endif
