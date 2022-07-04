#if __IOS__
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using CoreLocation;
using Google.Maps;

namespace Cartography.DynamicMap
{
	public class GoogleMapControlItem : INotifyPropertyChanged, IMapControlItem
	{

		public GoogleMapControlItem()
		{
			Marker = new Marker();
		}

		private IGeoLocated _item;
		public IGeoLocated Item
		{
			get { return _item; }
			set
			{
				_item = value;
				Marker.Position = new CLLocationCoordinate2D(value.Coordinates.Position.Latitude,value.Coordinates.Position.Longitude);
				RaisePropertyChanged();
			}
		}

		public IGeoLocated Content => Item;

		public Marker Marker { get; }

		public event PropertyChangedEventHandler PropertyChanged;


		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#endif
