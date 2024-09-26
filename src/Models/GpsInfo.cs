using System.Buffers;
using System.Globalization;
using ProtoBuf;

namespace ProtobufTests.Models;

public class GpsInfo
{
    public GpsInfo(byte[] rawProtobufData) 
    {
        try
        {
            var decoded =
                Serializer.Deserialize<MeshtasticDeserializationPacket>(new ReadOnlySequence<byte>(rawProtobufData));

            var gpsData = decoded.SubPacket.SubPacket.GpsInfo;

            var rawLatitude = gpsData.Latitude / 10000000m;
            var rawLongitude = gpsData.Longitude / 10000000m;

            Latitude = rawLatitude;
            Longitude = rawLongitude;

            AltitudeMetersAboveSeaLevel = gpsData.AltitudeMetersAboveSeaLevel;

            Time = gpsData.Time;

            PrecisionBits = gpsData.PrecisionBits;
        }
        catch (ProtoException)
        {
            Console.WriteLine("Protobuf Error when trying to decode raw bytes");
        }

    }
    
    public decimal Latitude { get; private set; }
    
    public decimal Longitude { get; private set; }
    
    public int AltitudeMetersAboveSeaLevel { get; private set; }
    
    public int Time { get; private set; }

    /// <summary>
    /// The number of bits of precision for the location information from the sending node
    /// </summary>
    public int PrecisionBits { get; private set; }
}