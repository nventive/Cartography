#if __ANDROID__
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Android.Gms.Maps.Model;
using Windows.Devices.Geolocation;

namespace Cartography.DynamicMap
{
	public class MapControlItem : INotifyPropertyChanged, IMapControlItem
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private IGeoLocated _content;

		public MapControlItem()
		{
			Options = new MarkerOptions();
		}

		public IGeoLocated Content
		{
			get { return _content; }
			set
			{
				_content = value;
				RaisePropertyChanged();
			}
		}

		protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		internal MapControlBase Map { get; set; }

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
				Options.SetPosition(new LatLng(value.Coordinates.Position.Latitude,value.Coordinates.Position.Longitude));
				Content = value;
			}
		}

		public MarkerOptions Options { get; protected set; }

		private Marker _marker;

		public Marker Marker
		{
			get { return _marker; }
			set
			{
				_marker = value;
				RaisePropertyChanged();
			}
		}

	}

	public class Pushpin : MapControlItem, ISelectable
	{
		private bool _isSelected;
		private int _zIndex;
		private Action<Pushpin, Android.Gms.Maps.Model.Marker> _markerUpdater;

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					RaisePropertyChanged();
				}
			}
		}

		public int ZIndex
		{
			get { return _zIndex; }
			set
			{
				if (_zIndex != value)
				{
					_zIndex = value;
					RaisePropertyChanged();
				}
			}
		}

		public Action<Pushpin, Marker> MarkerUpdater
		{
			get { return _markerUpdater; }
			set
			{
				_markerUpdater = value;
				if (_markerUpdater != null)
				{
					Options = Options.Visible(false);
				}
			}
		}

		protected override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			base.RaisePropertyChanged(propertyName);

			if (MarkerUpdater != null && Marker != null)
			{
				MarkerUpdater(this, Marker);
				Marker.Visible = true;
			}
		}
	}
}
#endif
