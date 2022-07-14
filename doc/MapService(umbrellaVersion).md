# MapService

## Summary

The `MapService` is a service that lets the application show the directions to a destination or show a specified location.

For directions-related functionality, the service opens the device's maps application pre-populated with the appropriate information.

For location-related functionality, the service opens the device's maps application and centers the view on the specified location.

## Platform support

| Feature                                                  | UWA | Android | iOS |
| -------------------------------------------------------- |:---:|:-------:|:---:|
| Show directions to a GeoCoordinate                       |  X  |    X    |  X  |
| Show and center a specified location's GeoCoordinate     |  X  |    X    |  X  |
| Specify the location display name                        |  X  |    -    |  X  |
| Shows directions by specifying LocationName              |  -  |    X    |  X  |
| Show and center a specified location by LocationName     |  -  |    X    |  X  |

## Usage

### 1. Configure your application.

- For all platforms

    1. Add a reference to the Umbrella Map Service (Umbrella.Location.MapService)


- For **UWA**,
    ```csharp
    container.Register<IMapService>(c => new MapService(
        () => c.Resolve<ILocationServiceEx>(),
        () => c.Resolve<IBrowser>()
    ));
    ```

    2. Add the **location** capability to your `Package.appxmanifest` file.
    ```xml
    <DeviceCapability Name="location" />
    ```
    
- For **Android**,
    ```csharp
    container.Register<IMapService>(c => new MapService(
        async ct => ContextHelper.Current,
    ));
    ```

- For **iOS**,
    ```csharp
    container.Register<IMapService>(c => new MapService(
		dispatcherScheduler
	));
    ```

### 2. Use the service.

```csharp
var mapService = container.Resolve<IMapService>();

// Shows directions to coordinates.
var mapRequest1 = new MapRequest(new GeoCoordinate(45.5016889, -73.56725599999999), "Montreal");

await mapService.ShowDirections(
    ct,
    mapRequest1
);

// Shows location of coordinates.
await mapService.ShowLocation(
    ct,
    mapRequest1
);

// Shows location, searching for a named location instead of using coordinates.
var mapRequest2 = new MapRequest("Ottawa");

await mapService.ShowLocation(
    ct,
    mapRequest2
);
```

## Known issues

On Android, searching by coordinates (even if a label is provided) will not cause a label to show on the map destination indicator. In order to see a label over the destination, one must seach by location name.
