using System;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Cartography.Core;
using Cartography.DynamicMap;

namespace Umbrella.Location.Samples.Uno
{
	public class DynamicMap_MoveSearchPageViewModel
	{

		//public DynamicMap_MoveSearchPageViewModel(ILocationServiceEx locationService)
		//{
		//	Build(b => b
		//		.MapComponent<PushpinEntity>(mcb => mcb
		//			.ShowPushpins(ppb => ppb
		//				.Load(GetPushpins)
		//				// Skipping the initial value (null) we push in the dynamic property
		//				.RefreshOn(MapViewPortCoordinates.Value.Skip(1).SelectUnit())

		//				// Defines a minimal zoom level under pushpins are hidden and query is not executed
		//				.WhenZoomIsGreaterThan(ZoomLevels.Region)

		//				//// To make sure that the pushpins update properly when IsTooFar pushed a new value
		//				.RefreshOn(IsTooFar.Value.DistinctUntilChanged().Skip(1).SelectUnit())
		//			)

		//			// Starting coordinates on the map
		//			// isAnimationDisabled so that we don't slowly zoom in at the launch
		//			.StartAt(l => l.Coordinates(GetStartingCoordinates, isAnimationDisabled: true))

		//			// Do not update (execute query) too often
		//			.EnableAutomaticMinDistanceForUpdate()

		//			//Locate me button
		//			.AttachZoomToUserPositionCommand("LocateMe", locationService)

		//			// Makes more options/extensions available
		//			.AdvancedConfiguration()

		//			// Make sure the zooming animation lasts 1 second
		//			.SetAnimationDurationSeconds(1)

		//			// Providing a dynamic property into which the MapComponent may store its viewport
		//			// (For if we want to retrieve information about the coordinates being viewed, zoom level, etc.)
		//			.ViewPort(MapViewPort)

		//			.ViewPortCoordinates(MapViewPortCoordinates)

		//			.Pushpins(Pushpins)
		//		)
		//		.Properties(pb => pb
		//			.Attach(MapViewPort, () => default(MapViewPort))
		//			.Attach(MapViewPortCoordinates, () => default(MapViewPortCoordinates))
		//			.Attach(CurrentCoordinate, () => new Coordinate())
		//			.Attach(IsTooFar, () => false)
		//		)
		//		.RegisterDisposable(ct => SubscribeOnRefreshIsTooFar(ct))
		//	);
		//}

		//private IDynamicProperty<MapViewPort> MapViewPort => this.GetProperty<MapViewPort>();

		//private IDynamicProperty<Coordinate> CurrentCoordinate => this.GetProperty<Coordinate>();

		//private IDynamicProperty<MapViewPortCoordinates> MapViewPortCoordinates => this.GetProperty<MapViewPortCoordinates>();

		//private IDynamicProperty<PushpinEntity[]> Pushpins => this.GetProperty<PushpinEntity[]>();

		//private IDynamicProperty<bool> IsTooFar => this.GetProperty<bool>();

		//private async Task<IDisposable> SubscribeOnRefreshIsTooFar(CancellationToken ct)
		//{
		//	return MapViewPort.Value
		//		.SelectManyDisposePrevious(async (viewPort, ct2) =>
		//		{
		//			if (viewPort != null)
		//			{
		//				IsTooFar.Value.OnNext(viewPort.ZoomLevel <= ZoomLevels.Region);
		//			}
		//		})
		//		.Subscribe(
		//			_ => { },
		//			e => this.Log().ErrorIfEnabled(() => "Error in subscription to update IsTooFar", e)
		//		);
		//}

		//private async Task<GeoCoordinate> GetStartingCoordinates(CancellationToken ct)
		//{
		//	return new GeoCoordinate
		//	{
		//		Latitude = 45.503343,
		//		Longitude = -73.571695
		//	};
		//}

		//private PushpinEntity[] _allPushpins = new PushpinEntity[]
		//{
		//		new PushpinEntity()
		//		{
		//			Name = "Location 1",
		//			Coordinates = new GeoCoordinate(45.506238, -73.576308)
		//		},
		//		new PushpinEntity()
		//		{
		//			Name = "Location 2",
		//			Coordinates = new GeoCoordinate(45.502042, -73.574162)
		//		},
		//		new PushpinEntity()
		//		{
		//			Name = "Location 3",
		//			Coordinates = new GeoCoordinate(45.505832, -73.565654)
		//		},
		//		new PushpinEntity()
		//		{
		//			Name = "Location 4",
		//			Coordinates = new GeoCoordinate(45.504554, -73.560611)
		//		},
		//		new PushpinEntity()
		//		{
		//			Name = "Location 5",
		//			Coordinates = new GeoCoordinate(45.497981, -73.556204)
		//		},
		//		new PushpinEntity()
		//		{
		//			Name = "Location 6",
		//			Coordinates = new GeoCoordinate(45.492106, -73.557889)
		//		},
		//		new PushpinEntity()
		//		{
		//			Name = "Location 7",
		//			Coordinates = new GeoCoordinate(45.485773, -73.558404)
		//		},
		//		new PushpinEntity()
		//		{
		//			Name = "Location 8",
		//			Coordinates = new GeoCoordinate(45.479755, -73.563404)
		//		},
		//		new PushpinEntity()
		//		{
		//			Name = "Location 9",
		//			Coordinates = new GeoCoordinate(45.473842, -73.569498)
		//		},
		//		new PushpinEntity()
		//		{
		//			Name = "Location 10",
		//			Coordinates = new GeoCoordinate(45.469967, -73.591009)
		//		}
		//};

		//private async Task<PushpinEntity[]> GetPushpins(CancellationToken ct, MapViewPort mapViewPort, MapViewPortCoordinates boundaries)
		//{
		//	return _allPushpins
		//		.Where(p => boundaries?.IsSurrounding(p.Coordinates) ?? false)
		//		.ToArray();
		//}
	}
}
