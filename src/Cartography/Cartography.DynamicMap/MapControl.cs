#if __MOBILE__ || WINDOWS

namespace Cartography.DynamicMap;

public partial class MapControl : MapControlBase
{
	public MapControl()
	{
		Initialize();
	}

	partial void Initialize();
}
#endif
