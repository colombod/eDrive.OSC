using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for <see cref="long" />
    /// </summary>
    [CustomOscJSonSerialiser('h', typeof (long))]
    public class LongSerialiser : OscTypeJsonSerialiser<long>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LongSerialiser" /> class.
        /// </summary>
        public LongSerialiser()
            : base('h')
        {
        }

        public override long Decode(JsonReader reader)
        {
            var ret = long.Parse(reader.ReadAsString());
            return ret;
        }

        public override void Encode(JsonWriter output, long value)
        {
            output.WriteValue(value);
        }
    }
}