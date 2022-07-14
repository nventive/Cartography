# Static Map

## Summary

The `StaticMapControl` is a control which displays a static image of a map retrieved from a StaticMapProvider in your app.

## Platform support

| Feature                               | UWA | Android | iOS |
| --------------------------------------|:---:|:-------:|:---:|
| Display a static map                  |  X  |    X    |  X  |
| Specify Latitude and Longitude        |  X  |    X    |  X  |
| Specify the Zoom Level                |  X  |    X    |  X  |
| Customize pushpin with template       |  X  |    X    |  X  |
| Specify Width and Height of the Map   |  X  |    X    |  X  |

## Prerequisites

In order to use the Google Maps Static API, you **must** enable the billing on your Google account.

You can follow the offical Google doc to achieve it : https://cloud.google.com/billing/docs/how-to/manage-billing-account.

It is possible to set some alert notifications in order to control your expenses (https://cloud.google.com/billing/docs/how-to/budgets).


## Usage

### 1. Configure your Android project using Google Maps (Google Cloud Platform Console)

On the Google Cloud Platform Console (<https://console.cloud.google.com>) :

    1. Go to APIs & Services > Dashboard > Enable APIs and Services.
    2. Search for Maps Static API and enable it.
    3. Under APIs & Services > Credentials, go to the details. Otherwise, create a new one. Save your API key.
    4. Under APIs & Services > Dashboard, Select Maps Static API and go under URL signing secret section. Save your Current secret.

### 2. Configure your application

On the Google Cloud Platform Console (<https://console.cloud.google.com>) :

    1. Go to APIs & Services > Dashboard > Enable APIs and Services.
    2. Search for Maps Static API and enable it.
    3. Under APIs & Services > Credentials, go to the details. Otherwise, create a new one. Save your API key.
    4. Under APIs & Services > Dashboard, Select Maps Static API and go under URL signing secret section. Save your Current secret.

### 2. Configure your application

- For all platforms

    1. Add a reference to the Umbrella Static Map Component (Umbrella.Location.StaticMap)
    2. Make sure that the content template for the StaticMap contains **ContentPresenter x:Name="PART_InnerMapContentControl"** (this is now part of the default style).

- For **UWA w/ Bing Maps** ,
    
	
	3. Set BingStaticMapProvider as StaticMapProvider
    ```csharp
    StaticMapControl.StaticMapProvider = new BingStaticMapProvider("API_KEY");
	
- For **UWA w/ Google Maps**,
    
	
	3. Set GoogleStaticMapProvider as StaticMapProvider
    ```csharp
    StaticMapControl.StaticMapProvider = new GoogleStaticMapProvider(Constants.GoogleMaps.ApiKey, Constants.GoogleMaps.Secret);
    ```

- For **Android**,


	3. Set GoogleSdkStaticMapProvider as StaticMapProvider (it uses the Google Maps SDK for Android)
	```csharp
       StaticMapControl.StaticMapProvider = new GoogleSdkStaticMapProvider();
	```

    4. Add the API key to AssemblyInfo.cs ; this should vary depending on the platform and environment, therefore should be
	retrieved from ClientConstants:
	 ```csharp
	[assembly: MetaData("com.google.android.maps.v2.API_KEY", Value = ClientConstants.Maps.GoogleMapApiKey)]
	 ```

    There is also a GoogleStaticMapProvider, which is now deprecated:
	
    3. Set GoogleStaticMapProvider as StaticMapProvider (it uses the Static Maps API, which is billable)
	```csharp
       StaticMapControl.StaticMapProvider = new GoogleStaticMapProvider("API_KEY","SECRET");
	```

    * Note: if you still use this provider, the content template for the StaticMap must also include an Image named **PART_InnerMap** (which was part of the previous default style).

- For **iOS w/ Apple Maps**,

	3. Set AppleStaticMapProvider as StaticMapProvider
	```csharp
       StaticMapControl.StaticMapProvider = new AppleStaticMapProvider();
	```

- For **iOS w/ Google Maps**,

    3. Set GoogleSdkStaticMapProvider as StaticMapProvider (it uses the Google Maps SDK for iOS)
	```csharp
       StaticMapControl.StaticMapProvider = new GoogleSdkStaticMapProvider();
	```
    4. Provide Api Key to Google Maps Services
    ```csharp
       Google.Maps.MapServices.ProvideApiKey("API_KEY");
    ```

### 3. Use the control

In your ViewModel, set the needed properties. Note that MapSize is a hint about the size of the map
that should be obtained, in order to avoid pixelation. It does not guarantee the size of the map that is
actually displayed.
```csharp
    Build(b => b
	    .Properties(pb => pb
		    .Attach(MapViewPort, GetMapViewPort)
		    .Attach(MapSize, () => new Size(300, 300))
	    )
    );

    private MapViewPort GetMapViewPort()
    {
	    return new MapViewPort(new Coordinate()
	    {
		    Latitude = 45.5016889,
		    Longitude = -73.56725599999999
	    })
	    {
		    ZoomLevel = ZoomLevels.City
	    };
    }
```

In your XAML, add the StaticMapControl and bind it to the ViewModel.
```xml
	<staticmap:StaticMapControl MapViewPort="{Binding [MapViewPort]}"
								Pushpin="Pushpin"
								PushpinTemplate="{StaticResource PushpinTemplate}"
								MapSize="{Binding [MapSize]}"
								Width="300"
								Height="300" />
```

It is possible to customize the pushpin you display on the map. You can use StaticMapControl's PushpinTemplate property to set the pushpin
```xml
    <staticmap:StaticMapControl MapViewPort="{Binding [MapViewPort]}"
							    Pushpin="Pushpin"
							    PushpinTemplate="{StaticResource PushpinTemplate}" />

    <DataTemplate x:Key="PushpinTemplate">
	    <Ellipse Width="10"
			      Height="10"
			      Fill="Red"/>
    </DataTemplate>
```

# Get API key for the StaticMap

In order to use the static map, you will need an API key and a secret key for Windows and Android. Here are the steps to retrieve it.

_Note: The client should be the one responsible for getting the API keys._

## Windows

_For the detailed procedure for Windows, please follow this link: https://msdn.microsoft.com/en-us/library/windows/apps/xaml/mt219694.aspx _

+ Go to https://www.bingmapsportal.com
+ Login to your account or register if you don't have one
+ Go to MyAccount -> Keys
+ Enter the following information:
	- Application name
	- Application URL (optional)
	- Key type
	- Application type
+ Enter the characters you see in the box
+ Hit *Create* and get the key

The key should be added as the value for the parameter _MapServiceToken_ for the MapControl object.

## Android

_For the detailed procedure on Android, please follow this link: https://developers.google.com/maps/documentation/android-api/signup#release-cert _

+ Retrieve the application's SHA-1 fingerprint
+ Create a project in the Google Developers Console
+ Go to Credentials -> Add credentials -> API key -> Android key
+ In the dialog box, enter the SHA-1 fingerprint and the app package name
+ Hit *Create* and get the key

The API key should be the value for the property _com.google.android.maps.v2.API_KEY_ in the AndroidManifest.xml file.

Since the new billing system introduced on July 16,2018, you will need to sign URL to do API calls

+ Go to the Google Cloud Console: https://console.cloud.google.com/
+ Under APIs & Services, go to the Maps Static API once enabled
+ Navigate to the URL signing secret tab

The current secret must be added on the GoogleStaticMapProvider constructor

## Known issues

None.
