namespace ProtobufTests.Models;

public class GpsInfo(decimal latitude, decimal longitude, int altitude, int precisionBits, string nodeId)
{
    public decimal Latitude { get; private set; } = latitude;

    public decimal Longitude { get; private set; } = longitude;
    
    public int AltitudeMetersAboveSeaLevel { get; private set; } = altitude;

    public string NodeId { get; private set; } = nodeId;

    /// <summary>
    /// The number of bits of precision for the location information from the sending node
    /// </summary>
    public int PrecisionBits { get; private set; } = precisionBits;
}