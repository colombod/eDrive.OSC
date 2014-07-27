using System;
using System.IO;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serialiser for <see cref="float" />
    /// </summary>
    [CustomOscSerialiser('f', typeof (float))]
    public class FloatSerialiser : OscTypeSerialiser<float>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DoubleSerialiser" /> class.
        /// </summary>
        public FloatSerialiser()
            : base('f')
        {
        }

        protected override char InternalGetTag(float value)
        {
            return float.IsInfinity(value) ? 'I' : Tag;
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override float Decode(byte[] data, int start, out int position)
        {
            const int size = sizeof (float);
            position = start + size;

            AdjustForEndianInPlace(ref data, ref start, size);

            var value = BitConverter.ToSingle(data, start);

            return value;
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, float value)
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