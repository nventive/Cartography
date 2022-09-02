using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cartography.DynamicMap
{
	public class PrettyMapViewPortEqualityComparer : IEqualityComparer<MapViewPort>
	{
		/*
		 * Values applies in calculation are based on this : 
		 * 
		 * 
		 * Zoom level		| Desired values | Result values 
		 *					| (in meters)	 | (in meters)
		 * -----------------|----------------|----------------
		 * Min = Earth	01	|				 | 4641588,834
		 *				02	|				 | 2154434,69
		 *				03	|				 | 1000000
		 *				04	|				 | 464158,8834
		 *				05	|				 | 215443,469
		 *	State		06	|	  100 000	 | 100000
		 *				07	|				 | 46415,88834
		 *				08	|				 | 21544,3469
		 *	Region		09	|	   10 000	 | 10000
		 *				10	|				 | 4641,588834
		 *				11	|				 | 2154,43469
		 *	City		12	|		1 000	 | 1000
		 *				13	|				 | 464,1588834
		 *				14	|				 | 215,443469
		 *	District	15	|		  100	 | 100
		 *				16	|				 | 46,41588834
		 *				17	|				 | 21,5443469
		 *				18	|		   10	 | 10
		 *				19	|				 | 4,641588834
		 *	Max			20	|				 | 2,15443469
		 * 
		 */

		/// <inherit />
		public bool Equals(MapViewPort x, MapViewPort y)
		{
			ILogger _logger = NullLogger.Instance;
			if (!x.ZoomLevel.HasValue || !y.ZoomLevel.HasValue)
			{
				_logger.LogWarning("PrettyMapViewPortEqualityComparer supports only comparaison of MapViewPort which has ZoomLevel.");

				throw new NotSupportedException("PrettyMapViewPortEqualityComparer supports only comparaison of MapViewPort which has ZoomLevel.");
			}

			var z1 = (double)x.ZoomLevel.Value;
			var z2 = (double)y.ZoomLevel.Value;

			// NOTE: When zooming out by tapping with 2-fingers, the zoom level is changed by 1
			// This threshold value is set to generally yield 0.9, ensuring a correct response to the zoom level change
			var threshold = (double)ZoomLevels.Maximum * 4.5 / 100;

			// If zoom level significatively changed, consider as not equals
			if (Math.Abs(z1 - z2) > threshold)
			{
				return false;
			}

			var zoomLevel = (z1 + z2) / 2;
			var minDistance = 10000000 / (Math.Pow(10, zoomLevel / 3));
			var distance = x.Center.Position.GetDistanceTo(y.Center.Position);

			return distance < minDistance;
		}

		/// <inherit />
		public int GetHashCode(MapViewPort obj)
		{
			// Always return same value to ensure to call the Equals method

			return -1;
		}
	}
}
