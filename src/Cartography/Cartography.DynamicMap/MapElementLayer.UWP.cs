#if NETFX_CORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using Uno.Extensions;
using Uno.Logging;
using Microsoft.Extensions.Logging;
using Windows.Devices.Geolocation;

namespace Nventive.Location.DynamicMap
{
	internal class MapControlIconItem : IMapControlItem, ISelectable
	{
		private readonly MapIconLayer _layer;
		private readonly Windows.UI.Xaml.Controls.Maps.MapControl _mapPresenter;

		private IGeoLocated _item;
		private MapIcon _icon;
		private MapIcon _selectedIcon;
		private bool _isSelected;
		private int _zIndex;

		private MapIcon GetOrCreateIcon()
		{
			if (_icon == null)
			{
				_icon = new MapIcon
				{
					CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
					Image = _layer.Icon,
					NormalizedAnchorPoint = _layer.IconOrigin,
					Visible = !IsSelected,
					ZIndex = ZIndex,
				};
				Item.Maybe(i => _icon.Location = _item.Coordinates);
				_mapPresenter.MapElements.Add(_icon);
			}

			return _icon;
		}

		private MapIcon GetOrCreateSelectedIcon()
		{
			if (_selectedIcon == null)
			{
				_selectedIcon = new MapIcon
				{
					CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
					Image = _layer.SelectedIcon,
					NormalizedAnchorPoint = _layer.IconOrigin,
					Visible = IsSelected,
					ZIndex = ZIndex,
				};
				Item.Maybe(i => _selectedIcon.Location = _item.Coordinates);
				_mapPresenter.MapElements.Add(_selectedIcon);
			}

			return _selectedIcon;
		}

		public MapControlIconItem(MapIconLayer layer, Windows.UI.Xaml.Controls.Maps.MapControl mapPresenter)
		{
			_layer = layer;
			_mapPresenter = mapPresenter;
		}

		public IGeoLocated Item
		{
			get { return _item; }
			set
			{
				_item = value;

				Recyle();

				if (IsSelected)
				{
					_icon.Maybe(i => i.Location = _item.Coordinates);
					GetOrCreateSelectedIcon().Location = _item.Coordinates;
				}
				else
				{
					GetOrCreateIcon().Location = _item.Coordinates;
					_selectedIcon.Maybe(i => i.Location = _item.Coordinates);
				}
			}
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				_isSelected = value;

				if (value)
				{
					GetOrCreateSelectedIcon().Visible = true;
					_icon.Maybe(i => i.Visible = false);
				}
				else
				{
					GetOrCreateIcon().Visible = true;
					_selectedIcon.Maybe(i => i.Visible = false);
				}
			}
		}

		public int ZIndex
		{
			get { return _zIndex; }
			set
			{
				_zIndex = value;

				_icon.Maybe(i => i.ZIndex = value);
				_selectedIcon.Maybe(i => i.ZIndex = value);
			}
		}

		public bool IsOwnerOf(MapElement element)
		{
			return element != null
				&& (_icon == element || _selectedIcon == element);
		}

		public void Delete()
		{
			_icon.Maybe(i => _mapPresenter.MapElements.Remove(i));
			_selectedIcon.Maybe(i => _mapPresenter.MapElements.Remove(i));
		}

		private void Recyle()
		{
			_layer.Recycle(this);

			if (_icon != null)
			{
				_mapPresenter.MapElements.AddDistinct(_icon);
			}

			if (_selectedIcon != null)
			{
				_mapPresenter.MapElements.AddDistinct(_selectedIcon);
			}
		}
	}

	internal class MapIconLayer : IMapLayer<MapControlIconItem>, IDisposable
	{
		private readonly List<MapControlIconItem> _items = new List<MapControlIconItem>();
		private readonly List<MapControlIconItem> _selectedItems = new List<MapControlIconItem>();
		private readonly ISubject<IGeoLocated[]> _observeSelected = new Subject<IGeoLocated[]>();

		private readonly Windows.UI.Xaml.Controls.Maps.MapControl _map;
		private readonly MapSelectionMode _selectionMode;

		internal IRandomAccessStreamReference Icon { get; }
		internal IRandomAccessStreamReference SelectedIcon { get; }

		internal Point IconOrigin { get; }

		public MapIconLayer(Windows.UI.Xaml.Controls.Maps.MapControl map, Point iconOrigin, IRandomAccessStreamReference icon)
			: this(map, iconOrigin, icon, null, MapSelectionMode.None)
		{
		}

		public MapIconLayer(
			Windows.UI.Xaml.Controls.Maps.MapControl map,
			Point iconOrigin,
			IRandomAccessStreamReference icon,
			IRandomAccessStreamReference selectedIcon,
			MapSelectionMode selectionMode)
		{
			_map = map;
			IconOrigin = iconOrigin;
			_selectionMode = selectionMode;

			Icon = icon;
			SelectedIcon = selectedIcon;

			if (_selectionMode != MapSelectionMode.None)
			{
				_map.MapElementClick += OnElementClick;
			}
		}

		public IObservable<IGeoLocated[]> ObserveSelected()
		{
			return _observeSelected;
		}

		private void OnElementClick(Windows.UI.Xaml.Controls.Maps.MapControl sender, MapElementClickEventArgs args)
		{
			var element = args.MapElements.FirstOrDefault();
			var item = _items.FirstOrDefault(i => i.IsOwnerOf(element));

			if (item != null)
			{
				var wasSelected = item.IsSelected;
				item.IsSelected = !wasSelected;

				if (wasSelected)
				{
					if (!_selectedItems.Remove(item))
					{
						if (this.Log().IsEnabled(LogLevel.Error))
						{
							this.Log().Error("Unselect an item which should not be selected !!");
						}						
					}
				}
				else
				{
					if (_selectedItems.Contains(item))
					{
						return; // Pin is already in list, do nothing (Sanity check)
					}

					switch (_selectionMode)
					{
						case MapSelectionMode.Single:
							for (var i = _selectedItems.Count - 1; i >= 0; i--)
							{
								_selectedItems[i].IsSelected = false;
								_selectedItems.RemoveAt(i);
							}
							break;

						case MapSelectionMode.Multiple:
							break;

						default:
							throw new InvalidOperationException("Cannot use selection mode {0}".InvariantCultureFormat(_selectionMode));
					}

					_selectedItems.Add(item);
				}

				_observeSelected.OnNext(_selectedItems.Select(i => i.Item).ToArray());
			}
		}

		public void Recycle(MapControlIconItem item)
		{
			item.IsSelected = false;
			_selectedItems.Remove(item);
		}

		public IEnumerable<MapControlIconItem> Items => _items;

		public int Count()
		{
			return _items.Count;
		}

		public void Clear()
		{
			_items.ForEach(i => i.Delete());
			_items.Clear();
		}

		public void Insert(int index, MapControlIconItem item)
		{
			_items.Add(item);
		}

		public bool Remove(MapControlIconItem item)
		{
			if (_items.Remove(item))
			{
				item.Delete();
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Add(MapControlIconItem item)
		{
			_items.Add(item);
		}

		public MapControlIconItem ElementAt(int index)
		{
			return _items.ElementAt(index);
		}

		public void RemoveAt(int index)
		{
			var item = ElementAt(index);
			item.Delete();
			_items.Remove(item);
		}

		public bool Contains(MapControlIconItem item)
		{
			return _items.Contains(item);
		}

		public void Dispose()
		{
			_map.MapElementClick -= OnElementClick;
		}
	}
}
#endif
