using System;
using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Deserialises a <see cref="bool" /> true value from 'T' tag
    /// </summary>
    [CustomOscJSonSerializer('F')]
    public class FalseBooleanValueDeserializer : OscTypeJsonSerializer<bool>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FalseBooleanValueDeserializer" /> class.
        /// </summary>
        public FalseBooleanValueDeserializer()
            : base('F')
        {
        }

        public override bool Decode(JsonReader reader)
        {
            return false;
        }

        public override void Encode(JsonWriter output, bool value)
        {
            throw new NotSupportedException("This serializer can only read");
        }
    }
}