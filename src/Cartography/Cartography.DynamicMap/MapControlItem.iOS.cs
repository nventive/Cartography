#if __IOS__
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CoreLocation;
using MapKit;
using Windows.Devices.Geolocation;

namespace Cartography.DynamicMap
{
	public class MapControlItem : MKAnnotation, INotifyPropertyChanged, IMapControlItem
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private CLLocationCoordinate2D _coordinate;
		private IGeoLocated _content;

		public IGeoLocated Content
		{
			get { return _content; }
			set
			{
				_content = value;
				RaisePropertyChanged();
			}
		}

		IGeoLocated IMapControlItem.Item
		{
			get { return Item; }
			set { Item = value; }
		}

		internal IGeoLocated Item
		{
			get { return (IGeoLocated)Content; }
			set
			{
				SetCoordinates(ToCLCoordinates(value.Coordinates));
				Content = value;
			}
		}

		public override CLLocationCoordinate2D Coordinate
		{
			get
			{
				return _coordinate;
			}
		}

		private void SetCoordinates(CLLocationCoordinate2D value)
		{
			WillChangeValue("coordinate");
			_coordinate = value;
			DidChangeValue("coordinate");
			RaisePropertyChanged();
		}

		protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		internal MapControl Map { get; set; }

		private CLLocationCoordinate2D ToCLCoordinates(Geopoint geoCoordinate)
		{
			return new CLLocationCoordinate2D(
				geoCoordinate.Position.Latitude,
				geoCoordinate.Position.Longitude
				);
		}
	}
}
#endif
