using System;
using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for <see cref="Guid" />
    /// </summary>
    [CustomOscJSonSerialiser('g', typeof (Guid))]
    public class GuidSerialiser : OscTypeJsonSerialiser<Guid>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GuidSerialiser" /> class.
        /// </summary>
        public GuidSerialiser() : base('g')
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
}