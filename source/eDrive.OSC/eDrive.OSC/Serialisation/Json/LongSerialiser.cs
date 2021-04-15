using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serializer for <see cref="long" />
    /// </summary>
    [CustomOscJSonSerializer('h', typeof (long))]
    public class LongSerializer : OscTypeJsonSerializer<long>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LongSerializer" /> class.
        /// </summary>
        public LongSerializer()
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