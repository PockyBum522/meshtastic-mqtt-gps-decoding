using MQTTnet;
using MQTTnet.Client;
using ProtobufTests.Models;

namespace ProtobufTests;

// You can use https://protobuf-decoder.netlify.app/ to decode a protobuf's structure, then set up classes for
//      deserialization. Those classes are in FirstMeshtasticDeserializationPacket.cs

static class Program
{
    private static bool DebugMessages => false;
    private static MqttServerSelectionEnum ServerSelection => MqttServerSelectionEnum.MeshtasticOfficial;
    
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
                .WithClientId("mesh_protobuf_test_02")
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

        if (rawPayload.Array == null)
        {
            throw new NullReferenceException();
            
            //return Task.CompletedTask;
        }
        
        var gpsInfo = new GpsInfo();

        if (!LocationChecks.CoordinatesAreInFlorida(gpsInfo)) return Task.CompletedTask;

        // _seenNodes.RemoveAll(x => x.NodeId == gpsInfo.NodeId);
        //
        // Console.WriteLine($"Adding: {gpsInfo.NodeId}, {gpsInfo.Latitude}, {gpsInfo.Longitude}, {gpsInfo.PrecisionBits}");
        //
        // _seenNodes.Add(gpsInfo);
        //
        // PrintAllNodes(_seenNodes);

        return Task.CompletedTask;
    }
    
    private static void PrintAllNodes(List<GpsInfo> seenNodes)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("Seen nodes list:");
        
        seenNodes = seenNodes.OrderBy(x => x.PrecisionBits).ToList();
        
        foreach (var seenNode in seenNodes)
        {
            Console.WriteLine($"{seenNode.NodeId}, {seenNode.Latitude}, {seenNode.Longitude}, {seenNode.PrecisionBits}");
        }
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();

    }
}

internal enum MqttServerSelectionEnum
{
    Uninitialized,
    Pocky,
    MeshtasticOfficial
}