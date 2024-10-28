using ProtobufTests.Models;

namespace ProtobufTests;

public static class LocationChecks
{
    public static bool CoordinatesAreInFlorida(GpsInfo gpsInfo)
    {
        // Bounding box for Florida
        return gpsInfo.Latitude > 31.1m &&
               gpsInfo.Latitude < 24.4m &&
               gpsInfo.Longitude < -87.7m &&
               gpsInfo.Longitude > -80.0m;
    }

    public static bool CoordinatesAreValid(GpsInfo? decodedLocation)
    {
        if (decodedLocation is null) return false;
        
        if (decodedLocation.Latitude < -90.0m) return false;
        if (decodedLocation.Latitude > 90.0m) return false;
        
        if (decodedLocation.Longitude < -180.0m) return false;
        if (decodedLocation.Longitude > 180.0m) return false;

        if (decodedLocation.Latitude < 0.00001m &&
            decodedLocation.Latitude > -0.00001m)
        {
            return false;
        }
        
        if (decodedLocation.Longitude < 0.00001m &&
            decodedLocation.Longitude > -0.00001m)
        {
            return false;
        }

        return true;
    }
}