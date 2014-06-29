#region

using System;
using System.IO;

#endregion

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serialiser for <see cref="double" />
    /// </summary>
    [CustomOscSerialiser('d', typeof (double))]
    public class DoubleSerialiser : OscTypeSerialiser<double>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DoubleSerialiser" /> class.
        /// </summary>
        public DoubleSerialiser() : base('d')
        {
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override double Decode(byte[] data, int start, out int position)
        {
            const int size = sizeof (double);
            position = start + size;

            AdjustForEndianInPlace(ref data, ref start, size);

            var value = BitConverter.ToDouble(data, start);

            return value;
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, double value)
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