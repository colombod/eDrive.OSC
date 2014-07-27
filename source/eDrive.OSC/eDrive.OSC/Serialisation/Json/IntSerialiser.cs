using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for <see cref="int" />.
    /// </summary>
    [CustomOscJSonSerialiser('i', typeof (int))]
    public class IntSerialiser : OscTypeJsonSerialiser<int>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IntSerialiser" /> class.
        /// </summary>
        public IntSerialiser()
            : base('i')
        {
        }

        public override int Decode(JsonReader reader)
        {
            var ret = reader.ReadAsInt32() ?? 0;
            return ret;
        }

        public override void Encode(JsonWriter output, int value)
        {
            output.WriteValue(value);
        }
    }
}