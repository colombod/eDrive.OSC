using Newtonsoft.Json;

namespace eDrive.OSC.Serialisation.Json;

/// <summary>
///     Serializer of <see cref="OscTimeTag" />.
/// </summary>
[CustomOscJSonSerializer('t', typeof(OscTimeTag))]
public class OscTimeTagSerializer : OscTypeJsonSerializer<OscTimeTag>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OscTimeTagSerializer" /> class.
    /// </summary>
    public OscTimeTagSerializer() : base('t')
    {
    }

    public override OscTimeTag Decode(JsonReader reader)
    {
        var v = reader.ReadAsString();
        var ret = ulong.Parse(v);
        return new OscTimeTag(ret);
    }

    public override void Encode(JsonWriter output, OscTimeTag value)
    {
        output.WriteValue(value.TimeTag);
    }
}