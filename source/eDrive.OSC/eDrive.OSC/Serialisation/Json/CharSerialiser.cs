using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serializer for <see cref="char" />
    /// </summary>
    [CustomOscJSonSerializer('c', typeof (char))]
    public class CharSerializer : OscTypeJsonSerializer<char>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CharSerializer" /> class.
        /// </summary>
        public CharSerializer() : base('c')
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