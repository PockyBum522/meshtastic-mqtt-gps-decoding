using System.Buffers;
using System.Globalization;
using ProtoBuf;
using ProtobufTests.Models;
using ProtobufTests.Models.MeshtasticDeserialization;
using Serilog;

namespace ProtobufTests;

public class MeshtasticPacketDecoder
{
    private readonly byte[] _rawData;
    private readonly string _messageAscii = "";

    private readonly ILogger _logger;
    public MeshtasticPacketDecoder(ILogger logger, byte[] rawData)
    {
        _logger = logger;
        _rawData = rawData;

        _messageAscii = System.Text.Encoding.ASCII.GetString(_rawData);
    }

    public GpsInfo GetLocationFromRawMessage()
    {
        _logger.Debug("");
        _logger.Debug("");
        _logger.Debug("New packet!");
        
        if (IsDecodableAsJsonPacketType())
            return DecodeJsonPacket();

        if (IsDecodableAsFirstPacketType())
            return DecodeFirstPacket();

        // if (IsDecodableAsSecondPacketType())
        //     return DecodeSecondPacket();
        //
        // if (IsDecodableAsThirdPacketType())
        //     return DecodeThirdPacket();
        
        _logger.Error("Could not decode packet as any known type");
        throw new ArgumentException("Could not decode packet as any known type");
    }

    private bool IsDecodableAsJsonPacketType()
    {
        var canDecode = false;
        
        try
        {
            if (_messageAscii.Equals("online", StringComparison.InvariantCultureIgnoreCase)) return false;
            if (_messageAscii.Equals("offline", StringComparison.InvariantCultureIgnoreCase)) return false;

            if (_messageAscii.Contains("\"latitude", StringComparison.InvariantCultureIgnoreCase) ||
                _messageAscii.Contains("\"longitude", StringComparison.InvariantCultureIgnoreCase) ||
                _messageAscii.Contains("\"position\"", StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Debug("JSON FOUND: {MessageAscii}", _messageAscii);

                canDecode = true;
            }
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Exception when attempting to check if packet can decode as JSON | Original: {RawData} / {MessageAscii}", _rawData, _messageAscii");

            return false;
        }

        if (!canDecode) return false;

        GpsInfo? decodedLocation = null;
        
        try
        {
            decodedLocation = DecodeJsonPacket();
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Exception when attempting to check if packet can decode as JSON | Original: {RawData} / {MessageAscii}", _rawData, _messageAscii);

            return false;
        }

        if (!LocationChecks.CoordinatesAreValid(decodedLocation))
        {
            return false;
        }
        
        return true;
    }
    
    private bool IsDecodableAsFirstPacketType()
    {
        GpsInfo? decodedLocation = null;
        
        try
        {
            decodedLocation = DecodeFirstPacket();
        }
        catch (Exception ex)
        {
            _logger.Warning(ex, "Exception when attempting to check if packet can decode as FirstPacketType | Original: {RawData} / {MessageAscii}", _rawData, _messageAscii);
            
            return false;
        }

        if (LocationChecks.CoordinatesAreValid(decodedLocation)) return true;
        
        return false;
    }

    private GpsInfo DecodeFirstPacket()
    {
        var decoded =
            Serializer.Deserialize<FirstMeshtasticDeserializationPacket>(new ReadOnlySequence<byte>(_rawData));

        if (decoded is null || 
            decoded.SubPacket is null ||
            decoded.SubPacket.SubPacket is null ||
            decoded.SubPacket.SubPacket.GpsInfo is null)
        {
            throw new NullReferenceException($"Something was null when attempting to decode as FirstPacketType | Original: {_rawData} / {_messageAscii}");
        }
            
        var rawGps = decoded.SubPacket.SubPacket.GpsInfo;
            
        var decodedLatitude = rawGps.Latitude / 10000000m;
        var decodedLongitude = rawGps.Longitude / 10000000m;

        var decodedAltitude = rawGps.AltitudeMetersAboveSeaLevel;
            
        var decodedPrecisionBits = rawGps.PrecisionBits;
            
        // Convert to hex
        var decodedNodeId = $"!{decoded.SubPacket.NodeIdDecimal:x8}";
            
        var decodedLocation = new GpsInfo(decodedLatitude, decodedLongitude, decodedAltitude, decodedPrecisionBits, decodedNodeId);

        return decodedLocation;
    }

    private bool IsDecodableAsSecondPacketType()
    {
        // Attempt decode as packet type
        
        // If exception, return false
        
        // If invalid GPS data, return false
        
        // return true
    }
    
    private bool IsDecodableAsThirdPacketType()
    {
        // Attempt decode as packet type
        
        // If exception, return false
        
        // If invalid GPS data, return false
        
        // return true
    }

    private GpsInfo DecodeJsonPacket()
    {
        var rawLatitude = GetJsonValue("latitude_i", _messageAscii);
        var decimalLatitude = decimal.Parse(rawLatitude, CultureInfo.InvariantCulture);
        var decodedLatitude = decimalLatitude / 10000000m;
        
        var rawLongitude = GetJsonValue("longitude_i", _messageAscii);
        var decimalLongitude = decimal.Parse(rawLongitude, CultureInfo.InvariantCulture);
        var decodedLongitude = decimalLongitude / 10000000m;

        if (rawLatitude.Length < 3 || rawLongitude.Length < 3)
            throw new ArgumentException($"When attempting to decode JSON, one of LAT:{rawLatitude}, LONG:{rawLongitude} were suspiciously low");
            
        var rawAltitude = GetJsonValue("altitude", _messageAscii);
        
        var altitude = 0;
        
        if (!string.IsNullOrWhiteSpace(rawAltitude))
            altitude = int.Parse(rawAltitude, CultureInfo.InvariantCulture);
        
        var rawPrecision = GetJsonValue("precision_bits", _messageAscii);
        
        var precisionBits = 0;
        
        if (!string.IsNullOrWhiteSpace(rawPrecision))
            precisionBits = int.Parse(rawPrecision, CultureInfo.InvariantCulture);
        
        var nodeIdRaw = GetJsonValue("id", _messageAscii);
        var nodeIdInt = long.Parse(nodeIdRaw, CultureInfo.InvariantCulture);

        var nodeId = "ERROR";
        
        nodeId = $"!{nodeIdInt:x8}";
        
        var returnGpsInfo = new GpsInfo(decodedLatitude, decodedLongitude, altitude, precisionBits, nodeId);

        return returnGpsInfo;
    }
    
    private string GetJsonValue(string key, string rawJson)
    {
        var fullQueryString = $"{key}\":";

        if (!rawJson.Contains(fullQueryString, StringComparison.InvariantCultureIgnoreCase)) return "";
        
        var colonPosition = rawJson.IndexOf(fullQueryString, StringComparison.InvariantCultureIgnoreCase);
        
        colonPosition += fullQueryString.Length - 1;
        
        string value = "";

        for (var i = colonPosition + 1; i < rawJson.Length; i++)
        {
            var nextChar = rawJson[i];
            
            if (nextChar == '\"') continue;
            if (string.IsNullOrWhiteSpace(nextChar.ToString())) continue;
            
            if (nextChar == ',') return value;
            if (nextChar == '}') return value;
            
            value += nextChar;
        }
        
        return value;
    }
}