using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cartography.Core;
using Cartography.DynamicMap;

namespace Umbrella.Location.Samples.Uno
{
	public class Pretty_PushpinSelectionPageViewModel
	{
		//public const string BANNER_VISIBLE = "Visible";
		//public const string BANNER_COLLAPSED = "Collapsed";

		//public const string LOCATION_STATE_MAP = "Map";
		//public const string LOCATION_STATE_LIST = "List";

		//private ILocationServiceEx _locationService;
		//private Func<string, Task> _updateVisualState;

		//public Pretty_PushpinSelectionPageViewModel(ILocationServiceEx locationService, Func<string, Task> updateVisualState)
		//{
		//	_locationService = locationService;
		//	_updateVisualState = updateVisualState;

		//	Build(b => b
		//		.MapComponent<PushpinEntity>(mcb => mcb
		//			.ShowPushpins(ppb => ppb
		//				// Query to execute to get pushpins when user pans or zooms
		//				.Load(GetPushpins)

		//				// Defines a minimal zoom level under pushpins are hidden and query is not executed
		//				.WhenZoomIsGreaterThan(ZoomLevels.State)

		//				// Reload pushpins when the viewport changes
		//				.RefreshOnViewPortChanged()

		//				// Reload pushpins when the property is updated
		//				.RefreshOnPropertyChanged(Pushpins)
		//			)

		//			.ShowUserPosition(_locationService)

		//			// Starting coordinates on the map
		//			// isAnimationDisabled so that we don't slowly zoom in at the launch
		//			.StartAt(l => l.Coordinates(GetStartingCoordinates, zoomLevel: ZoomLevels.Region, isAnimationDisabled: true))

		//			//Locate me button
		//			.AttachZoomToUserPositionCommand("LocateMe", locationService, onSuccess: OnSuccess, onError: OnError)

		//			// Do not update (execute query) too often
		//			.EnableAutomaticMinDistanceForUpdate()

		//			// Makes more options/extensions available
		//			.AdvancedConfiguration()

		//			// Make sure the zooming animation lasts 1 second
		//			.SetAnimationDurationSeconds(1)

		//			// Providing a dynamic property into which the MapComponent may store its viewport
		//			// (For if we want to retrieve information about the coordinates being viewed, zoom level, etc.)
		//			.ViewPort(MapViewPort)

		//			// Providing a dynamic property into which the MapComponent may store selected pushpins.
		//			// (If a user taps on or otherwise selects a pushpin, it will be set in this dynamic property)
		//			.SelectedPushpins(SelectedPushpins)
		//		)
		//		.Properties(pb => pb
		//			.Attach(MapViewPort, () => default(MapViewPort))
		//			.Attach(IsLocateMeOnError, () => false)
		//			.Attach(IsTooFar, () => false)
		//			.Attach(IsMapShowing, () => true)
		//			.Attach(Pushpins, GetInitialPushpins)
		//			.Attach(SelectedPushpin, () => SelectedPushpins.Value.Select(s => s.FirstOrDefault()))
		//			.Attach(IsPushpinSelected, () => SelectedPushpin.Value.Select(p => p != null))
		//			.AttachUserCommand(nameof(ToggleShowMap), cb => cb.Execute(ToggleShowMap))
		//		)
		//		.RegisterDisposable(ct => SubscribeOnPushpinSelected(ct))
		//		.RegisterDisposable(ct => SubscribeOnRefreshIsTooFar(ct))
		//	);
		//}

		//private async Task OnError(CancellationToken ct)
		//{
		//	IsLocateMeOnError.Value.OnNext(true);
		//	await _updateVisualState(BANNER_VISIBLE);
		//}

		//private async Task OnSuccess(CancellationToken ct)
		//{
		//	IsLocateMeOnError.Value.OnNext(false);
		//	var isPushpinSelected = await IsPushpinSelected;
		//	if (!isPushpinSelected)
		//	{
		//		await _updateVisualState(BANNER_COLLAPSED);
		//	}
		//}

		//private IDynamicProperty<MapViewPort> MapViewPort => this.GetProperty<MapViewPort>();

		//private IDynamicProperty<bool> IsPushpinSelected => this.GetProperty<bool>();

		//private IDynamicProperty<bool> IsLocateMeOnError => this.GetProperty<bool>();

		//private IDynamicProperty<bool> IsMapShowing => this.GetProperty<bool>();

		//private IDynamicProperty<PushpinEntity[]> Pushpins => this.GetProperty<PushpinEntity[]>();

		//private IDynamicProperty<PushpinEntity[]> SelectedPushpins => this.GetProperty<PushpinEntity[]>();

