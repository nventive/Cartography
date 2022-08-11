using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeolocatorService;
using System.Text;
using Windows.Devices.Geolocation;
using Cartography.DynamicMap;

namespace Samples.Entities
{
	public class PushpinEntity : IGeoLocated
	{
		public Geopoint Coordinates { get; set; }

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

			if (obj != null && other != null)
			{
				return other.Coordinates.Equals(Coordinates);
			}

			return false;
		}

		public override string ToString()
		{
			return $"{Coordinates.Position.Latitude}, {Coordinates.Position.Longitude}";
		}
	}
}
