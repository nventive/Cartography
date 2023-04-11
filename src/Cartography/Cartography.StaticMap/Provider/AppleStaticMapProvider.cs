#if __IOS__
using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using MapKit;
using Microsoft.Extensions.Logging;
using UIKit;
using Uno.Extensions;
using Uno.Logging;
#if NET6_0_OR_GREATER
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml.Media;
#endif

namespace Cartography.StaticMap.Provider
{
	/// <summary>
	/// Provides a static map created using the native iOS SDK.
	/// The resulting static map is a UIImage.
	/// </summary>
	internal class AppleStaticMapProvider : IStaticMapProvider
	{
		private readonly IDispatcherScheduler _dispatcherScheduler;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppleStaticMapProvider"/> class.
		/// </summary>
		/// <param name="dispatcherScheduler">Dispatcher</param>
		public AppleStaticMapProvider(IDispatcherScheduler dispatcherScheduler)
		{
			_dispatcherScheduler = dispatcherScheduler;
		}

		/// <summary>
		/// Get a map with the specified parameters.
		/// </summary>
		/// <param name="ct">Cancellation token</param>
		/// <param name="parameters">Map parameters</param>
		/// <returns>The map as a UIImage.</returns>
		public async Task<object> GetMap(CancellationToken ct, StaticMapParameters parameters)
		{
			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug($"Getting an Apple map with the scale: '{parameters?.Scale}', the width: '{parameters?.Width}', the height: '{parameters?.Height}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}'.");
			}

			return await await _dispatcherScheduler.Run(() =>
			{
				var options = CreateOptions(parameters);

				if (this.Log().IsEnabled(LogLevel.Information))
				{
					this.Log().Info($"Return an Apple map with the scale: '{parameters?.Scale}', the width: '{parameters?.Width}', the height: '{parameters?.Height}', '{parameters?.ViewPort?.PointsOfInterest}' POIs and a zoom level of '{parameters?.ViewPort?.ZoomLevel}').");
				}

				return CreateMapImage(ct, options);
			}, ct);
		}

		private MKMapSnapshotOptions CreateOptions(StaticMapParameters parameters)
		{
			return new MKMapSnapshotOptions()
			{
				Region = MapHelper.CreateRegion(parameters.ViewPort.Center, parameters.ViewPort.ZoomLevel.Value, new CGSize(parameters.Width, parameters.Height)),
				Size = new CGSize(parameters.Width, parameters.Height)
			};
		}

		private async Task<ImageSource> CreateMapImage(CancellationToken ct, MKMapSnapshotOptions options)
		{
			var tc = new TaskCompletionSource<UIImage>();
			using (ct.Register(() => tc.TrySetCanceled()))
			{
				var snapshotter = new MKMapSnapshotter(options);

				try
				{
					var snapshot = await snapshotter.StartAsync();

					tc.TrySetResult(snapshot.Image);
				}
				catch(Exception ex)
				{
					tc.TrySetException(ex);
				}

			}

			return await tc.Task;
		}
	}
}
#endif
