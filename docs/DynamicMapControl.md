# List of Control for dynamicMap

## DynamicMap

- SelectionMode : "**None**", "Single", "Multiple" / *Enable Selection type.*
- AutolocateButtonVisibility : "**Visible**", "Collapsed" / *Show locate me button.*
- CompassButtonVisibility : "**Visible**", "Collapsed" / *Show compass button when map isn't facing north. Position map facing north on tap.*
- MapStyleJson : String from JSON / *Set your mapstyle here. See [google map style wizard](https://mapstyle.withgoogle.com/). Android Only*
- IsRotateGestureEnabled : "**True**", "False" / *Set if user can rotate map.*
- PushpinIcon : Image file / *Set image for the unselected Pushpin.*
- SelectedPushpinIcon : Image file / *Set image for the selected Pushpin.*
- PushpinIconsPositionOrigin : "(double)x, (double)y" / *Set the origin position for Pushpin.*
- EnableAppleZoomAnimations: "**True**", "False" / *Enables or disables the animation when moving or zooming on Apple map.*

## Map Behavior control

- dynamicMap:MapControlBehavior.DisableRotation : "**True**", "False" / *Disable rotation.*
- dynamicMap:MapControlBehavior.IconWidth : float / *Set width of unselected pushpin.*
- dynamicMap:MapControlBehavior.IconHeight : float / *Set height of unselected pushpin.*
- dynamicMap:MapControlBehavior.SelectedIconWidth : float / *Set width of selected pushpin.*
- dynamicMap:MapControlBehavior.SelectedIconHeight : float / *Set height of selected pushpin.*
- dynamicMap:MapControlBehavior.CompassMargin : [Thickness](https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.thickness?view=winrt-22621) / *Set margin of compass.*
- dynamicMap:MapControlBehavior.PushpinImageSelector : IValueConverter / *Select the right image depending of parameter.*

### PushpinSelector example

```xml
<!-- YourMap.xaml -->

<Page.Resources>
    <ResourceDictionary>
        <converters:FromPushpinToStringConverter x:Key="PushpinToMapPin"
                                                 UnselectedValue="ms-appx:///Assets/Pushpin/inactive.png"
                                                 SelectedValue="ms-appx:///Assets/Pushpin/active.png" />
    </ResourceDictionary>
</Page.Resources> 
```

```csharp
// FromPushpinToStringConverter.cs

public class FromPushpinToStringConverter : ConverterBase
{
    public string UnselectedValue { get; set; }

    public string SelectedValue { get; set; }

    protected override object Convert(object value, Type targetType, object parameter)
    {
        var Pushpin = value as PushpinEntity;

#if __ANDROID__
        var isSelected = parameter as bool? ?? false;
#else
        var isSelected = Pushpin.SafeEquals(parameter as PushpinEntity);
#endif
        return isSelected
            ? SelectedValue
            : UnselectedValue;
        }
    }
```

### Multiple pushpins example

```csharp
// FromMultiplePushpinTypeToStringConverter.cs

public class FromMultiplePushpinTypeToStringConverter : ConverterBase
{
    public string UnselectedValue { get; set; }

    public string Type1 { get; set; }

    public string Type2 { get; set; }

    public string Type3 { get; set; }

    public string DefaultType { get; set; }

    protected override object Convert(object value, Type targetType, object parameter)
    {
        var LocationPushpin = value as LocationPushpin;

#if __ANDROID__
        var isSelected = parameter as bool? ?? false;
#else
        var isSelected = LocationPushpin.SafeEquals(parameter as LocationPushpin);
#endif

        if (!isSelected)
        {
            return UnselectedValue;
        }

        switch (LocationPushpin.typeName)
        {
            case "pushpinType1":
                return Type1;

            case "pushpinType2":
                return Type2;

            case "pushpinType3":
                return Type3;

            default:
                return DefaultType;
        }
    }
}
```
