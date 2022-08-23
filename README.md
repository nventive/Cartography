# Cartography

Nventive solution for mobile app map.

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

## Getting Started

### Samples
- Clone project.
- For seeing samples, build and install app with VS on the desire device (Android, IOS, or UWP)

### DynamicMap
- Add Cartography.DynamicMap NuGet package to your project.
- In your ViewModel :
```
using Cartography.DynamicMap
```
- Implement IDynamicMapComponent to your ViewModel
- Set Initial Value to your ViewPort.
- Add in your Page
#### UWP
```
<win:Grid>
   <dynamicMap:MapControl ViewModel="{Binding}" />
</win:Grid>
```
#### Android / IOS
```
<xamarin:Grid>
   <dynamicMap:MapControl ViewModel="{Binding}" />
</xamarin:Grid>
```
- Add Style MapControl : see https://github.com/nventive/Cartography/blob/master/Samples/Samples/Samples.Shared/Views/Styles/MapControl.xaml

### StaticMap
- Add Cartography.StaticMap NuGet package to your project.
- In your ViewModel :
```
using Cartography.StaticMap
```
- Implement IStaticMapComponent to your ViewModel
- Set Initial Value to your ViewPort and MapSize.
- Add in your Page 
```
<staticmap:StaticMapControl MapViewPort="{Binding ViewPort}"
                                        MapSize="{Binding MapSize}"
                                        Width="*YourChoice*"
                                        Height="*YourChoice*" />
```
- Add Style StaticMapControl : see https://github.com/nventive/Cartography/blob/master/Samples/Samples/Samples.Shared/Views/Styles/StaticMapControl.xaml

### MapService
- Add Cartography.MapService NuGet package to your project.
- In your ViewModel :
```
using Cartography.MapService
```
- Add Service :
```
private IMapService _mapService = this.GetService<IMapService>();
```
- For location :
```
   await _mapService.ShowLocation(ct, new MapRequest(
                new BasicGeoposition()
                {
                    Latitude = 45.5016889,
                    Longitude = -73.56725599999999
                },
                "Montreal"
                ));
```
OR
   for Direction (from user location to a GeoPosition) :
```
   await _mapService.ShowDirections(ct, new MapRequest(
                new BasicGeoposition()
                {
                    Latitude = 45.5016889,
                    Longitude = -73.56725599999999,
                },
                "Montreal"
            ));
```

## Features

### DynamicMap
1. `Show Map` : Show a interactive map on screen
   - `Google Map` : Show Google Map on screen available for UWP, IOS, Android(native)
   - `IOS Map` : Show Apple Map on screen available only on IOS(native)
   - `Bing Map` : Show Bing Map on screen available only on UWP(native)

2. `Show user location` : Show the user location on the map.

3. `Show Pushpin` : Show Pushpin (marker) on the map.
   - `Filter Pushpin`
   - `Add Pushpin`
   - `Remove Pushpin`
   - `Customize pushpin` : Change appearance of pushpin.
   - `Group pushpin` : Group multiple pushpins together, show only one pushpin.

4. `Map interaction`
   - `Drag` : Move the map.
   - `Zoom` : Two finger zooming.
   - `Rotate` : Two finger rotating, can be disable.
   - `Select pushpin` : Pushpin selected. Can be single or multiple.
   - `Deselect pushpin` : Pushpin deselection. Can be single or global.
   - `Zoom on pushpin` : At the start or after an action.
   - `Add Pushpin` : User can add pushpin.
   - `Remove Pushpin` : User can remove pushpin.
   - `Stop animation` : User can stop animation to somewhere.
   - `Zoom on user` : Center the map to user location.
   - `Show POI` : IOS only: show Point Of Interest. eg: Tour Eiffel.

5. `Follow User`
   - `Start follow user`
   - `Stop follow user` : Can detect if dragging or on button press.
   
### StaticMap
1. `Show Map`: Show a map on screen without interraction possible.
   - `Google Map` : Show Google Map on screen available for UWP, IOS, Android(native)
   - `IOS Map` : Show Apple Map on screen available only on IOS(native)
   - `Bing Map` : Show Bing Map on screen available only on UWP(native)
2. `Show Pushpin` : Show one pushpin on map (if place in bound)

### MapService
1.	`Location` : Open user default map service and show a location.
2.	`Direction` : Open user default map service and show direction from User location to somewhere.


## Changelog

Please consult the [CHANGELOG](CHANGELOG.md) for more information about version
history.

## License

This project is licensed under the Apache 2.0 license - see the
[LICENSE](LICENSE) file for details.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on the process for
contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).
