#if __IOS__
using MapKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cartography.DynamicMap;

internal class IosMapLayer : IMapLayer<Pushpin>
{
	private readonly MKMapView _control;

	public IosMapLayer(MKMapView control)
	{
		_control = control;
	}

	public IEnumerable<Pushpin> Items
	{
		get { return _control.Annotations.OfType<Pushpin>(); }
	}

	public int Count()
	{
		return _control.Annotations.OfType<Pushpin>().Count();
	}

	public void Clear()
	{
		_control.RemoveAnnotations(_control.Annotations);
	}

	public void Insert(int index, Pushpin pin)
	{
		// IOS does not handle insertion
		_control.AddAnnotation(pin);
	}

	public bool Remove(Pushpin pin)
	{
		_control.RemoveAnnotation(pin);
		return true;
	}

	public void Add(Pushpin item)
	{
		_control.AddAnnotation(item);
	}

	public Pushpin ElementAt(int index)
	{
		return (Pushpin)_control.Annotations.ElementAt(index);
	}

	public void RemoveAt(int index)
	{
		var pushpin = ElementAt(index);
		_control.RemoveAnnotation(pushpin);
	}

	public bool Contains(Pushpin item)
	{
		return _control.Annotations.Contains(item);
	}
}
#endif
