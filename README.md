# Cartography
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg?style=flat-square)](LICENSE)![Version](https://img.shields.io/nuget/v/Cartography.DynamicMap?style=flat-square)![Downloads](https://img.shields.io/nuget/dt/Cartography.DynamicMap?style=flat-square)   

Cartography provides you 3 map related libraries for iOS, Android and Windows using native map of each platform.

## Getting Started

### **Samples**
- Clone project a sample is available.
- For seeing samples, build and install app with VS on the desire device (Android, IOS, or UWP).

### API Key (not needed for mapService)
- For Google Maps (Android), you need to [create your Api Key](https://developers.google.com/maps/documentation/android-sdk/get-api-key).
- For Bing Maps, you need a [Bing Api Key](https://learn.microsoft.com/en-us/bingmaps/getting-started/bing-maps-dev-center-help/getting-a-bing-maps-key) to remove warning.

Set it in : sample.shared => Constants.

```csharp
public class GoogleMaps
{
	//TODO: Get new API Key
	public const string ApiKey = "";
}
```

### **DynamicMap**

### Permission
- Maps doesn't required to ask permission to the user.
- User Location isn't include in Cartography see: [GeolocatorService](https://github.com/nventive/GeolocatorService).

#### Android
- Setup your permission, in your AssemblyInfo of Android.

```csharp
//Required, replace AppName by your Application Name.
[assembly: UsesPermission("AppName.permission.MAPS_RECEIVE")]
[assembly: Permission(Name = "AppName.permission.MAPS_RECEIVE", ProtectionLevel = Android.Content.PM.Protection.Signature)]

[assembly: UsesPermission("com.google.android.providers.gsf.permission.READ_GSERVICES")]

[assembly: MetaData("com.google.android.maps.v2.API_KEY", Value = Constants.GoogleMaps.ApiKey)]

```
#### UWP
- For windows location is in Package.appxmanifest under capabilities, select Location and Internet(Client). 

### Instantiation

- Add Cartography.DynamicMap NuGet package to your project.
- In your ViewModel :
```csharp
using Cartography.DynamicMap
```
- Implement IDynamicMapComponent to your ViewModel. [ViewModel sample](https://github.com/nventive/Cartography/blob/master/Samples/Samples/Samples.Shared/Presentation/DynamicMap_FeaturesPageViewModel.cs)
- Set Initial Value to your ViewPort.
- Add in your Page.

[ViewModel sample](https://github.com/nventive/Cartography/blob/master/Samples/Samples/Samples.Shared/Presentation/DynamicMap_FeaturesPageViewModel.cs)

#### **UWP**
```xml
<win:Grid>
   <dynamicMap:MapControl ViewModel="{Binding}" />
</win:Grid>
```

- Add Style MapControl : see [MapControl](https://github.com/nventive/Cartography/blob/master/Samples/Samples/Samples.Shared/Views/Styles/MapControl.xaml).

#### **Android / IOS**
```xml
<xamarin:Grid>
   <dynamicMap:MapControl ViewModel="{Binding}" />
</xamarin:Grid>
```
- Control can be added to your map : See [DynamicMap Control Documentation](Documentation/DynamicMapControl.md).

&nbsp;
### **StaticMap**
- Add Cartography.StaticMap NuGet package to your project.
- In your ViewModel :
```csharp
using Cartography.StaticMap
```
- Implement IStaticMapComponent to your ViewModel.
- Set Initial Value to your ViewPort and MapSize.
- Add it in your Page.
```xml
<staticmap:StaticMapControl MapViewPort="{Binding ViewPort}"
                                        MapSize="{Binding MapSize}"
                                        Width="10"
                                        Height="10" />
```
- Add Style StaticMapControl : see [StaticMapControl](https://github.com/nventive/Cartography/blob/master/Samples/Samples/Samples.Shared/Views/Styles/StaticMapControl.xaml).

&nbsp;
### **MapService**
- Add Cartography.MapService NuGet package to your project.
- In your ViewModel :
```csharp
using Cartography.MapService
```
- Add Service :
```csharp
private IMapService _mapService = this.GetService<IMapService>();
```
- For location :
```csharp
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
```csharp
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
1. `Show Map` : Show a interactive map on screen.
   - `Google Map` : Show Google Map on screen available for UWP, iOS, Android(native).
   - `IOS Map` : Show Apple Map on screen available only on iOS(native).
   - `Bing Map` : Show Bing Map on screen available only on UWP(native).

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
   - `Show POI` : iOS only: show Point Of Interest. ex: Tour Eiffel.

5. `Follow User`
   - `Start follow user`
   - `Stop follow user` : Can detect if dragging or on button press.
   
### StaticMap
1. `Show Map`: Show a map on screen without interraction possible.
   - `Google Map` : Show Google Maps on screen available for UWP, iOS, Android(native).
   - `iOS Map` : Show Apple Maps on screen available only on iOS(native).
   - `Bing Map` : Show Bing Maps on screen available only on UWP(native).
2. `Show Pushpin` : Show one pushpin on map (if place in bound).

### MapService
1.	`Location` : Open user default map service and show a location.
2.	`Direction` : Open user default map service and show direction from User location to somewhere.


## Breaking Changes

Please consult the [BREAKING CHANGES](BREAKING_CHANGES.md) for the list of breaking changes.

Please consult the [CHANGELOG](CHANGELOG.md) for more information about version
history.

## License

This project is licensed under the [Apache 2.0 license](LICENSE).

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on the process for
contributing to this project.

Be mindful of our [Code of Conduct](CODE_OF_CONDUCT.md).
