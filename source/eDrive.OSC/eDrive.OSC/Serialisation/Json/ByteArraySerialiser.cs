using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace eDrive.OSC.Serialisation.Json;

/// <summary>
///     Serializer for binary blob.
/// </summary>
[CustomOscJSonSerializer('b', typeof(byte[]))]
public class ByteArraySerializer : OscTypeJsonSerializer<byte[]>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ByteArraySerializer" /> class.
    /// </summary>
    public ByteArraySerializer() : base('b')
    {
    }


    public override byte[] Decode(JsonReader reader)
    {
        var ret = new List<byte>();
        
        // Read the start array token
        if (reader.TokenType != JsonToken.StartArray)
        {
            reader.Read();
        }
        
        // Read array elements
        while (reader.Read() && reader.TokenType != JsonToken.EndArray)
        {
            if (reader.TokenType == JsonToken.Integer && reader.Value != null)
            {
                var value = Convert.ToInt32(reader.Value);
                ret.Add((byte)value);
            }
        }
        
        return ret.ToArray();
    }

    public override void Encode(JsonWriter output, byte[] value)
    {
        output.WriteStartArray();
        if (value != null
            && value.Length > 0)
        {
            for (var i = 0; i < value.Length; i++)
            {
                output.WriteValue(value[i]);
            }
        }
        output.WriteEndArray();
    }
}