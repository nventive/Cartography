﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uno.Extensions;

namespace Cartography.DynamicMap;

public class GeoLocatedGrouping<T> : IGeoLocatedGrouping<T>
{
	private IGrouping<Geocoordinate, T> _source;

	public GeoLocatedGrouping(IGrouping<Geocoordinate, T> source)
	{
		_source = source.Validation().NotNull("source");
	}

	public Geopoint Coordinates
	{
		get { return Key.Point; }
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
