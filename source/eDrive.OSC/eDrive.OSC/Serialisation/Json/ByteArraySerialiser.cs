
using System.Collections.Generic;
using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for binary blob.
    /// </summary>
    [CustomOscJSonSerialiser('b', typeof (byte[]))]
    public class ByteArraySerialiser : OscTypeJsonSerialiser<byte[]>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteArraySerialiser" /> class.
        /// </summary>
        public ByteArraySerialiser() : base('b')
        {
        }


        public override byte[] Decode(JsonReader reader)
        {
            reader.Read();
            var ret = new List<byte>();
            while (reader.TokenType != JsonToken.EndArray)
            {
                ret.Add((byte) reader.Value);
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