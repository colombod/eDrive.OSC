using Newtonsoft.Json;
using eDrive.Osc;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser of <see cref="OscTimeTag" />.
    /// </summary>
    [CustomOscJSonSerialiser('t', typeof (OscTimeTag))]
    public class OscTimeTagSerialiser : OscTypeJsonSerialiser<OscTimeTag>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscTimeTagSerialiser" /> class.
        /// </summary>
        public OscTimeTagSerialiser() : base('t')
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
}