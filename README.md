# Cartography Refactor

Doing a complete refactor of cartography module.  

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
<staticmap:StaticMapControl MapViewPort="{Binding MapViewPort}"
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
   1.1.	`Google Map` : Show Google Map on screen available for UWP, IOS, Android(native)
   1.2.	`IOS Map` : Show "Apple" Map on screen available only on IOS(native)
   1.3.	`Bing Map` : Show Bing Map on screen available only on UWP(native)

2. `Show user location` : Show the user location on the map.

3. `Show Pushpin` : Show Pushpin (marker) on the map.
   3.1.	`Filter Pushpin`
   3.2.	`Add Pushpin`
   3.3.	`Remove Pushpin`
   3.4.	`Customize pushpin` : Change apperance of pushpin.
   3.5.	`Group pushpin` : Group multiple pushpins together, show only one pushpin.

4. `Map interaction`
   4.1.	`Drag` : Move the map.
   4.2.	`Zoom` : Two finger zooming.
   4.3.	`Rotate` : Two finger rotating, can be disable.
   4.4.	`Select pushpin` : Pushpin selected. Can be single or multiple.
   4.5.	`Deselect pushpin` : Pushpin deselection. Can be single or global.
   4.6.	`Zoom on pushpin` : At the start or after an action.
   4.7.	`Add Pushpin` : User can add pushpin.
   4.8.	`Remove Pushpin` : User can remove pushpin.
   4.9.	`Stop animation` : User can stop animation to somewhere.
   4.10.	`Zoom on user` : Center the map to user location.
   4.11.	`Show POI` : IOS only: show Point Of Interest. eg: Tour Eiffel.

5. `Follow User`
   5.1.	`Start follow user`
   5.2.	`Stop follow user` : Can detect if dragging or on button press.
   
### StaticMap
1. `Show Map`: Show a map on screen without interraction possible.
   1.1.	`Google Map` : Show Google Map on screen available for UWP, IOS, Android(native)
   1.2.	`IOS Map` : Show "Apple" Map on screen available only on IOS(native)
   1.3.	`Bing Map` : Show Bing Map on screen available only on UWP(native)
3. `Show Pushpin` : Show one pushpin on map (if place in bound)

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
