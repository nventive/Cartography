#if WINDOWS
using Cartography.MapService;
using System.Threading;
using System.Threading.Tasks;

namespace Sample;

public class MapServiceWindows : IMapService
{
	public Task ShowDirections(CancellationToken ct, MapRequest request)
	{
		return Task.CompletedTask;
	}

	public Task ShowLocation(CancellationToken ct, MapRequest request)
	{
		return Task.CompletedTask;
	}
}
#endif