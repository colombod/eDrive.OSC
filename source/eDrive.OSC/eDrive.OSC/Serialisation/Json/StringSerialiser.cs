using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for <see cref="string" />
    /// </summary>
    [CustomOscJSonSerialiser('s', typeof (string))]
    public class StringSerialiser : OscTypeJsonSerialiser<string>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StringSerialiser" /> class.
        /// </summary>
        public StringSerialiser() : base('s')
        {
        }


        public override string Decode(JsonReader reader)
        {
            var ret = reader.ReadAsString();
            return ret;
        }

        public override void Encode(JsonWriter output, string value)
        {
            output.WriteValue(value);
        }
    }
}