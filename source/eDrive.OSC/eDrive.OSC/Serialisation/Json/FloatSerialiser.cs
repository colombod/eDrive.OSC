using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for <see cref="float" />
    /// </summary>
    [CustomOscJSonSerialiser('f', typeof (float))]
    public class FloatSerialiser : OscTypeJsonSerialiser<float>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Network.Http.Serialisation.DoubleSerialiser" /> class.
        /// </summary>
        public FloatSerialiser()
            : base('f')
        {
        }

        public override float Decode(JsonReader reader)
        {
            var ret = (float) (reader.ReadAsDecimal() ?? 0);
            return ret;
        }

        public override void Encode(JsonWriter output, float value)
        {
            output.WriteValue(value);
        }

        protected override char InternalGetTag(float value)
        {
            return float.IsInfinity(value) ? 'I' : Tag;
        }
    }
}