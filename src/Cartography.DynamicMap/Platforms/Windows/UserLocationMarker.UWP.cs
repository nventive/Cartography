#if WINDOWS && false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace Cartography.DynamicMap;

/// <summary>
/// Marker for the User Location
/// </summary>
public class UserLocationMarker : MapControlItem
{
	internal UserLocationMarker(MapControlBase map) 
		: base(map)
	{
	}
}
#endif
