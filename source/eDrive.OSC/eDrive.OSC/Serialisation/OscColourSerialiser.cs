using System.IO;

namespace eDrive.OSC.Serialisation
{
    /// <summary>
    ///     Serializer for Osc Colour
    /// </summary>
    [CustomOscSerializer('r', typeof(OscColour))]
    public class OscColourSerializer : OscTypeSerializer<OscColour>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscColourSerializer" /> class.
        /// </summary>
        public OscColourSerializer()
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
            var msg = SerializerFactory.IntSerializer.Decode(data, start, out position);
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
            return SerializerFactory.IntSerializer.Encode(output, data);
        }
    }
}