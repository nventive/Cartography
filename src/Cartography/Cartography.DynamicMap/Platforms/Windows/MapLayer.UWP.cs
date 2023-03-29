#if WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cartography.DynamicMap
{
	/// <summary>
	/// Represents a layer of item on the map
	/// </summary>
	internal class MapLayer<T> : IMapLayer<T> 
		where T : MapControlItem
	{
		private readonly List<T> _elements = new List<T>();
		private readonly Windows.UI.Xaml.Controls.Maps.MapControl _map;

		/// <summary>
		/// ctor.
		/// </summary>
		public MapLayer(Windows.UI.Xaml.Controls.Maps.MapControl map)
		{
			_map = map;
		}

		/// <inherit />
		public IEnumerable<T> Items => _elements;

		/// <inherit />
		public int Count()
		{
			return _elements.Count;
		}

		/// <inherit />
		public void Clear()
		{
			foreach (var element in _elements)
			{
				_map.Children.Remove(element);
			}

			_elements.Clear();
		}

		/// <inherit />
		public void Insert(int index, T item)
		{
			_elements.Insert(index, item);
			_map.Children.Insert(index, item);
		}

		/// <inherit />
		public bool Remove(T item)
		{
			return _elements.Remove(item)
			       && _map.Children.Remove(item);
		}

		/// <inherit />
		public void Add(T item)
		{
			_elements.Add(item);
			_map.Children.Add(item);
		}

		/// <inherit />
		public T ElementAt(int index)
		{
			return _elements.ElementAt(index);
		}

		/// <inherit />
		public void RemoveAt(int index)
		{
			_elements.RemoveAt(index);
			_map.Children.RemoveAt(index);
		}

		/// <inherit />
		public bool Contains(T item)
		{
			return _elements.Contains(item);
		}
	}
}
#endif
