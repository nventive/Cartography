﻿namespace Cartography;

public sealed class BasicGeoposition
{
    public BasicGeoposition()
    {
    }

    public BasicGeoposition(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public BasicGeoposition(double latitude, double longitude, double altitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
    }

    /// <summary>
    /// The latitude of the geographic position. The valid range of latitude values is from -90.0 to 90.0 degrees.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// The longitude of the geographic position. This can be any value. For values less
    /// than or equal to-180.0 or values greater than 180.0, the value may be wrapped
    /// and stored appropriately before it is used. For example, a longitude of 183.0
    /// degrees would become -177.0 degrees.
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// The altitude of the geographic position in meters.
    /// </summary>
    public double Altitude { get; set; }
}
