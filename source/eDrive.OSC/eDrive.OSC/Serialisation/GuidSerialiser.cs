using System;
using System.IO;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serializer for <see cref="Guid" />
    /// </summary>
    [CustomOscSerializer('g', typeof (Guid))]
    public class GuidSerializer : OscTypeSerializer<Guid>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GuidSerializer" /> class.
        /// </summary>
        public GuidSerializer() : base('g')
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
            var src = SerializerFactory.ByteArraySerializer.Decode(data, start, out position);
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
            return SerializerFactory.ByteArraySerializer.Encode(output, data);
        }
    }
}