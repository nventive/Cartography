#if NETFX_CORE
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Nventive.Location.DynamicMap
{
	/// <summary>
	/// Extensions of <see cref="Windows.UI.Xaml.Controls.Maps.MapControl"/>.
	/// </summary>
	public static class MapControlExtensions
	{
		/// <summary>
		/// Creates an observable sequence of the <see cref="Windows.UI.Xaml.Controls.Maps.MapControl.CenterChanged"/> event.
		/// </summary>
		internal static IObservable<Unit> ObserveCenterChanged(
			this Windows.UI.Xaml.Controls.Maps.MapControl map,
			FrameworkElementExtensions.UiEventSubscriptionsOptions options = FrameworkElementExtensions.UiEventSubscriptionsOptions.Default)
		{
			return FrameworkElementExtensions
				.FromEventPattern<Windows.Foundation.TypedEventHandler<Windows.UI.Xaml.Controls.Maps.MapControl, object>, object>(
					h => map.CenterChanged += h,
					h => map.CenterChanged -= h,
					map,
					options)
				.Select(_ => Unit.Default);
		}

		/// <summary>
		/// Creates an observable sequence of the <see cref="Windows.UI.Xaml.Controls.Maps.MapControl.HeadingChanged"/> event.
		/// </summary>
		internal static IObservable<Unit> ObserveHeadingChanged(
			this Windows.UI.Xaml.Controls.Maps.MapControl map,
			FrameworkElementExtensions.UiEventSubscriptionsOptions options = FrameworkElementExtensions.UiEventSubscriptionsOptions.Default)
		{
			return FrameworkElementExtensions
				.FromEventPattern<Windows.Foundation.TypedEventHandler<Windows.UI.Xaml.Controls.Maps.MapControl, object>, object>(
					h => map.HeadingChanged += h,
					h => map.HeadingChanged -= h,
					map,
					options)
				.Select(_ => Unit.Default);
		}

		/// <summary>
		/// Creates an observable sequence of the <see cref="Windows.UI.Xaml.Controls.Maps.MapControl.PitchChanged"/> event.
		/// </summary>
		internal static IObservable<Unit> ObservePitchChanged(
			this Windows.UI.Xaml.Controls.Maps.MapControl map,
			FrameworkElementExtensions.UiEventSubscriptionsOptions options = FrameworkElementExtensions.UiEventSubscriptionsOptions.Default)
		{
			return FrameworkElementExtensions
				.FromEventPattern<Windows.Foundation.TypedEventHandler<Windows.UI.Xaml.Controls.Maps.MapControl, object>, object>(
					h => map.PitchChanged += h,
					h => map.PitchChanged -= h,
					map,
					options)
				.Select(_ => Unit.Default);
		}

		/// <summary>
		/// Creates an observable sequence of the <see cref="Windows.UI.Xaml.Controls.Maps.MapControl.ZoomLevelChanged"/> event.
		/// </summary>
		internal static IObservable<Unit> ObserveZoomLevelChanged(
			this Windows.UI.Xaml.Controls.Maps.MapControl map,
			FrameworkElementExtensions.UiEventSubscriptionsOptions options = FrameworkElementExtensions.UiEventSubscriptionsOptions.Default)
		{
			return FrameworkElementExtensions
				.FromEventPattern<Windows.Foundation.TypedEventHandler<Windows.UI.Xaml.Controls.Maps.MapControl, object>, object>(
					h => map.ZoomLevelChanged += h,
					h => map.ZoomLevelChanged -= h,
					map,
					options)
				.Select(_ => Unit.Default);
		}
	}
}
#endif
