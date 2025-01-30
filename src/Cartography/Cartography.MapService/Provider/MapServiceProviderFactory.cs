#if __IOS__
using System.Collections.Generic;

namespace Cartography.MapService.Provider;

internal static class MapServiceProviderFactory
{

	public static IMapServiceProvider[] Create(MapServiceiOSProvider[] providers)
	{
		//Return the apple provider if no provider was selected
		if(providers == null)
		{
			return new IMapServiceProvider[]{ new AppleMapsServiceProvider()};
		}

		var returnProvider = new List<IMapServiceProvider>(providers.Length);

		foreach (var provider in providers)
		{
			switch (provider)
			{
				case MapServiceiOSProvider.AppleMap:
					returnProvider.Add(new AppleMapsServiceProvider());
					break;
				case MapServiceiOSProvider.GoogleMap:
					returnProvider.Add(new GoogleMapsServiceProvider());
					break;
				case MapServiceiOSProvider.Waze:
					returnProvider.Add(new WazeServiceProvider());
					break;
			}
		}

		return returnProvider.ToArray();
	}
}
#endif
