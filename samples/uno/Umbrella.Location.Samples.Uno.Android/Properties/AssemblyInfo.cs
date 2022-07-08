using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.App;
using Umbrella.Location.Samples.Uno.Shared;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Umbrella.Location.Samples.Uno.Android")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Umbrella.Location.Samples.Uno.Android")]
[assembly: AssemblyCopyright("Copyright ©  2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// We need to be able to download map tiles, access Google Play Services, Access API Endpoints
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]

// Google Maps for Android v2 needs this permission so that it may check the connection state as it must download data
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]

// These are optional, but recommended. They will allow Maps to use the My Location provider.
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
#if __ANDROID_29__
[assembly: UsesPermission(Android.Manifest.Permission.AccessBackgroundLocation)]
#endif

// Google Maps for Android v2 will cache map tiles on external storage
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]

[assembly: UsesLibrary("com.google.android.maps")]
[assembly: MetaData("com.google.android.maps.v2.API_KEY", Value = Constants.GoogleMaps.ApiKey)]
