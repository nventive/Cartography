﻿using System;
using System.ComponentModel;
using System.Linq;
using Uno.Extensions;
using Windows.Devices.Geolocation;

namespace Cartography.StaticMap
{
	/// <summary>
	/// Represents all information about the view port of a map.
	/// </summary>
	public class MapViewPort
	{
		private const int MAXIMUM_LONGITUDE = 90;
		private const int MINIMUM_LONGITUDE = -90;
		private const int MAXIMUM_LATITUDE = 180;

		/// <summary>
		/// Default ctor
		/// </summary>
		public MapViewPort()
		{
		}

		/// <summary>
		/// Instanciate a MapViewPort with its center
		/// </summary>
		/// <param name="center">The center</param>
		public MapViewPort(Geopoint center)
		{
			Center = center;
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="source"></param>
		public MapViewPort(MapViewPort source)
		{
			Center = source.Center;
			ZoomLevel = source.ZoomLevel;
			Heading = source.Heading;
			Pitch = source.Pitch;
		}

		/// <summary>
		/// The center of the map.
		/// </summary>
		public Geopoint Center { get; set; }

		/// <summary>
		/// The zoom level to be set on the MapControl. This property should not be set, or should be ignored if <see cref="PointsOfInterest"/> is set.
		/// Please see <see cref="ZoomLevels"/> for more information.
		/// </summary>
		public ZoomLevel? ZoomLevel { get; set; }

		/// <summary>
		/// DO NOT USE - Stud field only used to support deserialization since ZoomLevel is a struct.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Do not use this property, use ZoomLevel instead, this one is simply to support deserialization.", error: true)]
		public double? ZoomLevelValue
		{
			get { return (double?)ZoomLevel; }
			set { ZoomLevel = (ZoomLevel?)value; }
		}

		/// <summary>
		/// Defines an optional list of Points of Interest to be used by the MapControl to change the displayed map viewport ( auto zoom ).
		/// </summary>
		public Geopoint[] PointsOfInterest { get; set; }


		/// <summary>
		/// Gets or sets the heading.
		/// </summary>
		public double? Heading { get; set; }

		/// <summary>
		/// Gets or sets the pitch
		/// </summary>
		public double? Pitch { get; set; }

		/// <summary>
		/// Disables any animation that the platform map control offers when changing viewport
		/// </summary>
		public bool IsAnimationDisabled { get; set; }


		/// <inherit />
		//According to original dev, there was a problem in using the EqualityExtension. But there is no
		//traces why, and he can't recall.
		public override int GetHashCode()
		{
			return Center.GetHashCode()
				^ ZoomLevel.GetHashCode()
				^ this.Equality().GetHashCode(PointsOfInterest.Safe())
				^ Heading.GetHashCode()
				^ Pitch.GetHashCode();
		}

		/// <inherit />
		//According to original dev, there was a problem in using the EqualityExtension. But there is no
		//traces why, and he can't recall.
		public override bool Equals(object obj)
		{
			var other = obj as MapViewPort;

			return other != null
				&& Center.Equals(other.Center)
				&& ZoomLevel.GetValueOrDefault((ZoomLevel)double.NaN).Equals(other.ZoomLevel.GetValueOrDefault((ZoomLevel)double.NaN))
				&& PointsOfInterest.SafeSequenceEqual(other.PointsOfInterest)
				&& Heading.GetValueOrDefault(double.NaN).Equals(other.Heading.GetValueOrDefault(double.NaN))
				&& Pitch.GetValueOrDefault(double.NaN).Equals(other.Pitch.GetValueOrDefault(double.NaN));
		}

		public override string ToString()
		{
			return "[MapViewPort] Center: {0}, Zoom: {1}, POIs: {2}".InvariantCultureFormat(
				Center,
				ZoomLevel,
				string.Join(",", PointsOfInterest.Safe()));
		}
	}
}
