#if NETFX_CORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GeolocatorService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Cartography.DynamicMap.Helpers;

namespace Cartography.DynamicMap
{
	public partial class MapControlBase : Control
	{
		private Windows.UI.Xaml.Controls.Maps.MapControl _map;
		private ContentPresenter _errorPresenter;
		private MapLayer<Pushpin> _pushpins;
		private MapIconLayer _pushpinIcons;
		private MapLayer<UserLocationMarker> _userLocation;

		#region PushpinCommand (dp)
		/// <summary>
		/// Identifies the <see cref="PushpinCommand"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty PushpinCommandProperty = DependencyProperty.Register(
			"PushpinCommand", typeof(ICommand), typeof(MapControlBase), new PropertyMetadata(default(ICommand)));

		/// <summary>
		/// Command executed each time a pushpin is selected
		/// </summary>
		public ICommand PushpinCommand
		{
			get { return (ICommand)GetValue(PushpinCommandProperty); }
			set { SetValue(PushpinCommandProperty, value); }
		}
		#endregion

		/// <summary>
		/// ctor.
		/// </summary>
		partial void PartialConstructor(ILogger<MapControlBase> logger = null)
		{
			_logger = logger ?? NullLogger<MapControlBase>.Instance;
			DefaultStyleKey = typeof(MapControlBase);

			Loaded += (snd, e) => TryStart();
			Unloaded += (snd, e) => Pause();
		}

		#region View
		/// <inherit />
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_map = (this.GetTemplateChild(_mapPartName) as Windows.UI.Xaml.Controls.Maps.MapControl).Validation().NotNull(_mapPartName);
			_errorPresenter = (this.GetTemplateChild(_errorPresenterPartName) as ContentPresenter).Validation().NotNull(_errorPresenterPartName);

			if (UseIcons)
			{
				_pushpinIcons = new MapIconLayer(_map, PushpinIconsPositionOrigin, _icon, _selectedIcon, SelectionMode);
			}
			else
			{
				_pushpins = new MapLayer<Pushpin>(_map);
			}

			_map.PointerPressed -= OnPointerPressed;
			_map.PointerReleased -= OnPointerReleased;

			_map.PointerPressed += OnPointerPressed;
			_map.PointerReleased += OnPointerReleased;

			_userLocation = new MapLayer<UserLocationMarker>(_map);

			// Sepcify that the view is ready, so the control might begin begin its work.
			_isReady = true;

