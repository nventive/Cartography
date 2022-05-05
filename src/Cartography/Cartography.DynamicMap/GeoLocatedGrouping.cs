using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uno.Extensions;
using Windows.Devices.Geolocation;

namespace Nventive.Location.DynamicMap
{
	public class GeoLocatedGrouping<T> : IGeoLocatedGrouping<T>
	{
		private IGrouping<Geocoordinate, T> _source;

		public GeoLocatedGrouping(IGrouping<Geocoordinate, T> source)
		{
			_source = source.Validation().NotNull("source");
		}

		public Geopoint Coordinates
		{
			get { return _source.Key.Point; }
		}

		public Geocoordinate Key
		{
			get { return _source.Key; }
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _source.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _source.GetEnumerator();
		}
	}
}
