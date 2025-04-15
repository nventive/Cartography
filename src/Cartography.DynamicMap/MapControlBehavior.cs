#if __MOBILE__ // || WINDOWS
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Uno.Extensions;


#if __ANDROID__
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Content.Res;
using System.IO;
using System.Linq;
#endif

namespace Cartography.DynamicMap;

public static class MapControlBehavior
{
	public static bool GetDisableRotation(DependencyObject obj)
	{
		return (bool)obj.GetValue(DisableRotationProperty);
	}

	public static void SetDisableRotation(DependencyObject obj, bool value)
	{
		obj.SetValue(DisableRotationProperty, value);
	}

	public static readonly DependencyProperty DisableRotationProperty =
		DependencyProperty.RegisterAttached("DisableRotation", typeof(bool), typeof(MapControlBehavior), new PropertyMetadata(default(bool), OnDisableRotationChanged));

	private static void OnDisableRotationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		dependencyObject.Maybe<MapControl>(map =>
		{
			var disableRotation = GetDisableRotation(map);
#if __IOS__
#pragma warning disable 618 // Flagged a obsolete will be fixed in later build
			map.RotateEnabled = !disableRotation;
#pragma warning restore 618
#elif __ANDROID__
			map.IsRotateGestureEnabled = !disableRotation;
#endif
		});
	}

	public static float GetIconWidth(DependencyObject obj)
	{
		return (float)obj.GetValue(IconWidthProperty);
	}

	public static void SetIconWidth(DependencyObject obj, float value)
	{
		obj.SetValue(IconWidthProperty, value);
	}

	public static readonly DependencyProperty IconWidthProperty =
		DependencyProperty.RegisterAttached("IconWidth", typeof(float), typeof(MapControlBehavior), new PropertyMetadata(30f, OnPusphinImageSelectorChanged));

	public static float GetIconHeight(DependencyObject obj)
	{
		return (float)obj.GetValue(IconHeightProperty);
	}

	public static void SetIconHeight(DependencyObject obj, float value)
	{
		obj.SetValue(IconHeightProperty, value);
	}

	public static readonly DependencyProperty IconHeightProperty =
		DependencyProperty.RegisterAttached("IconHeight", typeof(float), typeof(MapControlBehavior), new PropertyMetadata(45f, OnPusphinImageSelectorChanged));

	public static float GetSelectedIconWidth(DependencyObject obj)
	{
		return (float)obj.GetValue(SelectedIconWidthProperty);
	}

	public static void SetSelectedIconWidth(DependencyObject obj, float value)
	{
		obj.SetValue(SelectedIconWidthProperty, value);
	}

	public static readonly DependencyProperty SelectedIconWidthProperty =
		DependencyProperty.RegisterAttached("SelectedIconWidth", typeof(float), typeof(MapControlBehavior), new PropertyMetadata(30f, OnPusphinImageSelectorChanged));

	public static float GetSelectedIconHeight(DependencyObject obj)
	{
		return (float)obj.GetValue(SelectedIconHeightProperty);
	}

	public static void SetSelectedIconHeight(DependencyObject obj, float value)
	{
		obj.SetValue(SelectedIconHeightProperty, value);
	}

	public static readonly DependencyProperty SelectedIconHeightProperty =
		DependencyProperty.RegisterAttached("SelectedIconHeight", typeof(float), typeof(MapControlBehavior), new PropertyMetadata(45f, OnPusphinImageSelectorChanged));

	public static IValueConverter GetPushpinImageSelector(DependencyObject obj)
	{
		return (IValueConverter)obj.GetValue(PushpinImageSelectorProperty);
	}

	public static void SetPushpinImageSelector(DependencyObject obj, IValueConverter value)
	{
		obj.SetValue(PushpinImageSelectorProperty, value);
	}

	public static readonly DependencyProperty PushpinImageSelectorProperty =
		DependencyProperty.RegisterAttached("PushpinImageSelector", typeof(IValueConverter), typeof(MapControlBehavior), new PropertyMetadata(null, OnPusphinImageSelectorChanged));

	private static void OnPusphinImageSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
