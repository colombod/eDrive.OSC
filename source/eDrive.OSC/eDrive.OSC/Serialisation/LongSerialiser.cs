using System;
using System.IO;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serializer for <see cref="long" />
    /// </summary>
    [CustomOscSerializer('h', typeof (long))]
    public class LongSerializer : OscTypeSerializer<long>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LongSerializer" /> class.
        /// </summary>
        public LongSerializer()
            : base('h')
        {
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override long Decode(byte[] data, int start, out int position)
        {
            const int size = sizeof (long);
            position = start + size;

            AdjustForEndianInPlace(ref data, ref start, size);

            var value = BitConverter.ToInt64(data, start);

            return value;
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, long value)
        {
            var data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian != OscPacket.LittleEndianByteOrder)
            {
                Utility.SwapEndianInPlace(ref data);
            }

            output.Write(data, 0, data.Length);

            return data.Length;
        }
    }
}