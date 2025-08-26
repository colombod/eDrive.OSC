using Newtonsoft.Json;

using System;

namespace eDrive.OSC.Serialisation.Json;

/// <summary>
///     Deserialises a <see cref="bool" /> true value from 'T' tag
/// </summary>
[CustomOscJSonSerializer('T')]
public class TrueBooleanValueDeserializer : OscTypeJsonSerializer<bool>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TrueBooleanValueDeserializer" /> class.
    /// </summary>
    public TrueBooleanValueDeserializer() : base('T')
    {
    }

    public override bool Decode(JsonReader reader)
    {
        return true;
    }

    public override void Encode(JsonWriter output, bool value)
    {
        throw new NotSupportedException("This serializer can only read");
    }
}