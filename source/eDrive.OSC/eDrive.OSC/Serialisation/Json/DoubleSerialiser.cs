using Newtonsoft.Json;

namespace eDrive.OSC.Serialisation.Json
{
    /// <summary>
    ///     Serializer for <see cref="double" />
    /// </summary>
    [CustomOscJSonSerializer('d', typeof(double))]
    public class DoubleSerializer : OscTypeJsonSerializer<double>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DoubleSerializer" /> class.
        /// </summary>
        public DoubleSerializer() : base('d')
        {
        }


        public override double Decode(JsonReader reader)
        {
            var ret = 0.0;
            try
            {
                reader.Read();

                ret = (double)reader.Value;

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