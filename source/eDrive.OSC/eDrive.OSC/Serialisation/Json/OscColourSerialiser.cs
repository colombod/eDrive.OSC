using Newtonsoft.Json;
using eDrive.Osc;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for Osc Colour
    /// </summary>
    [CustomOscJSonSerialiser('r', typeof (OscColour))]
    public class OscColourSerialiser : OscTypeJsonSerialiser<OscColour>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscColourSerialiser" /> class.
        /// </summary>
        public OscColourSerialiser()
            : base('r')
        {
        }

        public override OscColour Decode(JsonReader reader)
        {
            reader.Read();
            var r = (byte) (reader.ReadAsInt32() ?? 0);
            var g = (byte) (reader.ReadAsInt32() ?? 0);
            var b = (byte) (reader.ReadAsInt32() ?? 0);
            var a = (byte) (reader.ReadAsInt32() ?? 0);
            reader.Read();
            var ret = new OscColour(r, g, b, a);

            return ret;
        }

        public override void Encode(JsonWriter output, OscColour value)
        {
            output.WriteStartArray();
            output.WriteValue(value.R);
            output.WriteValue(value.G);
            output.WriteValue(value.B);
            output.WriteValue(value.A);
            output.WriteEndArray();
        }
    }
}