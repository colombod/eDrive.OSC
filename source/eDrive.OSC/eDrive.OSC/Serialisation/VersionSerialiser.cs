#region

using System;
using System.IO;

#endregion

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serialiser for <see cref="Version" />
    /// </summary>
    [CustomOscSerialiser('v', typeof (Version))]
    public class VersionSerialiser : OscTypeSerialiser<Version>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VersionSerialiser" /> class.
        /// </summary>
        public VersionSerialiser() : base('v')
        {
        }

        public override Version Decode(byte[] data, int start, out int position)
        {
            var vs = SerialiserFactory.StringSerialiser.Decode(data, start, out position);
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
            return SerialiserFactory.StringSerialiser.Encode(output, d);
        }
    }
}