#if __ANDROID__ || __IOS__
		if (d != null && GetPushpinImageSelector(d) is IValueConverter imageSelector)
		{
#if __ANDROID__
			if (d is MapControl map)
			{
				map.MarkerUpdater = (pin, marker) =>
				{
					try
					{
						var density = DeviceDisplay.GetMainDisplayInfo().Density;
						var name = (string)imageSelector.Convert(pin.Content, null, pin.IsSelected, System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

						// Format the asset name
						var formattedName = name.Replace("ms-appx:///", "").Replace("Pushpin/", "").Replace("Assets/", "");

						// Define density suffix options
						string[] scalingSuffixes =
						[
							".scale-100",
							".scale-125",
							".scale-150",
							".scale-200",
							".scale-300",
							".scale-400",
							"",
						];

						// **Find the best matching suffix for the device's density**
						var bestSuffix = density switch
						{
							<= 1.0 => ".scale-100",
							<= 1.25 => ".scale-125",
							<= 1.5 => ".scale-150",
							<= 2.0 => ".scale-200",
							<= 3.0 => ".scale-300",
							<= 4.0 => ".scale-400",
							_ => ""
						};

						var context = Android.App.Application.Context;
						var assets = context.Assets;
						string selectedAsset = null;

						// **First try the best suffix, then fall back to other options**
						foreach (var suffix in new string[] { bestSuffix }.Concat<string>(scalingSuffixes))
						{
							var assetName = formattedName;
							if (!string.IsNullOrEmpty(suffix))
							{
								var extensionIndex = formattedName.LastIndexOf('.');
								if (extensionIndex > -1)
								{
									assetName = formattedName.Insert(extensionIndex, suffix);
								}
							}

							// **Check if the asset exists**
							try
							{
								using var testStream = assets.Open(assetName);
								selectedAsset = assetName;
								break; // Stop searching once a valid asset is found
							}
							catch (FileNotFoundException)
							{
								// Ignore and try next suffix
							}
						}

						if (selectedAsset == null)
						{
							Console.WriteLine($"[Error] No valid asset found for {formattedName}, using default marker.");
							marker.SetIcon(BitmapDescriptorFactory.DefaultMarker());
						}
						else
						{
							// **Load and resize the bitmap based on density**
							using var stream = assets.Open(selectedAsset);
							var bitmap = BitmapFactory.DecodeStream(stream);
							var bitmapDescriptor = BitmapDescriptorFactory.FromBitmap(bitmap);
							marker.SetIcon(bitmapDescriptor);
						}

						marker.Title = pin.Content.ToString();
					}
					catch (Exception ex)
					{
						Console.WriteLine($"[Error] Failed to set marker icon: {ex.Message}");
						marker.SetIcon(BitmapDescriptorFactory.DefaultMarker()); // Fallback to default marker
					}
				};
			}
#elif __IOS__
			if (d is MapControl map)
			{
				var iconWidth = GetIconWidth(map);
				var iconHeight = GetIconHeight(map);
				var selectedIconWidth = GetSelectedIconWidth(map);
				var selectedIconHeight = GetSelectedIconHeight(map);
				var biggerWidth = iconWidth > selectedIconWidth ? iconWidth : selectedIconWidth;
				var biggerHeight = iconHeight > selectedIconHeight ? iconHeight : selectedIconHeight;
				var iconMargin = biggerHeight - iconHeight;
				var selectedMargin = biggerHeight - selectedIconHeight;

				map.PinTemplate = view =>
				{
					var pushpin = view.Annotation as MapControlItem;

					// Moves the image up, so that the bottom of the pins points to the correct position.
					var yAxisPoint = (biggerHeight / 2);
					view.CenterOffset = new System.Drawing.PointF(0, -yAxisPoint);


					// This fixes an issue that causes the image of recycled pins to not be updated properly.
					var selectedImage = imageSelector.Convert(pushpin.Content, typeof(string), pushpin.Content, System.Globalization.CultureInfo.CurrentUICulture.ToString()).ToString();
					var nonSelectedImage = imageSelector.Convert(pushpin.Content, typeof(string), null, System.Globalization.CultureInfo.CurrentUICulture.ToString()).ToString();

					return new Microsoft.UI.Xaml.Controls.Grid
					{
						Frame = new System.Drawing.RectangleF(0, 0, biggerWidth, biggerHeight),
						ClipsToBounds = true,
						Children =
						{
							new Microsoft.UI.Xaml.Controls.Image
							{
								ContentMode = UIKit.UIViewContentMode.Bottom,
								Source = selectedImage,
								Width = selectedIconWidth,
								Height = selectedIconHeight,
								Margin = new Thickness(0,selectedMargin,0,0),
							}.Binding("Visibility", new Binding { Path = "IsSelected", Converter = TrueToVisible}),

							new Microsoft.UI.Xaml.Controls.Image
							{
								ContentMode = UIKit.UIViewContentMode.Bottom,
								Source = nonSelectedImage,
								Width = iconWidth,
								Height = iconHeight,
								Margin = new Thickness(0, iconMargin, 0,0),
							}.Binding("Visibility", new Binding { Path = "IsSelected", Converter = TrueToCollapsed}),
						}
					};
				};
			}
#endif
		}
#endif
	}

	public static Thickness GetCompassMargin(DependencyObject obj)
	{
		return (Thickness)obj.GetValue(CompassMarginProperty);
	}

	public static void SetCompassMargin(DependencyObject obj, Thickness value)
	{
		obj.SetValue(CompassMarginProperty, value);
	}

	public static readonly DependencyProperty CompassMarginProperty =
		DependencyProperty.RegisterAttached("CompassMargin", typeof(Thickness), typeof(MapControlBehavior), new PropertyMetadata(default(Thickness), OnCompassMarginChanged));

	private static FromBoolToVisibility TrueToVisible = new FromBoolToVisibility() { VisibilityIfTrue = VisibilityIfTrue.Visible };
	private static FromBoolToVisibility TrueToCollapsed = new FromBoolToVisibility() { VisibilityIfTrue = VisibilityIfTrue.Collapsed };

	private static void OnCompassMarginChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
	{
		dependencyObject.Maybe<MapControl>(map =>
		{
			var margin = GetCompassMargin(map);

#if __ANDROID__
			map.CompassMargin = margin;
#endif
		});

	}

	// Converter is defined here as it is required by the behavior and we don't reference nventive.View
	private class FromBoolToVisibility : IValueConverter
	{
		private readonly ILogger _logger = NullLogger.Instance;
		public VisibilityIfTrue VisibilityIfTrue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			bool inverse = VisibilityIfTrue == VisibilityIfTrue.Collapsed;

			Visibility visibilityOnTrue = (!inverse) ? Visibility.Visible : Visibility.Collapsed;
			Visibility visibilityOnFalse = (!inverse) ? Visibility.Collapsed : Visibility.Visible;

			var valueToConvert = (bool)value;
			return valueToConvert ? visibilityOnTrue : visibilityOnFalse;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			_logger.LogWarning("The ConvertBack method is not implemented.");

			throw new NotImplementedException();
		}
	}

	public enum VisibilityIfTrue
	{
		Visible,
		Collapsed
	}
}
#endif
