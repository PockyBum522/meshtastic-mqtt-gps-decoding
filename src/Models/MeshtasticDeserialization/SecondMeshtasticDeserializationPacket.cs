using ProtoBuf;

namespace ProtobufTests.Models.MeshtasticDeserialization;

[ProtoContract]
public class SecondMeshtasticDeserializationPacket
{
    [ProtoMember(1)]
    public SecondMeshtasticPacketFour? SubPacket { get; set; }
}

[ProtoContract]
public class SecondMeshtasticPacketFour
{
    [ProtoMember(1)]
    public uint NodeIdDecimal { get; set; } 
    
    [ProtoMember(4)]
    public SecondMeshtasticPacketTwo? SubPacket { get; set; }
}

[ProtoContract]
public class SecondMeshtasticPacketTwo
{
    [ProtoMember(2)]
    public SecondMeshtasticGpsLocationPacket? GpsInfo { get; set; }
}
 
[ProtoContract]
public class SecondMeshtasticGpsLocationPacket
{
    [ProtoMember(1)] 
    public string NodeLongName { get; set; } = "";
    
    [ProtoMember(9)]
    public int Latitude { get; set; }
    
    [ProtoMember(10)]
    public int Longitude { get; set; }
    
    [ProtoMember(11)]
    public int AltitudeMetersAboveSeaLevel { get; set; }
    
    /// <summary>
    /// The number of bits of precision for the location information from the sending node
    /// </summary>
    [ProtoMember(12)]
    public int PrecisionBits { get; set; }
}