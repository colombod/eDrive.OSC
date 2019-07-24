using System;
using System.IO;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serializer of <see cref="OscTimeTag" />.
    /// </summary>
    [CustomOscSerializer('t', typeof (OscTimeTag))]
    public class OscTimeTagSerializer : OscTypeSerializer<OscTimeTag>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscTimeTagSerializer" /> class.
        /// </summary>
        public OscTimeTagSerializer() : base('t')
        {
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override OscTimeTag Decode(byte[] data, int start, out int position)
        {
            if (BitConverter.IsLittleEndian != OscPacket.LittleEndianByteOrder)
            {
                Utility.SwapEndianInPlace(ref data, start, 8);
            }

            var tag = BitConverter.ToUInt64(data, start);

            var ret = new OscTimeTag(tag);
            position = start + 8;
            return ret;
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, OscTimeTag value)
        {
            var data = BitConverter.GetBytes(value.TimeTag);
            if (BitConverter.IsLittleEndian != OscPacket.LittleEndianByteOrder)
            {
                Utility.SwapEndianInPlace(ref data, 0, 8);
            }
            output.Write(data, 0, 8);
            return 8;
        }
    }
}