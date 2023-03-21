#if __IOS__
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using GeolocatorService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using UIKit;
using Uno.Extensions;
using Uno.Logging;
using Windows.Devices.Geolocation;
using Microsoft.UI.Xaml;
using System.Drawing;

namespace Cartography.DynamicMap
{
	public partial class MapControlBase
	{
		private UIPanGestureRecognizer _gestureRecognizer;

		partial void PartialConstructor(ILogger<MapControlBase> logger = null)
		{
			_isReady = true;
			_logger = logger ?? NullLogger<MapControlBase>.Instance;
		}

		public ZoomLevel ZoomLevel => GetZoomLevel();

		/// <summary>
		/// Enables multiple selected pins
		/// </summary>
		protected bool AllowMultipleSelection { get { return SelectionMode == MapSelectionMode.Multiple; } }

		protected abstract ZoomLevel GetZoomLevel();

		protected abstract void UpdateMapUserLocation(LocationResult locationAndStatus);

		protected abstract IEnumerable<IObservable<Unit>> GetViewPortChangedTriggers();

		protected abstract Geopoint GetCenter();

		protected abstract MapViewPortCoordinates GetViewPortCoordinates();

		private MapViewPort GetViewPort()
		{
			var viewPort = new MapViewPort(GetCenter())
			{
				// Zoom level: http://stackoverflow.com/questions/7594827/how-to-find-current-zoom-level-of-mkmapview
				ZoomLevel = ZoomLevel,

				//TODO
				// http://stackoverflow.com/questions/15842914/progmatically-setting-heading-in-mkmapview
				// http://stackoverflow.com/questions/21453199/ios-set-mkmapview-camera-heading-rotation-around-anchor-point
				// Heading = _map.Heading,
			};

			return viewPort;
		}

		protected abstract Task SetViewPort(CancellationToken ct, MapViewPort viewPort);

		protected abstract void UpdateMapPushpins(IGeoLocated[] items, IGeoLocated[] selectedItems);

		protected abstract void UpdateIcon(object icon);

		protected abstract void UpdateSelectedIcon(object icon);

		protected abstract void UpdateMapSelectedPushpins(IGeoLocated[] items);

		protected abstract void UpdateCompassButtonVisibilityInner(Visibility compassButtonVisibility);

		protected abstract void UpdateIsRotateGestureEnabledInner(bool isRotateGestureEnabled);
		protected abstract bool GetInitializationStatus();

		partial void UpdateCompassButtonVisibility(Visibility compassButtonVisibility)
		{
			_logger.Debug("Updating the compass button's visibility.");

			UpdateCompassButtonVisibilityInner(compassButtonVisibility);

			_logger.Info("Updated the compass button's visibility.");
		}

		partial void UpdateIsRotateGestureEnabled(bool isRotateGestureEnabled)
		{
			_logger.Debug($"{(IsRotateGestureEnabled ? "Enabling" : "Disabling")} the rotation gesture.");

			UpdateIsRotateGestureEnabledInner(isRotateGestureEnabled);

			_logger.Info($"{(IsRotateGestureEnabled ? "Enabled" : "Disabled")} the rotation gesture.");
		}

		protected void AddDragGestureRecognizer(UIView mapView)
		{
			_gestureRecognizer = new UIPanGestureRecognizer(DidDragMap)
			{
				Delegate = new MapGestureRecognizerDelegate()
			};

			mapView.AddGestureRecognizer(_gestureRecognizer);
		}

		private void OnIsUserDragging(bool isDragging)
		{
			_isUserDragging.OnNext(isDragging);
		}

		private void DidDragMap(UIGestureRecognizer gestureRecognizer)
		{
			OnIsUserDragging(gestureRecognizer.State == UIGestureRecognizerState.Began ||
				gestureRecognizer.State == UIGestureRecognizerState.Changed);
		}

		protected void OnPushpinsSelected(IGeoLocated[] selectedContent)
		{
			_selectedPushpins.OnNext(selectedContent);
		}

#region Pushpins ICONS
		protected UIImage _icon;
		protected UIImage _selectedIcon;

		protected void UpdateIcon(ref UIImage icon, object value)
		{
			if (value == null)
			{
				icon = null;
				return;
			}

			icon = ToImageSource(value);

			if (icon == null)
			{
				_logger.Error($"Failed to convert '{value}' to a PushpinIcon");
			}
		}

		private static UIImage ToImageSource(object value)
		{
			//TODO use a centralized conversion.

			var uriStr = value as string;
			if (uriStr.HasValueTrimmed()
				&& Uri.IsWellFormedUriString(uriStr, UriKind.RelativeOrAbsolute))
			{
				value = new Uri(uriStr, UriKind.RelativeOrAbsolute);
			}

			var uri = value as Uri;
			if (uri != null)
			{
				if (!uri.IsAbsoluteUri)
				{
					return UIImage.FromBundle(uri.OriginalString);
				}

				switch (uri.Scheme.ToUpperInvariant())
				{
					case "BUNDLE":
						return UIImage.FromBundle(uri.LocalPath);

					case "FILE":
						return UIImage.FromFile(uri.LocalPath);

					case "HTTP":
					default:
						throw new NotSupportedException("Cannot use scheme '{0}' for source of icons of map".InvariantCultureFormat(uri.Scheme));
				}
			}

			var data = value as NSData;
			if (data != null)
			{
				return UIImage.LoadFromData(data);
			}

			var image = value as CGImage;
			if (image != null)
			{
				return UIImage.FromImage(image);
			}

			return value as UIImage;
		}
#endregion

		protected static Color GetColorFromKey(string key)
		{
			return (Color)ResourceHelper.FindResource(key);
		}

		private class MapGestureRecognizerDelegate : UIGestureRecognizerDelegate
		{
			public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
			{
				return true;
			}
		}
	}
}
#endif
