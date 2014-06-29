#region

using System;
using System.IO;

#endregion

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serialiser for <see cref="Guid" />
    /// </summary>
    [CustomOscSerialiser('g', typeof (Guid))]
    public class GuidSerialiser : OscTypeSerialiser<Guid>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GuidSerialiser" /> class.
        /// </summary>
        public GuidSerialiser() : base('g')
        {
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override Guid Decode(byte[] data, int start, out int position)
        {
            var src = SerialiserFactory.ByteArraySerialiser.Decode(data, start, out position);
            return new Guid(src);
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, Guid value)
        {
            var data = value.ToByteArray();
            return SerialiserFactory.ByteArraySerialiser.Encode(output, data);
        }
    }
}