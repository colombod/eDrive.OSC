using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serializer for <see cref="string" />
    /// </summary>
    [CustomOscJSonSerializer('s', typeof (string))]
    public class StringSerializer : OscTypeJsonSerializer<string>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StringSerializer" /> class.
        /// </summary>
        public StringSerializer() : base('s')
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