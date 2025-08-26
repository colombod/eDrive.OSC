using Newtonsoft.Json;

namespace eDrive.OSC.Serialisation.Json;

/// <summary>
///     Serializer for Osc Colour
/// </summary>
[CustomOscJSonSerializer('r', typeof(OscColour))]
public class OscColourSerializer : OscTypeJsonSerializer<OscColour>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OscColourSerializer" /> class.
    /// </summary>
    public OscColourSerializer()
        : base('r')
    {
    }

    public override OscColour Decode(JsonReader reader)
    {
        reader.Read();
        var r = (byte)(reader.ReadAsInt32() ?? 0);
        var g = (byte)(reader.ReadAsInt32() ?? 0);
        var b = (byte)(reader.ReadAsInt32() ?? 0);
        var a = (byte)(reader.ReadAsInt32() ?? 0);
        reader.Read();
        var ret = new OscColour(r, g, b, a);

        return ret;
    }

    public override void Encode(JsonWriter output, OscColour value)
    {
        output.WriteStartArray();
        output.WriteValue(value.R);
        output.WriteValue(value.G);
        output.WriteValue(value.B);
        output.WriteValue(value.A);
        output.WriteEndArray();
    }
}