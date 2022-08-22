﻿# Project Title

Cartography Refactor

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
   <dynamicMap:MapControl ViewModel="{Binding }" />
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
1.	#### Show Map
   1.1.	Google Map
      1.1.1.	Android
      1.1.2.	IOS
      1.1.3.	UWP
   1.2.	IOS Map
      1.2.1.	IOS only
   1.3.	Bing Map
      1.3.1.	UWP only

2.	#### Show user location

3.	#### Show Pushpin
   3.1.	Filter Pushpin
   3.2.	Add Pushpin
   3.3.	Remove Pushpin
   3.4.	Customize pushpin
   3.5.	Group pushpin

4.	#### Map interaction
   4.1.	Drag
   4.2.	Zoom
   4.3.	Rotate
   4.4.	Select pushpin
   4.5.	Deselect pushpin
   4.6.	Zoom on pushpin
   4.7.	Add Pushpin
   4.8.	Remove Pushpin
   4.9.	Stop animation
   4.10.	Zoom on user
   4.11.	Show POI

5.	#### Follow User
   5.1.	Start follow user
   5.2.	Stop follow user
   
### StaticMap
1.	#### Show Map
   1.1.	Google Map
      1.1.1.	Android
      1.1.2.	IOS
      1.1.3.	UWP
   1.2.	IOS Map
      1.2.1.	IOS only
   1.3.	Bing Map
      1.3.1.	UWP only
2.	#### Show user location
3.	#### Show Pushpin

### MapService
1.	#### Open user default map service and show location.
2.	#### Open user default map service and show direction.


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
