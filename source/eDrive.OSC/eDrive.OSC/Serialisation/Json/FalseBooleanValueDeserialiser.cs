using System;
using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Deserialises a <see cref="bool" /> true value from 'T' tag
    /// </summary>
    [CustomOscJSonSerialiser('F')]
    public class FalseBooleanValueDeserialiser : OscTypeJsonSerialiser<bool>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FalseBooleanValueDeserialiser" /> class.
        /// </summary>
        public FalseBooleanValueDeserialiser()
            : base('F')
        {
        }

        public override bool Decode(JsonReader reader)
        {
            return false;
        }

        public override void Encode(JsonWriter output, bool value)
        {
            throw new NotSupportedException("This serialiser can only read");
        }
    }
}