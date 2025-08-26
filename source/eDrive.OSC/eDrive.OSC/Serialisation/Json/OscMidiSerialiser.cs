using Newtonsoft.Json;

namespace eDrive.OSC.Serialisation.Json;

/// <summary>
///     Encdodes and decodes midi messages
/// </summary>
[CustomOscJSonSerializer('m', typeof(OscMidiMessage))]
public class OscMidiSerializer : OscTypeJsonSerializer<OscMidiMessage>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OscMidiSerializer" /> class.
    /// </summary>
    public OscMidiSerializer() : base('m')
    {
    }

    public override OscMidiMessage Decode(JsonReader reader)
    {
        reader.Read();
        var port = (byte)(reader.ReadAsInt32() ?? 0);
        var status = (byte)(reader.ReadAsInt32() ?? 0);
        var d1 = (byte)(reader.ReadAsInt32() ?? 0);
        var d2 = (byte)(reader.ReadAsInt32() ?? 0);
        reader.Read();
        var ret = new OscMidiMessage(port, status, d1, d2);

        return ret;
    }

    public override void Encode(JsonWriter output, OscMidiMessage value)
    {
        output.WriteStartArray();
        output.WriteValue(value.PortId);
        output.WriteValue(value.Status);
        output.WriteValue(value.Data1);
        output.WriteValue(value.Data2);
        output.WriteEndArray();
    }
}