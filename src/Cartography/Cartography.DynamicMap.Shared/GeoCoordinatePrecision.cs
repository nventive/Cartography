using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cartography.DynamicMap
{
	/// <summary>
	/// Source: http://gis.stackexchange.com/questions/8650/how-to-measure-the-accuracy-of-latitude-and-longitude
	/// The value of the enum represents the number of digits that will be kept.
	/// </summary>
	public enum GeoCoordinatePrecision
	{
		HundredsOfKilometers = 0,
		TensOfKilometers = 1,
		Kilometers = 2,
		HundredsOfMeters = 3,
		TensOfMeters = 4,
		Meters = 5,
		Centimeters = 6,
		Millimeters = 7
	}
}
