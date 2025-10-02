#if __IOS__
using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Uno.Extensions;

namespace Cartography.DynamicMap.GoogleMap.iOS;

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
#if __IOS__
		if (d != null && GetPushpinImageSelector(d) is IValueConverter imageSelector)
		{
			if (d is GoogleMapControl gmap)
			{
				gmap.MarkerUpdater = (pin, marker) =>
				{
					var name = (string)imageSelector.Convert(pin.Content, null, pin.IsSelected, CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
					var formattedName = name.Replace("ms-appx:///", "");
					marker.Icon = UIKit.UIImage.FromFile(formattedName);
					marker.Title = pin.Content.ToString();
				};
			}
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
