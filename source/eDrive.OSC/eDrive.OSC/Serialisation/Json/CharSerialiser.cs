using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for <see cref="char" />
    /// </summary>
    [CustomOscJSonSerialiser('c', typeof (char))]
    public class CharSerialiser : OscTypeJsonSerialiser<char>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CharSerialiser" /> class.
        /// </summary>
        public CharSerialiser() : base('c')
        {
        }

        public override char Decode(JsonReader reader)
        {
            var ret = reader.ReadAsString()[0];
            return ret;
        }

        public override void Encode(JsonWriter output, char value)
        {
            output.WriteValue(value);
        }
    }
}