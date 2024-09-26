# meshtastic-mqtt-gps-decoding
Decodes GPS location information seen on the mesh

This basically just sets up protobuf deserialization and a MQTT listener.

Protobuf data was initially decoded with https://protobuf-decoder.netlify.app/ to get the structure, then referenced with https://github.com/meshtastic/protobufs/blob/master/meshtastic/mesh.proto

I may convert the entire proto file later. As of right now there's just hardcoded classes to deserialize to.

# src\SECRETS\SECRETS.cs format:
```
namespace ProtobufTests;

public static class SECRETS
{
    public static string MqttUsername => "YourMqttUsername";
    public static string MqttPassword => "YourMqttPassword";
}
```

