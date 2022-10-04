#if WINDOWS_UWP || WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !WINUI
using Windows.UI.Xaml.Controls.Maps;
#endif

namespace Cartography.DynamicMap
{
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
}
#endif
