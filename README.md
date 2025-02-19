# Cartography

Nventive solution for mobile app map.

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

## Getting Started

### **Samples**

- Clone project a sample is available.
- For seeing samples, build and install app with VS on the desire device (Android & IOS)

## Mobile

### Android

On Android we need to add these lines to the manifest : 
```
   <uses-library android:name="com.google.android.maps" />
   <meta-data android:name="com.google.android.maps.v2.API_KEY" android:value="" />
   <meta-data android:name="com.google.android.gms.version" android:value="@integer/google_play_services_version" />
```

You'll also need to add the location permissions : 
```
	<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
	<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

### iOS

For iOS you need to add these lines to your info.plist.
```
   <key>NSLocationWhenInUseUsageDescription</key>
   <string>Sample would like to access your location</string>
   <key>NSLocationUsageDescription</key>
   <string>Sample would like to access your location</string>
```

## **DynamicMap**

- Add Cartography.DynamicMap NuGet package to your project.
- In your ViewModel:

   ```csharp
   using Cartography.DynamicMap
   ```

- Implement IDynamicMapComponent to your ViewModel
- Set Initial Value to your ViewPort.
- Add in your Page

### **Android / IOS**

```xml
<mobile:Grid>
   <dynamicMap:MapControl ViewModel="{Binding}" />
</mobile:Grid>
```

## Pushpins 

In order for the pushpins to display properly on all platforms you need to add a MapControlBehavior.PushpinImageSelector 

```xml
<mobile:Grid>
   <dynamicMap:MapControl ViewModel="{Binding}"
   dynamicMap:MapControlBehavior.PushpinImageSelector="{StaticResource PushpinEntityToMapPin}"/>
</mobile:Grid>
```

Then you need to have a converter in your page for the Selector 
```xml
<converters:FromPushpinEntityToStringConverter x:Key="PushpinEntityToMapPin"
                                               UnselectedValue="ms-appx:///Assets/Pushpin/inactive.png"
                                               SelectedValue="ms-appx:///Assets/Pushpin/active.png" />
```

Finally you need to place your pushpin icons assets in the Shared project assets folder AND the android assets folder in the mobile head. 
If you don't do the latter the pushpin won't work on android. 
It is imperative that you place the assets in the android assets folder and not in a subfolder otherwise the assets wont be found by the MapControlBehavior.
This is because uno 4.5 changed how the assets where generated forcing us to apply a workaround for Android. 

Your Android folder should look like this : 

Sample.Mobile
    - Android
        - Assets
            - active.png
            - active.scale-200.png
            - active.scale-300.png
            - inactive.png
            - inactive.scale-200.png
            - inactive.scale-300.png

- Add Style MapControl: see <https://github.com/nventive/Cartography/blob/master/Samples/Samples/Samples.Shared/Views/Styles/MapControl.xaml>

#### **DynamicMap Control**

- Control can be added to your map: See [DynamicMapControl](Documentation/DynamicMapControl.md)

## **StaticMap**

- Add Cartography.StaticMap NuGet package to your project.
- In your ViewModel:

   ```csharp
   using Cartography.StaticMap
   ```

- Implement IStaticMapComponent to your ViewModel
- Set Initial Value to your ViewPort and MapSize.
- Add in your Page

   ```xml
   <staticmap:StaticMapControl MapViewPort="{Binding ViewPort}"
                               MapSize="{Binding MapSize}"
                               Width="10"
                               Height="10" />
   ```

- Add Style StaticMapControl: see <https://github.com/nventive/Cartography/blob/master/Samples/Samples/Samples.Shared/Views/Styles/StaticMapControl.xaml>

## **MapService**

- Add Cartography.MapService NuGet package to your project.
- In your ViewModel:

   ```csharp
   using Cartography.MapService
   ```

- Add Service:

   ```csharp
   private IMapService _mapService = this.GetService<IMapService>();
   ```

- For location:

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

- for Direction (from user location to a GeoPosition):

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

1. `Location` : Open user default map service and show a location.
2. `Direction` : Open user default map service and show direction from User location to somewhere.

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
