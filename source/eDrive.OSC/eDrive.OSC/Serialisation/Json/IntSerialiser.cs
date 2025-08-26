using Newtonsoft.Json;

namespace eDrive.OSC.Serialisation.Json
{
    /// <summary>
    ///     Serializer for <see cref="int" />.
    /// </summary>
    [CustomOscJSonSerializer('i', typeof(int))]
    public class IntSerializer : OscTypeJsonSerializer<int>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="IntSerializer" /> class.
        /// </summary>
        public IntSerializer()
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