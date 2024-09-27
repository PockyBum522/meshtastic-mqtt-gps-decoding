using MQTTnet;
using MQTTnet.Client;
using ProtobufTests.Models;

namespace ProtobufTests;

// You can use https://protobuf-decoder.netlify.app/ to decode a protobuf's structure, then set up classes for
//      deserialization. Those classes are in MeshtasticDeserializationPacket.cs

static class Program
{
    private static bool DebugMessages => true;
    private static MqttServerSelectionEnum ServerSelection => MqttServerSelectionEnum.Pocky;
    
    static async Task Main(string[] args)
    {
        await StartMqttListener();
    } 
 
    private static async Task StartMqttListener()
    {
        var mqttFactory = new MqttFactory();

        using var mqttClient = mqttFactory.CreateMqttClient();

        MqttClientOptionsBuilder? mqttClientOptions;
        var topicString = "";
        
        if (ServerSelection == MqttServerSelectionEnum.Pocky)
        {
            mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId("mesh_protobuf_test_01")
                .WithTcpServer("192.168.1.25", 1883)
                .WithCredentials(SECRETS.MqttUsernamePocky, SECRETS.MqttPasswordPocky);

            topicString = "allenst/meshtastic/#";
        }
        else if (ServerSelection == MqttServerSelectionEnum.MeshtasticOfficial)
        {
            mqttClientOptions = new MqttClientOptionsBuilder()
                .WithClientId("mesh_protobuf_test_01")
                .WithTcpServer("mqtt.meshtastic.org", 1883)
                .WithCredentials(SECRETS.MqttUsernameOfficial, SECRETS.MqttPasswordOfficial);
            
            topicString = "msh/US/#";
        }
        else
        {
            throw new ArgumentException("Invalid server selection");
        }
        
        if (string.IsNullOrWhiteSpace(topicString)) throw new ArgumentException("Invalid topic string");
        
        var builtClient = mqttClientOptions.Build();
        
        mqttClient.DisconnectedAsync += async e =>
        {
            if (e.ClientWasConnected)
            {
                // Use the current options as the new options.
                await mqttClient.ConnectAsync(mqttClient.Options);
            }
        };
        
        // Setup message handling before connecting 
        mqttClient.ApplicationMessageReceivedAsync += HandleIncomingMessage;

        await mqttClient.ConnectAsync(builtClient, CancellationToken.None);

        var mqttSubscribeOptions = 
            mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic(topicString); })
                .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        Console.WriteLine("Subscribed to MQTT topics");
        
        // Pause forever to wait for incoming messages
        while (true){ await Task.Delay(1000); }
    }

    private static Task HandleIncomingMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        var rawPayload = e.ApplicationMessage.PayloadSegment;

        var messageAscii = System.Text.Encoding.ASCII.GetString(rawPayload);
        
        if (DebugMessages)
        {
            Console.WriteLine();
            Console.WriteLine($"[DEBUG - RAW MQTT] New message on: {e.ApplicationMessage.Topic}");
            Console.WriteLine($"[DEBUG - RAW MQTT] New message payload: {messageAscii}");
        }
        
        
        if (rawPayload.Array == null) throw new NullReferenceException();
        
        var gpsInfo = new GpsInfo(rawPayload.Array, DebugMessages);

        // Bounding box for Florida
        if (gpsInfo.Latitude > 31.1m) return Task.CompletedTask;
        if (gpsInfo.Latitude < 24.4m) return Task.CompletedTask;
        if (gpsInfo.Longitude < -87.7m) return Task.CompletedTask;
        if (gpsInfo.Longitude > -80.0m) return Task.CompletedTask;
        
        Console.WriteLine();
        Console.WriteLine($"[{DateTimeOffset.Now.ToString("G")}] New GPS info in FL bounding box: ");
        Console.WriteLine($"{gpsInfo.Latitude}, {gpsInfo.Longitude}");
        Console.WriteLine($"DOP Bits: {gpsInfo.PrecisionBits}");

        return Task.CompletedTask;
    }
}

internal enum MqttServerSelectionEnum
{
    Uninitialized,
    Pocky,
    MeshtasticOfficial
}


// static void SerializingExample()
// {
//     var protoObject = new ProtobufTestClass()
//     {
//         CallSign = "XABX",
//         Latitude = 28.1234f,
//         Longitude = -81.4321f
//     };
//     
//     var serBytes = ProtoSerialize(protoObject);
//     
//     File.WriteAllBytes("/home/david/Desktop/protobuftest.proto", serBytes);
//     
//     foreach (var serByte in serBytes)
//     {
//         Console.Write(serByte);
//         Console.Write(" ");
//     }
// }
//
// private static byte[] ProtoSerialize<T>(T record) where T : class
// {
//     using var stream = new MemoryStream();
//     Serializer.Serialize(stream, record);
//     return stream.ToArray();
// }