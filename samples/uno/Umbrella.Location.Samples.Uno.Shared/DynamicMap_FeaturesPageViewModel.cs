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
	public class DynamicMap_FeaturesPageViewModel : PageViewModel
	{
		public DynamicMap_FeaturesPageViewModel(ILocationServiceEx locationService)
		{
			Build(b => b
				.MapComponent<PushpinEntity>(mcb => mcb
					.ShowPushpins(ppb => ppb
						// Query to execute to get pushpins when user pan or zoom
						.Load(GetPushpins)

						// Defines a minimal zoom level under pushpins are hidden and query is not executed
						.WhenZoomIsGreaterThan(ZoomLevels.Region)

						// Reload pushpins when the viewport changes
						.RefreshOnViewPortChanged()

						// Reload pushpins when the property is updated
						.RefreshOnPropertyChanged(Pushpins)
					)
					// Starting coordinates on the map
					// isAnimationDisabled so that we don't slowly zoom in at the launch
					.StartAt(l => l.Coordinates(GetStartingCoordinates, ZoomLevels.City, isAnimationDisabled: true))

					// Do not update (execute query) too often
					.EnableAutomaticMinDistanceForUpdate()

					//Locate me button
					.AttachZoomToUserPositionCommand("LocateMe", locationService, onError: OnError)

					// Makes more options/extensions available
					.AdvancedConfiguration()

					// Make sure the zooming animation lasts 1 second
					.SetAnimationDurationSeconds(1)

					// Providing a dynamic property into which the MapComponent may store its viewport
					// (For if we want to retrieve information about the coordinates being viewed, zoom level, etc.)
					.ViewPort(MapViewPort)

					// Providing a dynamic property into which the MapComponent may store the boundaries
					// of the viewport in latitude, longitude
					.ViewPortCoordinates(MapViewPortCoordinates)

					// Providing a dynamic property into which the MapComponent may store selected pushpins.
					// (If a user taps on or otherwise selects a pushpin, it will be set in this dynamic property)
					.SelectedPushpins(SelectedPushpins)
				)
				.Properties(pb => pb
					// Changing this default value will not have any effect, because we zoom in to the pushpins anyway
					.Attach(MapViewPort, () => default(MapViewPort))
					.Attach(MapViewPortCoordinates, () => default(MapViewPortCoordinates))
					.Attach(ViewPortLongitude, () => default(string))
					.Attach(ViewPortLatitude, () => default(string))
					.Attach(Pushpins, GetInitialPushpins)
					.Attach(IsPointOfInterestShown, () => false)
					.Attach(IsLocateMeOnError, () => false)
					.Attach(IsTooFar, () => false)
					.AttachUserCommand(nameof(AddPushpin), AddPushpin)
					.AttachUserCommand(nameof(RemoveSelectedPushpin), RemoveSelectedPushpin)
					.AttachUserCommand(nameof(UpdateViewPort), UpdateViewPort)
					.AttachUserCommand(nameof(CenterOnPOI), CenterOnPOI)
					.Attach(SelectedPushpin, () => SelectedPushpins.Value.Select(s => s.FirstOrDefault()))
				)
				.RegisterDisposable(ct => SubscribeOnRefreshIsTooFar(ct))
			);
		}

		private IDynamicProperty<MapViewPort> MapViewPort => this.GetProperty<MapViewPort>();

		private IDynamicProperty<PushpinEntity[]> Pushpins => this.GetProperty<PushpinEntity[]>();

		private IDynamicProperty<PushpinEntity[]> SelectedPushpins => this.GetProperty<PushpinEntity[]>();

		private IDynamicProperty<PushpinEntity> SelectedPushpin => this.GetProperty<PushpinEntity>();

		private IDynamicProperty<string> ViewPortLongitude => this.GetProperty<string>();

		private IDynamicProperty<string> ViewPortLatitude => this.GetProperty<string>();

		private IDynamicProperty<bool> IsPointOfInterestShown => this.GetProperty<bool>();

		private IDynamicProperty<bool> IsLocateMeOnError => this.GetProperty<bool>();

		private IDynamicProperty<bool> IsTooFar => this.GetProperty<bool>();

		private async Task<IDisposable> SubscribeOnRefreshIsTooFar(CancellationToken ct)
		{
			return MapViewPort.Value
				.SelectManyDisposePrevious(async (viewPort, ct2) =>
				{
					if (viewPort != null)
					{
						IsTooFar.Value.OnNext(viewPort.ZoomLevel <= ZoomLevels.Region);
					}
				})
				.Subscribe(
					_ => { },
					e => this.Log().ErrorIfEnabled(() => "Error in subscription to update IsTooFar", e)
				);
		}

		private IDynamicProperty<MapViewPortCoordinates> MapViewPortCoordinates => this.GetProperty<MapViewPortCoordinates>();

		private async Task<GeoCoordinate> GetStartingCoordinates(CancellationToken ct)
		{
			return new GeoCoordinate
			{
				Latitude = 45.5016889,
				Longitude = -73.56725599999999
			};
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

		private async Task<Unit> AddPushpin(CancellationToken ct)
		{
			var pushpins = await Pushpins.Value.FirstAsync(ct);

			var newPushpin = await CreatePushpinAtCenter(ct);

			var list = pushpins.ToList();
			list.Add(newPushpin);

			Pushpins.Value.OnNext(list.ToArray());
			return Unit.Default;
		}

		private async Task<PushpinEntity> CreatePushpinAtCenter(CancellationToken ct)
		{
			var viewport = await MapViewPort.Value.FirstAsync(ct);
			var latitude = viewport.Center.Latitude;
			var longitude = viewport.Center.Longitude;

			return new PushpinEntity()
			{
				Coordinates = new GeoCoordinate(latitude, longitude),
				Name = string.Empty
			};
		}

		private async Task<Unit> RemoveSelectedPushpin(CancellationToken ct)
		{
			var pushpins = await Pushpins.Value.FirstAsync(ct);
			var selected = await SelectedPushpin.Value.FirstAsync(ct);

			var list = pushpins.ToList();
			list.RemoveAll(p => p.Coordinates == selected.Coordinates);

			Pushpins.Value.OnNext(list.ToArray());
			return Unit.Default;
		}

		private async Task OnError(CancellationToken ct)
		{
			IsLocateMeOnError.Value.OnNext(true);
		}

		private async Task UpdateViewPort(CancellationToken ct)
		{
			var latitudeString = await ViewPortLatitude;
			var longitudeString = await ViewPortLongitude;
			if (double.TryParse(latitudeString, out var latitude)
				&& double.TryParse(longitudeString, out var longitude)
				&& latitude >= -90
				&& latitude <= 90
				&& longitude >= -180
				&& longitude <= 180)
			{
				var viewport = await MapViewPort;
				viewport.Center.Latitude = latitude;
				viewport.Center.Longitude = longitude;
				MapViewPort.Value.OnNext(viewport);
			}
			else
			{
				throw new InvalidOperationException("Both latitude and longitude must be valid");
			}
		}

		private async Task CenterOnPOI(CancellationToken ct)
		{
			MapViewPort.Value.OnNext(new Core.MapViewPort(new Coordinate { Latitude = 45.582, Longitude = -73.749 }) { ZoomLevel = new ZoomLevel(16.0441079686075) });
		}
	}
}
