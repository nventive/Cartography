using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Text;

namespace Umbrella.Location.Samples.Uno
{
	public class PushpinEntity : IGeoLocated, INotifyPropertyChanged
	{
		public GeoCoordinate Coordinates { get; set; }

		public string Name { get; set; }

		public string Address { get; set; }

		public string City
		{
			get
			{
				return "Montreal";
			}
		}

		public string Province
		{
			get
			{
				return "QC";
			}
		}

		public string Zipcode
		{
			get
			{
				return "HHH HHH";
			}
		}

		public string Country
		{
			get
			{
				return "Canada";
			}
		}

		private bool _isTileSelected;

		public bool IsTileSelected
		{
			get => _isTileSelected;
			set
			{
				if (value != _isTileSelected)
				{
					_isTileSelected = value;
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsTileSelected)));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public override int GetHashCode() => Coordinates.GetHashCode();

		/// We must override Equals for the pushpin properly. Otherwise, they will flicker every time you change one of the pushpins.
		public override bool Equals(object obj)
		{
			var other = obj as PushpinEntity;

			if (obj != null && other.Coordinates != null)
			{
				return other.Coordinates.Equals(Coordinates);
			}

			return false;
		}

		public override string ToString()
		{
			return $"{Name}: {Coordinates}";
		}
	}
}
