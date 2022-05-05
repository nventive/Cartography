#if NETFX_CORE
using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using GeolocatorService;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;
using Windows.System;

namespace Nventive.Location.MapService
{
	/// <summary>
	/// Implementation of <see href="IMapService" />
	/// </summary>
	public class MapServiceUWP : IMapService
	{
		private readonly IGeolocatorService _locationService;
		private readonly IDispatcherScheduler _dispatcherScheduler;

		/// <summary>
		/// Initializes a new instance of the <see cref="MapServiceUWP"/> class.
		/// </summary>
		/// <param name="dispatcherScheduler">Dispatcher</param>
		/// <param name="locationService">Location service</param>
		public MapServiceUWP(IDispatcherScheduler dispatcherScheduler, IGeolocatorService locationService)
		{
			_dispatcherScheduler = dispatcherScheduler;
			_locationService = locationService;
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
				var currentLocation = (await _locationService.GetLocation(ct)).Point.Position;

				var url = "bingmaps:?rtp=pos.{0}_{1}~pos.{2}_{3}_{4}".InvariantCultureFormat(
					currentLocation.Latitude,
					currentLocation.Longitude,
					request.Coordinates.Latitude,
					request.Coordinates.Longitude,
					request.Label);

				await _dispatcherScheduler.Run(async _ => await Launcher.LaunchUriAsync(new Uri(url)), ct);

				if (this.Log().IsEnabled(LogLevel.Information))
				{
					this.Log().Info("Directions shown.");
				}
			}
			else
			{
				if (this.Log().IsEnabled(LogLevel.Error))
				{
					this.Log().Error("Directions not shown because the coordinates are null.");
				}
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
				var url = "bingmaps:?cp={0}~{1}"
					.InvariantCultureFormat(
						request.Coordinates.Latitude,
						request.Coordinates.Longitude);

				await _dispatcherScheduler.Run(async _ => await Launcher.LaunchUriAsync(new Uri(url)), ct);

				if (this.Log().IsEnabled(LogLevel.Information))
				{
					this.Log().Info("The location is shown.");
				}
			}
			else
			{
				if (this.Log().IsEnabled(LogLevel.Error))
				{
					this.Log().Error("Location not shown because the coordinates are null.");
				}
			}
		}
	}
}
#endif
