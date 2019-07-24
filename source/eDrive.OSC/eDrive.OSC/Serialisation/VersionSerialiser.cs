using System;
using System.IO;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serializer for <see cref="Version" />
    /// </summary>
    [CustomOscSerializer('v', typeof (Version))]
    public class VersionSerializer : OscTypeSerializer<Version>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VersionSerializer" /> class.
        /// </summary>
        public VersionSerializer() : base('v')
        {
        }

        public override Version Decode(byte[] data, int start, out int position)
        {
            var vs = SerializerFactory.StringSerializer.Decode(data, start, out position);
            return Version.Parse(vs);
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int Encode(Stream output, Version value)
        {
            var d = value.ToString();
            return SerializerFactory.StringSerializer.Encode(output, d);
        }
    }
}