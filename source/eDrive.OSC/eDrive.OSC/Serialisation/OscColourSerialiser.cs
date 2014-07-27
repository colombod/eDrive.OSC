using System.IO;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serialiser for Osc Colour
    /// </summary>
    [CustomOscSerialiser('r', typeof (OscColour))]
    public class OscColourSerialiser : OscTypeSerialiser<OscColour>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscColourSerialiser" /> class.
        /// </summary>
        public OscColourSerialiser()
            : base('r')
        {
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override OscColour Decode(byte[] data, int start, out int position)
        {
            var msg = SerialiserFactory.IntSerialiser.Decode(data, start, out position);
            var ret = new OscColour(msg);
            return ret;
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, OscColour value)
        {
            var data = value.ToInt32();
            return SerialiserFactory.IntSerialiser.Encode(output, data);
        }
    }
}