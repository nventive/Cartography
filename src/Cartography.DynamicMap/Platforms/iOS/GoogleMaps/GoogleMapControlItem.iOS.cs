#if __IOS__
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
				var coords = new CLLocationCoordinate2D(value.Coordinates.Position.Latitude, value.Coordinates.Position.Longitude);
				Marker.Position = coords;
				RaisePropertyChanged();
			}
		}

		public IGeoLocated Content => Item;

		public Marker Marker { get; }

		public event PropertyChangedEventHandler PropertyChanged;


		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
#endif