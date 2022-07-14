# LocationServiceEx

## Summary

The `LocationServiceEx` is a service that allow to retrieve last recorded geolocations of the device running the application. This is done using GeoCoordinateWachers.

## Platform support

| Feature                                                            | UWA | Android | iOS |
| ------------------------------------------------------------------ |:---:|:-------:|:---:|
| Set time interval between location check                           |     |    X    |  X  |
| Set highAccuracy mode                                              |     |    X    |  X  |
| Set minimum movement to report a position                          |     |    X    |  X  |
| Get Status of the GeoPosition                                      |     |    X    |  X  |
| Create new GeoCoordinateWatcher                                    |     |    X    |  X  |
| Start GeoCoordinateWatcher                                         |     |    X    |  X  |
| Stop GeoCoordinateWacher                                           |     |    X    |  X  |
| Get last recorde location                                          |     |    X    |  X  |
| Get timestamp of last recorded location                            |     |    X    |  X  |
| Get status of the geolocation                                      |     |    X    |  X  |
| Check if location services are currently enabled on the device.    |     |    X    |  X  |

## Usage

- For all platforms

    1. Add a reference to the Umbrella Location Service (Umbrella.Location.LocationService)

## UWA

- For **UWA**,
	Turn on your location services in location privacy.

- In Module.cs:
	```cs
		Container.Register<ILocationServiceEx>(c =>
			new LocationServiceEx(
				c.ResolveNamed<IScheduler>("UIThread"),
				highAccuracy: false,
				movementThresholdMeters: 5000,
				reportInterval: TimeSpan.FromMinutes(20)
			)
		);

		...

		Container.Register<IMapService>(c =>
			new MapService(
				c.Resolve<ILocationServiceEx>(),
				c.Resolve<IBrowser>()
			)
		);
	```
	```cs
		var scheduler = TaskPoolScheduler.Default;

		var sut = new LocationService(scheduler);
		var position =
			sut
				.FilterInvalidPositions()
				.CurrentLocationAndStatus
				.LogDebug()
				.FirstValueDebugging(scheduler);
		Console.WriteLine(position.Location.Location.ToString());
	```

## Android

Make sure to add the required permissions to the consumer application:

```csharp
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
#if __ANDROID_29__ // and, background location access is required
[assembly: UsesPermission(Android.Manifest.Permission.AccessBackgroundLocation)]
#endif
```

## Known issues
- [iOS] `LocationService` doesn't receive/publish status changes
  > - make sure the `subscriptionScheduler` is the ui thread one; or
  > - make sure the `CLLocationManager` is created on ui thread when using `Func<CLLocationManager> locationManager` parameter