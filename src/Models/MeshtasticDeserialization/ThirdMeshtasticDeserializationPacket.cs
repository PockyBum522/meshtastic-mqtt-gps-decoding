using ProtoBuf;

namespace ProtobufTests.Models.MeshtasticDeserialization;

[ProtoContract]
public class ThirdMeshtasticDeserializationPacket
{
    [ProtoMember(1)]
    public ThirdMeshtasticPacketFour? SubPacket { get; set; }
}

[ProtoContract]
public class ThirdMeshtasticPacketFour
{
    [ProtoMember(1)]
    public uint NodeIdDecimal { get; set; } 
    
    [ProtoMember(4)]
    public ThirdMeshtasticPacketTwo? SubPacket { get; set; }
}

[ProtoContract]
public class ThirdMeshtasticPacketTwo
{
    [ProtoMember(2)]
    public ThirdMeshtasticPacketTwoAgain? SubPacket { get; set; }
}

[ProtoContract]
public class ThirdMeshtasticPacketTwoAgain
{
    [ProtoMember(2)]
    public ThirdMeshtasticGpsLocationPacket? GpsInfo { get; set; }
}
 
[ProtoContract]
public class ThirdMeshtasticGpsLocationPacket
{
    [ProtoMember(1)]
    public int Latitude { get; set; }
    
    [ProtoMember(2)]
    public int Longitude { get; set; }
}