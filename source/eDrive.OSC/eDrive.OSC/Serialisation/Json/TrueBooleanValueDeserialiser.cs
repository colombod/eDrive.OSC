using System;
using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Deserialises a <see cref="bool" /> true value from 'T' tag
    /// </summary>
    [CustomOscJSonSerialiser('T')]
    public class TrueBooleanValueDeserialiser : OscTypeJsonSerialiser<bool>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TrueBooleanValueDeserialiser" /> class.
        /// </summary>
        public TrueBooleanValueDeserialiser() : base('T')
        {
        }

        public override bool Decode(JsonReader reader)
        {
            return true;
        }

        public override void Encode(JsonWriter output, bool value)
        {
            throw new NotSupportedException("This serialiser can only read");
        }
    }
}