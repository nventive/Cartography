using GeolocatorService;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Presentation;

public static class GeolocatorServiceExtensions
{
	/// <summary>
	/// Observes the user's location.
	/// </summary>
	public static IObservable<Geocoordinate> ObserveLocation(this IGeolocatorService service)
	{
		return Observable.FromEventPattern<LocationChangedEventHandler, LocationChangedEventArgs>(
			h => service.LocationChanged += h,
			h => service.LocationChanged -= h
		)
		.Select(p => p.EventArgs.Coordinate);
	}

	/// <summary>
	/// Gets and observes the user's location (or null if it's unavailable). Requests the permission if this was not already done.
	/// The location becomes null as soon as the location permission is denied.
	/// </summary>
	public static IObservable<Geocoordinate> GetAndObserveLocationOrDefault(IGeolocatorService service)
	{
		var initialLocationOrNull = Observable.Create<Geocoordinate>(service.GetInitialLocationOrDefault);
		var locationObservable = ObserveLocation(service);
		var locationUnavailableObservable = ObserveIsPermissionGranted(service)
			.Where(isGranted => !isGranted)
			.Select(_ => default(Geocoordinate));

		return Observable.Merge(initialLocationOrNull, locationObservable, locationUnavailableObservable);
	}

	private static async Task GetInitialLocationOrDefault(this IGeolocatorService service, IObserver<Geocoordinate> observer, CancellationToken ct)
	{
		var isPermissionGranted = await service.RequestPermission(ct);
		var currentLocationOrNull = isPermissionGranted ? await service.GetLocationOrDefault(ct) : null;
		observer.OnNext(currentLocationOrNull);
	}

	/// <summary>
	/// Observes the location permission.
	/// </summary>
	public static IObservable<bool> ObserveIsPermissionGranted(this IGeolocatorService service)
	{
		return Observable.FromEventPattern<LocationPermissionChangedEventHandler, LocationPermissionChangedEventArgs>(
			h => service.LocationPermissionChanged += h,
			h => service.LocationPermissionChanged -= h
		)
		.Select(p => p.EventArgs.IsLocationPermissionGranted);
	}

	/// <summary>
	/// Gets and observes the location permission. Requests the permission if this was not already done.
	/// </summary>
	public static async Task<IObservable<bool>> GetAndObserveIsPermissionGranted(this IGeolocatorService service, CancellationToken ct)
	{
		var isPermissionGranted = await service.RequestPermission(ct);

		return ObserveIsPermissionGranted(service).StartWith(isPermissionGranted);
	}

	/// <summary>
	/// Gets the current location. Does not throw an <see cref="InvalidOperationException"/> when that location is unavailable
	/// (for instance, because of lacking permissions) but rather returns null.
	/// </summary>
	public static async Task<Geocoordinate> GetLocationOrDefault(this IGeolocatorService locationService, CancellationToken ct)
	{
		try
		{
			return await locationService.GetLocation(ct);
		}
		catch (InvalidOperationException)
		{
			return null;
		}
	}
}
