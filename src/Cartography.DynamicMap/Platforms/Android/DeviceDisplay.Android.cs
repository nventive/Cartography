#if ANDROID
using System;
using System.Diagnostics;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace Cartography.DynamicMap
{
	public static class DeviceDisplay
	{
		public static DisplayInfo GetMainDisplayInfo()
		{
			using var displayMetrics = new DisplayMetrics();
			var display = GetDefaultDisplay();
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CA1422 // Validate platform compatibility
#pragma warning disable CA1416 // Validate platform compatibility
			display?.GetRealMetrics(displayMetrics);
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility

			return new DisplayInfo(
				width: displayMetrics?.WidthPixels ?? 0,
				height: displayMetrics?.HeightPixels ?? 0,
				density: displayMetrics?.Density ?? 1,
				orientation: CalculateOrientation(),
				rotation: CalculateRotation(display),
				rate: display?.RefreshRate ?? 0);
		}

		static DisplayRotation CalculateRotation(Display? display) =>
	display?.Rotation switch
	{
		SurfaceOrientation.Rotation270 => DisplayRotation.Rotation270,
		SurfaceOrientation.Rotation180 => DisplayRotation.Rotation180,
		SurfaceOrientation.Rotation90 => DisplayRotation.Rotation90,
		SurfaceOrientation.Rotation0 => DisplayRotation.Rotation0,
		_ => DisplayRotation.Unknown,
	};

		static DisplayOrientation CalculateOrientation() =>
			Application.Context.Resources?.Configuration?.Orientation switch
			{
				Orientation.Landscape => DisplayOrientation.Landscape,
				Orientation.Portrait => DisplayOrientation.Portrait,
				Orientation.Square => DisplayOrientation.Portrait,
				_ => DisplayOrientation.Unknown
			};

		static Display? GetDefaultDisplay()
		{
			try
			{
				using var service = Application.Context.GetSystemService(Context.WindowService);
				using var windowManager = service?.JavaCast<IWindowManager>();
				return windowManager?.DefaultDisplay;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Unable to get default display: {ex}");
				return null;
			}
		}
	}
}
#endif