		//private async Task<IDisposable> SubscribeOnPushpinSelected(CancellationToken ct)
		//{
		//	return IsPushpinSelected.Value
		//		.CombineLatest(
		//			IsLocateMeOnError.Value,
		//			IsTooFar.Value,
		//			(isPushpinSelected, isLocateMeOnError, isTooFar) =>
		//				new
		//				{
		//					IsPushpinSelected = isPushpinSelected,
		//					IsLocateMeOnError = isLocateMeOnError,
		//					IsTooFar = isTooFar
		//				}
		//		)
		//		.DistinctUntilChanged()
		//		.SelectManyDisposePrevious(async (state, ct2) =>
		//		{
		//			if (state.IsPushpinSelected || state.IsLocateMeOnError || state.IsTooFar)
		//			{
		//				await _updateVisualState(BANNER_VISIBLE);
		//			}
		//			else
		//			{
		//				await _updateVisualState(BANNER_COLLAPSED);
		//			}
		//		})
		//		.Subscribe(
		//			_ => { },
		//			e => this.Log().ErrorIfEnabled(() => "Error in subscription to update visual state", e)
		//		);
		//}

		//private IDynamicProperty<PushpinEntity> SelectedPushpin => this.GetProperty<PushpinEntity>();

		//private IDynamicProperty<bool> IsTooFar => this.GetProperty<bool>();

		//private async Task ToggleShowMap(CancellationToken ct)
		//{
		//	var isMapShowing = await IsMapShowing;

		//	await _updateVisualState(isMapShowing ? LOCATION_STATE_LIST : LOCATION_STATE_MAP);

		//	IsMapShowing.Value.OnNext(!isMapShowing);
		//}

		//private async Task<IDisposable> SubscribeOnRefreshIsTooFar(CancellationToken ct)
		//{
		//	return MapViewPort.Value
		//		.SelectManyDisposePrevious(async (viewPort, ct2) =>
		//		{
		//			if (viewPort != null)
		//			{
		//				var isTooFar = viewPort.ZoomLevel <= ZoomLevels.State;
		//				var oldValue = await IsTooFar;
		//				if (oldValue != isTooFar)
		//				{
		//					IsTooFar.Value.OnNext(isTooFar);
		//					if (isTooFar)
		//					{
		//						await _updateVisualState(BANNER_VISIBLE);
		//					}
		//					else
		//					{
		//						var isLocateMeOnError = await IsLocateMeOnError;
		//						if (!isLocateMeOnError)
		//						{
		//							await _updateVisualState(BANNER_COLLAPSED);
		//						}
		//					}
		//				}
		//			}
		//		})
		//		.Subscribe(
		//			_ => { },
		//			e => this.Log().ErrorIfEnabled(() => "Error in subscription to update IsTooFar", e)
		//		);
		//}

		//private async Task<GeoCoordinate> GetStartingCoordinates(CancellationToken ct)
		//{
		//	return new GeoCoordinate(45.5250919, -73.42036505);
		//}

		//private async Task<PushpinEntity[]> GetPushpins(CancellationToken ct, MapViewPort mapViewPort)
		//{
		//	return await Pushpins.Value.FirstAsync(ct);
		//}

		//private async Task<PushpinEntity[]> GetInitialPushpins(CancellationToken ct)
		//{
		//	return new[] {
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 1",
		//				Address = "123 Main Street 1",
		//				Coordinates = new GeoCoordinate { Latitude = 46.3938717, Longitude = -72.0921769 }
		//			},
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 2",
		//				Address = "123 Main Street 2",
		//				Coordinates = new GeoCoordinate { Latitude = 45.5502838, Longitude = -73.2801901 }
		//			},
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 3",
		//				Address = "123 Main Street 3",
		//				Coordinates = new GeoCoordinate { Latitude = 45.5502838, Longitude = -72.0921769 }
		//			},
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 4",
		//				Address = "123 Main Street 4",
		//				Coordinates = new GeoCoordinate { Latitude = 47.5502838, Longitude = -72.0921769 }
		//			},
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 5",
		//				Address = "123 Main Street 5",
		//				Coordinates = new GeoCoordinate { Latitude = 45.5502838, Longitude = -71.0921769 }
		//			},
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 6",
		//				Address = "123 Main Street 6",
		//				Coordinates = new GeoCoordinate { Latitude = 48.5502838, Longitude = -75.0921769 }
		//			},
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 7",
		//				Address = "123 Main Street 7",
		//				Coordinates = new GeoCoordinate { Latitude = 41.5502838, Longitude = -70.0921769 }
		//			},
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 8",
		//				Address = "123 Main Street 8",
		//				Coordinates = new GeoCoordinate { Latitude = 45.49990, Longitude = -73.56054 }
		//			},
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 9",
		//				Address = "123 Main Street 9",
		//				Coordinates = new GeoCoordinate { Latitude = 44.50990, Longitude = -73.56054 }
		//			},
		//			new PushpinEntity {
		//				Name = "Contoso Pvt. Ltd. 10",
		//				Address = "123 Main Street 10",
		//				Coordinates = new GeoCoordinate { Latitude = 46.51990, Longitude = -73.56054 }
		//			},
		//		};
		//}
	}
}
