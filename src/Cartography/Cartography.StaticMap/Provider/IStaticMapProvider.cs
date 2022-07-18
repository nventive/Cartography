#if NETFX_CORE || __IOS__ || __ANDROID__
using System.Threading;
using System.Threading.Tasks;

namespace Cartography.StaticMap.Provider
{
	/// <summary>
	/// Provides a static map to display.
	/// The resulting static map may be an image, a url or other.
	/// </summary>
	internal interface IStaticMapProvider
	{
		/// <summary>
		/// Get a map with the specified parameters.
		/// </summary>
		/// <param name="ct">Cancellation token</param>
		/// <param name="parameters">Map parameters</param>
		/// <returns>An image or a mapview.</returns>
		Task<object> GetMap(CancellationToken ct, StaticMapParameters parameters);
	}
}
#endif
