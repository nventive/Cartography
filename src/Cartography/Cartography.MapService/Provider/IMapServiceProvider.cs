#if __IOS__
using Foundation;

namespace Cartography.MapService;

/// <summary>
/// This contract defines a service provider which supports locations and directions services
/// </summary>
internal interface IMapServiceProvider
{
	/// <summary>
	/// Get the Provider Friendly Name
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Get the provider base URL
	/// </summary>
	NSUrl Url { get; }

	/// <summary>
	/// Requests the provider to get the url the assiociated map application to get directions to the specified destination.
	/// </summary>
	/// <param name="ct">CancellationToken</param>
	/// <param name="mapRequest">The object that contains the parmeters for the request</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	NSUrl GetDirectionsUrl(MapRequest mapRequest);

	/// <summary>
	/// Requests the provider to get the url the associated map application to show the specified location.
	/// </summary>
	/// <param name="ct">Cancellation token</param>
	/// <param name="mapRequest">The object that contains the parmeters for the request</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	NSUrl GetLocationUrl(MapRequest mapRequest);
}
#endif
