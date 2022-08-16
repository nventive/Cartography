#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Uno.Extensions;
using Uno.Logging;

namespace Cartography.DynamicMap
{
	internal class GoogleMapLayer : IMapLayer<Pushpin>
	{
		private readonly Dictionary<Pushpin, Marker> _inner = new Dictionary<Pushpin, Marker>();
		private readonly Dictionary<Marker, Pushpin> _reverse = new Dictionary<Marker, Pushpin>(new MarkerComparer());
		private readonly GoogleMap _map;
		private readonly ILogger _logger;

		public GoogleMapLayer(GoogleMap map, ILogger logger = null)
		{
			_map = map;
			_logger = logger ?? NullLogger.Instance;
		}

		public IEnumerable<Pushpin> Items
		{
			get { return _inner.Keys; }
		}

		public Pushpin FindPushPin(Marker marker)
		{
			return _reverse.UnoGetValueOrDefault(marker);
		}

		public int Count()
		{
			return _inner.Count;
		}

		public void Clear()
		{
			_logger.Debug("Clearing all pushpins.");

			var markers = _reverse.Keys.ToArray();

			_inner.Clear();
			_reverse.Clear();

			// We cannot clear the map, as it might contain other glyphs, like polygons.
			foreach(var marker in markers)
			{
				marker.Remove();
			}

			_logger.Info("Cleared all pushpins.");
		}

		public void Insert(int index, Pushpin item)
		{
			Add(item);
		}

		public bool Remove(Pushpin item)
		{
			
			_logger.Debug("Removing a pushpin.");
			
			var marker = _inner.GetValueOrDefaultAndRemove(item);
			if (marker == null)
			{
				return false;
			}
			else
			{
				_reverse.Remove(marker);
				marker.Remove();

				_logger.Info("Removed a pushpin.");

				return true;
			}
		}

		public void Add(Pushpin item)
		{
			_logger.Debug("Adding a pushpin.");

			var marker = _map.AddMarker(item.Options);
			item.Marker = marker;

			_inner.Add(item, marker);
			_reverse.Add(marker, item);

			_logger.Info("Added a pushpin.");
		}

		public Pushpin ElementAt(int index)
		{
			return _inner.ElementAt(index).Key;
		}

		public void RemoveAt(int index)
		{
			_logger.Warn("Cannot remove an item at a specific index on GoogleMap.");

			throw new NotSupportedException("Cannot remove an item at a specific index on GoogleMap.");
		}

		public bool Contains(Pushpin item)
		{
			return _inner.ContainsKey(item);
		}

		/// <summary>
		/// This comparer is required as returned marker instances are of
		/// difference references for the same value.
		/// </summary>
		private class MarkerComparer : IEqualityComparer<Marker>
		{
			public bool Equals(Marker x, Marker y)
			{
				var equal = x.Equals(y);
				return equal;
			}

			public int GetHashCode(Marker obj)
			{
				var hash = obj.GetHashCode();
				return hash;
			}
		}
	}
}
#endif
