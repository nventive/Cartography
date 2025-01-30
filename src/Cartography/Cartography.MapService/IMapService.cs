using System.Threading;
using System.Threading.Tasks;

namespace Cartography.MapService;

/// <summary>
/// This contract defines a service dedicated to platform-specific directions displaying.
/// </summary>
public interface IMapService
{
	/// <summary>
	/// Opens the maps application to get directions to the specified destination.
	/// </summary>
	/// <param name="ct">CancellationToken</param>
	/// <param name="request">MapRequest</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	Task ShowDirections(CancellationToken ct, MapRequest request);

	/// <summary>
	/// Opens the maps application to show the specified location.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <param name="request">The object that contains the parmeters for the req</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	Task ShowLocation(CancellationToken ct, MapRequest request);
}
