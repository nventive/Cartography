#if __IOS__ || __ANDROID__ || NETFX_CORE
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cartography.DynamicMap
{
	public partial class MapControl : MapControlBase
	{
		public MapControl()
		{
			Initialize();
		}

		partial void Initialize();
	}
}
#endif
