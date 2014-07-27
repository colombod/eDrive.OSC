using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for <see cref="double" />
    /// </summary>
    [CustomOscJSonSerialiser('d', typeof (double))]
    public class DoubleSerialiser : OscTypeJsonSerialiser<double>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DoubleSerialiser" /> class.
        /// </summary>
        public DoubleSerialiser() : base('d')
        {
        }


        public override double Decode(JsonReader reader)
        {
            var ret = 0.0;
            try
            {
                reader.Read();

                ret = (double) reader.Value;

            }
            catch
            {
                ret = double.NaN;
            }

            return ret;
        }

        public override void Encode(JsonWriter output, double value)
        {
            output.WriteValue(value);
        }
    }
}