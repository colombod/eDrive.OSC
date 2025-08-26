using System;
using System.IO;

namespace eDrive.OSC.Serialisation
{
    /// <summary>
    ///     Encdodes and decodes midi messages
    /// </summary>
    [CustomOscSerializer('m', typeof(OscMidiMessage))]
    public class OscMidiSerializer : OscTypeSerializer<OscMidiMessage>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscMidiSerializer" /> class.
        /// </summary>
        public OscMidiSerializer() : base('m')
        {
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override OscMidiMessage Decode(byte[] data, int start, out int position)
        {
            position = 4 + start;

            var ret = new OscMidiMessage(data[3 + start], data[2 + start], data[1 + start], data[start]);
            return ret;
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, OscMidiMessage value)
        {
            var data = value.ToInt32();

            var buffer = BitConverter.GetBytes(data);
            output.Write(buffer, 0, buffer.Length);

            return buffer.Length;
        }
    }
}