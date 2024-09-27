using System.Buffers;
using System.Globalization;
using ProtoBuf;

namespace ProtobufTests.Models;

public class GpsInfo
{
    public GpsInfo(byte[] rawProtobufData, bool debugMessages = false) 
    {
        var messageAscii = System.Text.Encoding.ASCII.GetString(rawProtobufData);

        if (debugMessages)
        {
            Console.WriteLine();
            Console.WriteLine($"[DEBUG - GPS DATA CONSTRUCTOR] Message ASCII is: {messageAscii}");
        }

        try
        {
            var decoded =
                Serializer.Deserialize<MeshtasticDeserializationPacket>(new ReadOnlySequence<byte>(rawProtobufData));

            if (decoded is null) return;
            if (decoded.SubPacket is null) return;
            if (decoded.SubPacket.SubPacket is null) return;
            if (decoded.SubPacket.SubPacket.GpsInfo is null) return;

            var gpsData = decoded.SubPacket.SubPacket.GpsInfo;

            Latitude = gpsData.Latitude / 10000000m;
            Longitude = gpsData.Longitude / 10000000m;

            AltitudeMetersAboveSeaLevel = gpsData.AltitudeMetersAboveSeaLevel;

            Time = gpsData.Time;

            PrecisionBits = gpsData.PrecisionBits;

            if (!debugMessages) return;
            
            Console.WriteLine();
            Console.WriteLine($"[DEBUG - GPS DATA CONSTRUCTOR FINAL] Location: {Latitude}, {Longitude}");
        }
        catch (ProtoException)
        {
            if (!debugMessages) return;
            Console.WriteLine("ProtoException when trying to decode raw bytes");
            DebugProtobufBytes(rawProtobufData);
        }
        catch (EndOfStreamException)
        {
            if (!debugMessages) return;
            Console.WriteLine("EndOfStreamException when trying to decode raw bytes");
            DebugProtobufBytes(rawProtobufData);
        }
    }

    private void DebugProtobufBytes(byte[] rawProtobufData)
    {
        foreach (var b in rawProtobufData)
        {
            Console.Write(b.ToString("X2"));
            Console.Write(" ");
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