#if __IOS__
using System;
using System.Collections.Generic;
using System.Text;
using CoreLocation;
using Google.Maps;
using Microsoft.Extensions.Logging;
using Uno.Extensions;
using Uno.Logging;

namespace Nventive.Location.DynamicMap
{
	public class GoogleMapLayer : IMapLayer<GooglePushpin>
	{
		private readonly List<GooglePushpin> _pushpins = new List<GooglePushpin>();
		private readonly MapView _map;

		public GoogleMapLayer(MapView map)
		{
			_map = map;
		}

		public IEnumerable<GooglePushpin> Items => _pushpins;

		public void Add(GooglePushpin item)
		{
			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug("Adding a pushpin.");
			}

			SetMap(item.Marker);
			_pushpins.Add(item);

			if (this.Log().IsEnabled(LogLevel.Information))
			{
				this.Log().Info("Added a pushpin.");
			}
		}

		public void Clear()
		{
			foreach (var pin in _pushpins)
			{
				pin.Marker.Map = null;
			}
			_pushpins.Clear();
		}

		public bool Contains(GooglePushpin item) => _pushpins.Contains(item);

		public int Count() => _pushpins.Count;

		public GooglePushpin ElementAt(int index) => _pushpins[index];

		public void Insert(int index, GooglePushpin item)
		{
			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug("Inserting a pushpin.");
			}

			SetMap(item.Marker);
			_pushpins.Insert(index, item);

			if (this.Log().IsEnabled(LogLevel.Information))
			{
				this.Log().Info("Inserted a pushpin.");
			}
		}

		public bool Remove(GooglePushpin item)
		{
			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug("Removing a pushpin.");
			}

			if (_pushpins.Contains(item))
			{
				item.Marker.Map = null;
				_pushpins.Remove(item);

				if (this.Log().IsEnabled(LogLevel.Information))
				{
					this.Log().Info("Removed a pushpin.");
				}

				return true;
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			if (this.Log().IsEnabled(LogLevel.Debug))
			{
				this.Log().Debug($"Removing the pushpin at the index '{index}'.");
			}

			var item = _pushpins[index];
			item.Marker.Map = null;
			_pushpins.RemoveAt(index);

			if (this.Log().IsEnabled(LogLevel.Information))
			{
				this.Log().Info($"Removed the pushpin at the index '{index}'.");
			}
		}

		private void SetMap(Marker marker)
		{
			marker.Map = _map;

			// This works around apparent bug in SDK where coordinate doesn't update correctly if Map is set to null then back to same MapView.
			var coord = marker.Position;
			marker.Position = default(CLLocationCoordinate2D);
			marker.Position = coord;
		}
	}
}
#endif
