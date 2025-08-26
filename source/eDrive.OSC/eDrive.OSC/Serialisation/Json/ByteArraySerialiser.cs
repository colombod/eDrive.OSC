using Newtonsoft.Json;

using System.Collections.Generic;

namespace eDrive.OSC.Serialisation.Json
{
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
            reader.Read();
            var ret = new List<byte>();
            while (reader.TokenType != JsonToken.EndArray)
            {
                ret.Add((byte)reader.Value);
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
}