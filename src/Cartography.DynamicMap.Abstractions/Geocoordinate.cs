using System;

namespace Cartography.DynamicMap;

public sealed class Geocoordinate
{
    public Geocoordinate(
        double latitude,
        double longitude,
        DateTimeOffset timestamp,
        Geopoint point
    )
    {
        Latitude = latitude;
        Longitude = longitude;
        Timestamp = timestamp;
        Point = point;
    }

    public double Latitude { get; }

    public double Longitude { get; }

    public DateTimeOffset Timestamp { get; }

    public Geopoint Point { get; }

}
