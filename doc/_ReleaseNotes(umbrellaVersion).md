# Release notes

## Next Version
### Features

### Breaking changes

### Bug fixes

## 7.0.0
### Features
* Update Uno.UI to 4.1.8

## 6.0.0
### Features
* Add support for Android 12
* UAP 10.0.19041 support
* NET472 support

### Breaking changes
* Removed support for Android 10.0

## 5.0.0
### Features
* Add support for Android 11
* Add support to UAP 18362

### Breaking changes
* Remove support for Android 9

## 4.0.0
### Features
* Updated Xamarin.Google.iOS.Maps to 3.7.0.1 for support of Xamarin build download 0.10.0
* Updated location permission request for Android Q compliance
### Breaking changes
* updated CommonServiceLocator to 2.0.5
* Dropped support for MonoAndroid80 target
* Updated to Android 10 with support of Android X
* Updated to Xamaring build download 0.10.0 to fix sub-dependencies issues
* Updated to Uno.UI 3.0+
* 
### Bugfixes
* Fixed selected pushpins not appearing on top of unselected ones
* Replaced use of deprecated method `ProvideAPIKey` with `ProvideApiKey`

## 3.8.0
### Features
* Added a sample and documentation to center the map on current location and zoom out to see all pushpins
* MapControlBehavior.PushpinImageSelector now works for iOS GoogleMapControl
### Breaking changes
* Fix zoom level scale difference between Apple and Google map.
### Bugfixes
* Fix zoom level scale difference between Apple and Google map.
* Apple map adjusts itself to see all defined points of interest even if a map center is defined.

## 3.7.0

### Features
 * Added "MapStyleJson" property to MapControlBase. Intended for hiding map features on Android (see https://developers.google.com/maps/documentation/android-sdk/hiding-features ).
 * Enabled StaticGoogleMaps in uwp
 * Added ViewPortCoordinates to MapControl builder, to obtain visible boundaries.
   Added overload in pushpins builder to Load and LoadForViewPort which include these coordinates.
 * Created MapComponentBuilderExtensions.Feeds as a place to add extensions supporting Feeds
 * Added logs
 * MapCompoment.StartAt: added an optional parameter to zoom in to the initial coordinates without animation
 * MapComponent.SetAnimationDuration: sets the duration of the zooming animation when using iOS MapKit (it was sometimes too quick)

### Breaking changes
 * Update Xamarin.GooglePlayServices.Base to 71.1610.0 for MonoAndroid90 (and keep 60.1142.1 for MonoAndroid80)
 * Update Xamarin.GooglePlayServices.Maps to 71.1610.0 for Android SDK 9
 * Update Xamarin.GooglePlayServices.Location and Maps to 71.1600.0 for Android SDK 9
 * Update Xamarin.GooglePlayServices.Location and Maps to 60.1142.1 for Android SDK 8
 * [iOS] LocationServiceEx ctor parameter for Dispatcher is now required, because the service works incorrectly without it
 * [Android/iOS] MapControlBehavior.PushpinImageSelector properties must be a source URI.

### Bugfixes
 * [Android] Samples were not working on Android 9
 * [iOS] AttachZoomToUserPositionCommand didn't take into account the fact that the LocationAndStatus.Status was changed. So when the user changed the location permission in the settings it wasn't updated. The app ahd to be killed and relaunched to update the status.
 * [iOS] Additionnal keys in Info.plist were needed to make RequestAlwaysAuthorization works.
 * [iOS] LocationServiceEx.CurrentLocationAndStatus was sometimes never initialized, when the current location had been obtained previously and then the app was killed.
 * [Android] LocationServiceEx updated the user location incorrectly when the location permission was asked in-app, acting as though it was denied even when it was granted.
 * [iOS] MapControlBehavior, the selected pushpin was not showing if the pushpin was recycled. For exemple, if all pushpins were removed and then re-added.
 * [iOS] `LocationServiceEx` were not receiving `AuthorizationChanged` event to propagate the status changes. This is because `CLLocationManager` was not created on ui thread (via `subscriptionScheduler`).

## 3.6.0
### Features
 * Updated support libraries to 28.0.0.1 for Android 9
### Bugfixes
 * Bug 152623: LocationServiceEx fails on pre-Pie devices when targetting MonoAndroid90

## 3.5.0
### Features
 * Updated Umbrella.Location to target MonoAndroid90.
 * Updated Umbrella.Location to target uap 10.0.17763
 * Added support for [SourceLink](https://github.com/dotnet/sourcelink)
 * StaticMap: Now includes a provider that uses the Google mobile SDK on iOS (**GoogleSdkStaticMapProvider**)

### Breaking changes
 * Projects need to be updated to uap 10.0.17763 in the UWP csproj
 
### Bugfixes
 * Bug 148953: StaticMapControl fails on UWP

## 3.4.0
 * No major changes.

## 3.3.0 

### Features
 * Improves customizability of the map control pushpins (see pretty dynamic sample for reference)

### Breaking changes
 * Removed Android SDK 7.0 and 7.1 support
 * Moved to Uno.UI public nuget packages
 * StaticMap : Needs API key and secret to work when using the **GoogleStaticMapProvider**. See StaticMap documentation 
 * DynamicMap : The MapControlBehavior has been updated to support custom pushpins for **Xamarin**. See DynamicMap documentation (look for *MapControlBehavior*)
 * StaticMap: now includes a provider that uses the mobile SDK on Android (**GoogleSdkStaticMapProvider**)
  - The StaticMap.xaml style must include **ContentPresenter x:Name="PART_InnerMapContentControl"** - *this is required whether you change providers or not*.
    If you use the new provider,  this can replace the Image named "PART_InnerMap" since you
    no longer use the **GoogleStaticMapProvider**. Otherwise, both template parts must be present.
    To use this new provider, the static map provider for Android must be updated. 
  See [StaticMap documentation](StaticMap.md) for further information.
 * 
 
### Bugfixes
 * Bug 132697: Map's EnableAutomaticMinDistanceForUpdate does not handle zoom correctly

## 3.2.0
 
### Breaking changes
 * Removed Android SDK 7.0 and 7.1 support
 * Moved to Uno.UI public nuget packages
 * StaticMap : Needs API key and secret to work. See StaticMap documentation 
 
### Bugfixes
 * Bug 132697: Map's EnableAutomaticMinDistanceForUpdate does not handle zoom correctly

## 3.1.0
 
### Bugfixes
 * Bug 128677: [DynamicMapControl] Using .EnableAutomaticMinDistanceForUpdate() causes the map to never update when moving with small increments. Now compares the last effective viewport instead of the last reported viewport when comparing the distance and zoom level
 * Bug 130572: [Android] Image generated by StaticMapControl sometimes fails to load 

## 3.0.0

### Features
* Initial version based on **Umbrella V2**.
* Added CompassButtonVisibility
* Added onSuccess delegate on AttachZoomToUserPositionCommand extension method

### Bugfixes
 * Bug 128370: [DynamicMapControl] Missing property in order to remove compass
 * Bug 128933: [MapComponent] Unable to trigger an action upon completing 'Locate Me' command