			TryStart();
		}

		private void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			_isUserDragging.OnNext(false);
		}

		private void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			_isUserDragging.OnNext(true);
		}
		#endregion

		#region User location
		private void UpdateMapUserLocation(LocationResult locationAndStatus)
		{
			if (locationAndStatus == null)
				return;

			if (locationAndStatus.IsSuccessful)
			{
				var marker = _userLocation.Items.SingleOrDefault();
				if (marker == null)
				{
					_userLocation.Add(marker = new UserLocationMarker(this));
				}

				marker.UpdateLocation(locationAndStatus.Location.Point);
			}
			else
			{
				_userLocation.Clear();
			}
		}
		#endregion

		#region View port

		private IEnumerable<IObservable<Unit>> GetViewPortChangedTriggers()
		{
			yield return _map.ObserveCenterChanged().Where(_ => !_isAnimating);
			yield return _map.ObserveHeadingChanged().Where(_ => !_isAnimating);
			yield return _map.ObservePitchChanged().Where(_ => !_isAnimating);
			yield return _map.ObserveZoomLevelChanged().Where(_ => !_isAnimating);
		}

		private MapViewPort GetViewPort()
		{
			return new MapViewPort(_map.Center)
			{
				Heading = _map.Heading,
				Pitch = _map.Pitch,
				ZoomLevel = (ZoomLevel)_map.ZoomLevel
			};
		}

		private bool GetInitializationStatus() => true;

		private async Task SetViewPort(CancellationToken ct, MapViewPort viewPort)
		{
			if (viewPort.PointsOfInterest != null && viewPort.PointsOfInterest.Any() && viewPort.Center != default(Geopoint))
			{
				var bounds = AddPushpinPaddingToBounds(viewPort);

				await _map.TrySetViewBoundsAsync(bounds, null, viewPort.IsAnimationDisabled ? MapAnimationKind.None : MapAnimationKind.Default);
			}
			else
			{
				await _map
					.TrySetViewAsync(viewPort.Center, (double?)viewPort.ZoomLevel, viewPort.Heading, viewPort.Pitch, viewPort.IsAnimationDisabled ? MapAnimationKind.None : MapAnimationKind.Default)
					.AsTask(ct);
			}
		}

		private GeoboundingBox AddPushpinPaddingToBounds(MapViewPort viewPort)
		{
			var frontiers = viewPort.GetBounds();

			// create ViewPort with calculated dimensions
			var northWestCorner = new BasicGeoposition() { Latitude = frontiers.WestFrontier, Longitude = frontiers.NorthFrontier };
			var southEastCorner = new BasicGeoposition() { Latitude = frontiers.EastFrontier, Longitude = frontiers.SouthFrontier };

			return new GeoboundingBox(northWestCorner, southEastCorner);
		}
		#endregion

		#region View port coordinates
		private MapViewPortCoordinates GetViewPortCoordinates()
		{
			var visibleRegionBounds = _map.GetVisibleRegion(MapVisibleRegionKind.Full);
			return new MapViewPortCoordinates(
				northEast: new BasicGeoposition
				{
					Latitude = visibleRegionBounds.Positions.Max(p => p.Latitude),
					Longitude = visibleRegionBounds.Positions.Max(p => p.Longitude)
				},
				northWest: new BasicGeoposition
				{
					Latitude = visibleRegionBounds.Positions.Max(p => p.Latitude),
					Longitude = visibleRegionBounds.Positions.Min(p => p.Longitude)
				},
				southWest: new BasicGeoposition
				{
					Latitude = visibleRegionBounds.Positions.Min(p => p.Latitude),
					Longitude = visibleRegionBounds.Positions.Min(p => p.Longitude)
				},
				southEast: new BasicGeoposition
				{
					Latitude = visibleRegionBounds.Positions.Min(p => p.Latitude),
					Longitude = visibleRegionBounds.Positions.Max(p => p.Longitude)
				}
			);
		}
		#endregion

		private void UpdateMapPushpins(IGeoLocated[] items, IGeoLocated[] selectedItems)
		{
			if (_pushpins == null)
			{
				_pushpinIcons.Update(items, selectedItems, containerFactory: i => new MapControlIconItem(_pushpinIcons, _map) { Item = i });
			}
			else
			{
				_pushpins.Update(items, selectedItems, containerFactory: i => new Pushpin(this) { Item = i });
			}
		}

		#region Pushpins ICONS
		private bool UseIcons => _icon != null;

		private RandomAccessStreamReference _icon;
		private RandomAccessStreamReference _selectedIcon;

		private void UpdateIcon(object icon)
		{
			if (_icon != null)
			{
				throw new InvalidOperationException("Pushpins icons cannot be changed.");
			}

			UpdateIcon(ref _icon, icon);
		}

		private void UpdateSelectedIcon(object icon)
		{
			if (_selectedIcon != null)
			{
				throw new InvalidOperationException("Pushpins icons cannot be changed.");
			}

			UpdateIcon(ref _selectedIcon, icon);
		}

		private static void UpdateIcon(ref RandomAccessStreamReference icon, object value)
		{
			if (value == null)
			{
				icon = null;
				return;
			}

			icon = ToImageSource(value);
			if (icon == null)
			{
				throw new InvalidOperationException("Failed to convert '{0}' to a PushpinIcon".InvariantCultureFormat(value));
			}
		}

		private static RandomAccessStreamReference ToImageSource(object value)
		{
			var uriStr = value as string;
			if (uriStr.HasValueTrimmed()
				&& Uri.IsWellFormedUriString(uriStr, UriKind.RelativeOrAbsolute))
			{
				value = new Uri(uriStr, UriKind.RelativeOrAbsolute);
			}

			var uri = value as Uri;
			if (uri != null)
			{
				// CreateFromUri supports only absolute URIs, so make relative URIs to the application package
				if (!uri.IsAbsoluteUri)
				{
					uri = new Uri("ms-appx:///" + uri.OriginalString.TrimStart('/', '\\'));
				}

				return RandomAccessStreamReference.CreateFromUri(uri);
			}

			var file = value as IStorageFile;
			if (file != null)
			{
				return RandomAccessStreamReference.CreateFromFile(file);
			}

			var stream = value as IRandomAccessStream;
			if (stream != null)
			{
				return RandomAccessStreamReference.CreateFromStream(stream);
			}

			return null;
		}
		#endregion

		#region Selection
		private void UpdateMapSelectedPushpins(IGeoLocated[] items)
		{
			if (_pushpins == null)
			{
				_pushpinIcons.UpdateSelection(items);
			}
			else
			{
				_pushpins.UpdateSelection(items);
			}
		}

		/// <summary>
		/// Toggle selection state of a single pushpin (e.g. when user tap on a puhpin)
		/// </summary>
		/// <param name="pushpin"></param>
		/// <returns>Indicates if select has an effect or not</returns>
		internal bool ToggleSelection(Pushpin pushpin)
		{
			if (!_pushpins.Contains(pushpin))
			{
				return false;
			}

			var shouldSelect = !pushpin.IsSelected;

			// Execute command even if selection mode is none
			var isCommandExecuted = false;
			if (shouldSelect)
			{
				var command = PushpinCommand;
				if (command != null && command.CanExecute(pushpin.Content))
				{
					command.Execute(pushpin.Content);
					isCommandExecuted = true;
				}
			}

			var selectionMode = SelectionMode;
			IGeoLocated[] selectedItems;
			switch (selectionMode)
			{
				case MapSelectionMode.None:
					return isCommandExecuted;

				case MapSelectionMode.Single:
				default:
					foreach (var container in _pushpins.Items)
					{
						container.IsSelected = false;
					}
					selectedItems = shouldSelect
						? new[] { pushpin.Item }
						: new IGeoLocated[0];
					break;

				case MapSelectionMode.Multiple:
					selectedItems = _pushpins
						.Items
						.Where(c => c.IsSelected)
						.Select(c => c.Item)
						.Concat((IEnumerable<IGeoLocated>)pushpin.Item)
						.ToArray();
					break;
			}

			if (shouldSelect)
			{
				_pushpins.Remove(pushpin);
				_pushpins.Add(pushpin);
			}

			// Update the is selected only after remove/add (to ensure to get the animation)
			pushpin.IsSelected = shouldSelect;

			// Finally publish to VM
			_selectedPushpins.OnNext(selectedItems);
			return true;
		}
		#endregion

	}
}
#endif
