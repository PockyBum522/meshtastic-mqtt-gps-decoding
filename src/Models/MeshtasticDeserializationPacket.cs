using ProtoBuf;

namespace ProtobufTests.Models;

[ProtoContract]
public class MeshtasticDeserializationPacket 
{
    [ProtoMember(1)]
    public MeshtasticPacketFour? SubPacket { get; set; }
}

[ProtoContract]
public class MeshtasticPacketFour 
{
    [ProtoMember(4)]
    public MeshtasticPacketTwo? SubPacket { get; set; }
}

[ProtoContract]
public class MeshtasticPacketTwo 
{
    [ProtoMember(2)]
    public MeshtasticGpsLocationPacket? GpsInfo { get; set; }
}
 
[ProtoContract]
public class MeshtasticGpsLocationPacket 
{
    [ProtoMember(1)]
    public int Latitude { get; set; }
    
    [ProtoMember(2)]
    public int Longitude { get; set; }
    
    [ProtoMember(3)]
    public int AltitudeMetersAboveSeaLevel { get; set; }

    [ProtoMember(4)]
    public int Time { get; set; }

    /// <summary>
    /// The number of bits of precision for the location information from the sending node
    /// </summary>
    [ProtoMember(23)]
    public int PrecisionBits { get; set; }
    
    
    // Data from original .proto file is below:
    
    //
    // /*
    //  * This is usually not sent over the mesh (to save space), but it is sent
    //  * from the phone so that the local device can set its time if it is sent over
    //  * the mesh (because there are devices on the mesh without GPS or RTC).
    //  * seconds since 1970
    //  */
    // fixed32 time = 4;
    //
    // /*
    //  * How the location was acquired: manual, onboard GPS, external (EUD) GPS
    //  */
    // enum LocSource {
    //   /*
    //    * TODO: REPLACE
    //    */
    //   LOC_UNSET = 0;
    //
    //   /*
    //    * TODO: REPLACE
    //    */
    //   LOC_MANUAL = 1;
    //
    //   /*
    //    * TODO: REPLACE
    //    */
    //   LOC_INTERNAL = 2;
    //
    //   /*
    //    * TODO: REPLACE
    //    */
    //   LOC_EXTERNAL = 3;
    // }
    //
    // /*
    //  * TODO: REPLACE
    //  */
    // LocSource location_source = 5;
    //
    // /*
    //  * How the altitude was acquired: manual, GPS int/ext, etc
    //  * Default: same as location_source if present
    //  */
    // enum AltSource {
    //   /*
    //    * TODO: REPLACE
    //    */
    //   ALT_UNSET = 0;
    //
    //   /*
    //    * TODO: REPLACE
    //    */
    //   ALT_MANUAL = 1;
    //
    //   /*
    //    * TODO: REPLACE
    //    */
    //   ALT_INTERNAL = 2;
    //
    //   /*
    //    * TODO: REPLACE
    //    */
    //   ALT_EXTERNAL = 3;
    //
    //   /*
    //    * TODO: REPLACE
    //    */
    //   ALT_BAROMETRIC = 4;
    // }
    //
    // /*
    //  * TODO: REPLACE
    //  */
    // AltSource altitude_source = 6;
    //
    // /*
    //  * Positional timestamp (actual timestamp of GPS solution) in integer epoch seconds
    //  */
    // fixed32 timestamp = 7;
    //
    // /*
    //  * Pos. timestamp milliseconds adjustment (rarely available or required)
    //  */
    // int32 timestamp_millis_adjust = 8;
    //
    // /*
    //  * HAE altitude in meters - can be used instead of MSL altitude
    //  */
    // optional sint32 altitude_hae = 9;
    //
    // /*
    //  * Geoidal separation in meters
    //  */
    // optional sint32 altitude_geoidal_separation = 10;
    //
    // /*
    //  * Horizontal, Vertical and Position Dilution of Precision, in 1/100 units
    //  * - PDOP is sufficient for most cases
    //  * - for higher precision scenarios, HDOP and VDOP can be used instead,
    //  *   in which case PDOP becomes redundant (PDOP=sqrt(HDOP^2 + VDOP^2))
    //  * TODO: REMOVE/INTEGRATE
    //  */
    // uint32 PDOP = 11;
    //
    // /*
    //  * TODO: REPLACE
    //  */
    // uint32 HDOP = 12;
    //
    // /*
    //  * TODO: REPLACE
    //  */
    // uint32 VDOP = 13;
    //
    // /*
    //  * GPS accuracy (a hardware specific constant) in mm
    //  *   multiplied with DOP to calculate positional accuracy
    //  * Default: "'bout three meters-ish" :)
    //  */
    // uint32 gps_accuracy = 14;
    //
    // /*
    //  * Ground speed in m/s and True North TRACK in 1/100 degrees
    //  * Clarification of terms:
    //  * - "track" is the direction of motion (measured in horizontal plane)
    //  * - "heading" is where the fuselage points (measured in horizontal plane)
    //  * - "yaw" indicates a relative rotation about the vertical axis
    //  * TODO: REMOVE/INTEGRATE
    //  */
    // optional uint32 ground_speed = 15;
    //
    // /*
    //  * TODO: REPLACE
    //  */
    // optional uint32 ground_track = 16;
    //
    // /*
    //  * GPS fix quality (from NMEA GxGGA statement or similar)
    //  */
    // uint32 fix_quality = 17;
    //
    // /*
    //  * GPS fix type 2D/3D (from NMEA GxGSA statement)
    //  */
    // uint32 fix_type = 18;
    //
    // /*
    //  * GPS "Satellites in View" number
    //  */
    // uint32 sats_in_view = 19;
    //
    // /*
    //  * Sensor ID - in case multiple positioning sensors are being used
    //  */
    // uint32 sensor_id = 20;
    //
    // /*
    //  * Estimated/expected time (in seconds) until next update:
    //  * - if we update at fixed intervals of X seconds, use X
    //  * - if we update at dynamic intervals (based on relative movement etc),
    //  *   but "AT LEAST every Y seconds", use Y
    //  */
    // uint32 next_update = 21;
    //
    // /*
    //  * A sequence number, incremented with each Position message to help
    //  *   detect lost updates if needed
    //  */
    // uint32 seq_number = 22;
}