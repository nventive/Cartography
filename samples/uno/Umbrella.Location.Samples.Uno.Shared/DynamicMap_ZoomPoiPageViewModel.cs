using nVentive.Umbrella.Extensions;
using nVentive.Umbrella.Presentation.Light;
using System;
using System.Device.Location;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Umbrella.Location.Core;
using Umbrella.Location.DynamicMap;
using Umbrella.Location.LocationService;
using Uno.Extensions;
using Uno.Logging;

namespace Umbrella.Location.Samples.Uno
{
	public class DynamicMap_ZoomPoiPageViewModel : PageViewModel
	{
		private ILocationServiceEx _locationService;

		public DynamicMap_ZoomPoiPageViewModel(ILocationServiceEx locationService)
		{
			_locationService = locationService;

			Build(b => b
				.MapComponent<PushpinEntity>(mcb => mcb
					.ShowPushpins(ppb => ppb
						// Query to execute to get pushpins when user pan or zoom
						.Load(GetPushpins)
					)
					// Starting coordinates on the map
					// isAnimationDisabled so that we don't slowly zoom in at the launch
					.StartAt(l => l.ViewPort(GetStartingViewPort))

					// Makes more options/extensions available
					.AdvancedConfiguration()
				)
				.Properties(pb => pb
					.Attach(Pushpins, GetInitialPushpins)
				)
			);
		}

		private IDynamicProperty<PushpinEntity[]> Pushpins => this.GetProperty<PushpinEntity[]>();

		private async Task<MapViewPort> GetStartingViewPort(CancellationToken ct)
		{
			// Make sure that we are centered on user location but that we zoom out sufficiently to see all the pushpins
			var userLocation = await _locationService.GetValidLocation(ct);
			return new MapViewPort(GetUmbrellaCoordinate(userLocation.Location))
			{
				PointsOfInterest =
					(await GetInitialPushpins(ct))
					.Select(p => GetUmbrellaCoordinate(p.Coordinates))
					.ToArray()
			};
		}

		private Coordinate GetUmbrellaCoordinate(GeoCoordinate coordinate)
		{
			return new Coordinate { Latitude = coordinate.Latitude, Longitude = coordinate.Longitude };
		}

		private async Task<PushpinEntity[]> GetPushpins(CancellationToken ct, MapViewPort mapViewPort)
		{
			return await Pushpins.Value.FirstAsync(ct);
		}

		private async Task<PushpinEntity[]> GetInitialPushpins(CancellationToken ct)
		{
			return new[]
			{
				new PushpinEntity
				{
					Name = "Pushpin 1",
					Coordinates = new GeoCoordinate { Latitude = 46.3938717, Longitude = -72.0921769 }
				},
				new PushpinEntity
				{
					Name = "Pushpin 2",
					Coordinates = new GeoCoordinate { Latitude = 45.5502838, Longitude = -73.2801901 }
				},
				new PushpinEntity
				{
					Name = "Pushpin 3",
					Coordinates = new GeoCoordinate { Latitude = 45.5502838, Longitude = -72.0921769 }
				},
			};
		}
	}
}
