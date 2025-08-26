using Newtonsoft.Json;

using System;

namespace eDrive.OSC.Serialisation.Json;

/// <summary>
///     Serializer for <see cref="Guid" />
/// </summary>
[CustomOscJSonSerializer('g', typeof(Guid))]
public class GuidSerializer : OscTypeJsonSerializer<Guid>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GuidSerializer" /> class.
    /// </summary>
    public GuidSerializer() : base('g')
    {
    }

    public override Guid Decode(JsonReader reader)
    {
        var v = reader.ReadAsString();
        var ret = Guid.Parse(v);
        return ret;
    }

    public override void Encode(JsonWriter output, Guid value)
    {
        output.WriteValue(value.ToString());
    }
}