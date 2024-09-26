using MQTTnet;
using MQTTnet.Client;
using ProtobufTests.Models;

namespace ProtobufTests;

// You can use https://protobuf-decoder.netlify.app/ to decode a protobuf's structure, then set up classes for
//      deserialization. Those classes are in MeshtasticDeserializationPacket.cs

static class Program
{
    static async Task Main(string[] args)
    {
        await StartMqttListener();
    } 
 
    private static async Task StartMqttListener()
    {
        var mqttFactory = new MqttFactory();

        using var mqttClient = mqttFactory.CreateMqttClient();
        
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithClientId("mesh_protobuf_test")
            .WithTcpServer("192.168.1.25", 1883)
            .WithCredentials(SECRETS.MqttUsername, SECRETS.MqttPassword)
            .Build();

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

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = 
            mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(f => { f.WithTopic("allenst/meshtastic/#"); })
                .Build();

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        Console.WriteLine("Subscribed to MQTT topics");
        
        // Pause forever to wait for incoming messages
        while (true){ await Task.Delay(1000); }
    }

    private static Task HandleIncomingMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        var rawPayload = e.ApplicationMessage.PayloadSegment;

        //var asciiPayload = System.Text.Encoding.ASCII.GetString(rawPayload);

        // Console.WriteLine($"New message on: {e.ApplicationMessage.Topic}");
        // Console.WriteLine($"New message payload: {asciiPayload}");
        
        if (rawPayload.Array == null) throw new NullReferenceException();
        
        var gpsInfo = new GpsInfo(rawPayload.Array);

        Console.WriteLine();
        Console.WriteLine($"[{DateTimeOffset.Now.ToString("G")}] New GPS info: ");
        Console.WriteLine($"Lat: {gpsInfo.Latitude}");
        Console.WriteLine($"Long: {gpsInfo.Longitude}");
        
        return Task.CompletedTask;
    }
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