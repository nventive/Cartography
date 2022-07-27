using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Cartography.Core;
using Cartography.DynamicMap;

namespace Samples.Presentation
{
	public partial class DynamicMap_SelectedFlipViewPageViewModel : ViewModel
	{
        public DynamicMap_SelectedFlipViewPageViewModel()
        {

        }
		//private static Feeds<DynamicMap_SelectedFlipViewPageViewModel> _feeds;

		//private readonly double _minimumLatitude = 45.475;
		//private readonly double _minimumLongitude = -73.63;

		//private readonly double _maximumLatitude = 45.52;
		//private readonly double _maximumLongitude = -73.53;

		//private bool _isInitialZoom = true;
		//private ILocationServiceEx _locationService;

		//public DynamicMap_SelectedFlipViewPageViewModel(ILocationServiceEx locationService)
		//{
		//	Build(b => b
		//		.MapComponent<PushpinEntity>(mcb => mcb
		//			.AttachZoomToUserPositionCommand("LocateMe", locationService)

		//			// Makes more options/extensions available
		//			.AdvancedConfiguration()

		//				// Providing a feed into which the MapComponent may store its viewport
		//				// In this sample we will be using it to push custom viewport changes
		//				.ViewPort(MapViewPort)

		//				// Feed that provides pushpins to display on the map
		//				.Pushpins(Pushpins)

		//				// Feed that specifies the selected pushpins
		//				// In this sample only a single pushpin will be selected at a time
		//				.SelectedPushpins(SelectedPushpins)

		//				// Provide a custom auto zoom selector whenever SelectedPushpins changes
		//				.AutoZoomTo(
		//					methodName: "AutoZoomToSelectedCustom",
		//					source: () => SelectedPushpins.Get.ToObservable(),
		//					viewportSelector: CreateMapViewPortForSelectedPushpins
		//				)

		//				// Make sure the zooming animation lasts 1 second
		//				.SetAnimationDurationSeconds(1)
		//		)
		//		.Properties(pb => pb
		//			.Attach(MapViewPort)
		//			.Attach(Pushpins)
		//			.Attach(SelectedPushpins)
		//			.Attach(SelectedFlipViewIndex)
		//		)
		//		.Loaded(OnLoaded)
		//	);

		//	Initialize();
		//}

		//private async Task OnLoaded(CancellationToken ct, bool isFirstLoad)
		//{
		//	if (!isFirstLoad)
		//	{
		//		return;
		//	}

		//	// Bind SelectedFlipViewIndex and SelectedPushpins to intents that will keep them synchronized
		//	SelectedFlipViewIndexChanged.Bind(SelectedFlipViewIndex.Get.AsSignal());
		//	SelectedPushpinChanged.Bind(SelectedPushpins.Get.AsSignal());
		//}
		//private async Task<MapViewPort> CreateMapViewPortForSelectedPushpins(CancellationToken ct, PushpinEntity[] selectedPushpins, GeoCoordinate center)
		//{
		//	// Perform any custom logic to create the MapViewPort here

		//	var pushpin = selectedPushpins.First();

		//	var viewPort = new MapViewPort(pushpin.Coordinates)
		//	{
		//		ZoomLevel = ZoomLevels.District,
		//		// isAnimationDisabled so that we don't slowly zoom in at the launch
		//		IsAnimationDisabled = _isInitialZoom,
		//	};

		//	_isInitialZoom = false;

		//	return viewPort;
		//}

		//private Feed<PushpinEntity[]> Pushpins { get; } = _feeds.GetFeed(that => _feeds
		//	.FeedFromAsync(async ct =>
		//	{
		//		var pushpins = await that.GeneratePushpins(ct);

		//		return pushpins.ToArray();
		//	})
		//);

		//private State<MapViewPort> MapViewPort { get; } = _feeds.GetState(that => _feeds.StateFromValue<MapViewPort>());

		//private State<PushpinEntity[]> SelectedPushpins { get; } = _feeds.GetState(that => _feeds.StateFromValue(new PushpinEntity[0]));

		//private State<int> SelectedFlipViewIndex { get; } = _feeds.GetState(that => _feeds.StateFromValue(0));

		//private Intent<Unit> SelectedPushpinChanged { get; } = _feeds.GetIntent(that => that.SelectedFlipViewIndex.CreateIntent(async (ct, currentIndex) =>
		//{
		//	var selectedPush = await that.SelectedPushpins.Get.ToTask(ct);
		//	var pushpins = await that.Pushpins.ToTask(ct);

		//	//Set the tile selected flag in the PeekingFlipView items
		//	//Necessary to do this on the dispatcher so that the UI is aware
		//	if (pushpins != null)
		//	{
		//		var dispatcher = await that.GetDispatcher(ct);
		//		dispatcher.ScheduleTask(
		//			async (ct2, _) =>
		//			{
		//				foreach (var pushPin in pushpins)
		//				{
		//					pushPin.IsTileSelected = selectedPush.Contains(pushPin);
		//				}
		//			})
		//			.DisposeWith(that.Subscriptions);
		//	}

		//	var newIndex = pushpins?.IndexOf(selectedPush.FirstOrDefault());

		//	if (newIndex >= 0 && newIndex != currentIndex.SomeOrDefault())
		//	{
		//		return newIndex;
		//	}

		//	return currentIndex;
		//}));

		//private Intent<Unit> SelectedFlipViewIndexChanged { get; } = _feeds.GetIntent(that => that.SelectedPushpins.CreateIntent(async (ct, currentPushpin) =>
		//{
		//	var selectedFlipViewIndex = await that.SelectedFlipViewIndex.Get.ToTask(ct);
		//	var pushPins = await that.Pushpins.ToTask(ct);

		//	var selectedPushpin = pushPins?.ElementAtOrDefault(selectedFlipViewIndex);

		//	if (selectedPushpin != null && selectedPushpin != currentPushpin.SomeOrDefault()?.FirstOrDefault())
		//	{
		//		return new[] { selectedPushpin };
		//	}

		//	return currentPushpin;
		//})
		//);

		//private async Task<PushpinEntity[]> GeneratePushpins(CancellationToken ct)
		//{
		//	var pushpinList = new List<PushpinEntity>();

		//	var random = new Random();

		//	for (int i = 1; i <= 20; i++)
		//	{
		//		var coordinate = GenerateCoordinate(random);
		//		pushpinList.Add(new PushpinEntity() { Coordinates = coordinate, Name = $"Pushpin {i}" });
		//	}

		//	return pushpinList.ToArray();
		//}

		//private GeoCoordinate GenerateCoordinate(Random random)
		//{
		//	var latitude = random.NextDouble() * (_maximumLatitude - _minimumLatitude) + _minimumLatitude;
		//	var longitude = random.NextDouble() * (_maximumLongitude - _minimumLongitude) + _minimumLongitude;

		//	return new GeoCoordinate(latitude, longitude);
		//}
	}
}
