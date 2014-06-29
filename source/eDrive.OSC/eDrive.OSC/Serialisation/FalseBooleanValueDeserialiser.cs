#region

using System;
using System.IO;

#endregion

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Deserialises a <see cref="bool" /> true value from 'T' tag
    /// </summary>
    [CustomOscSerialiser('F')]
    public class FalseBooleanValueDeserialiser : OscTypeSerialiser<bool>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FalseBooleanValueDeserialiser" /> class.
        /// </summary>
        public FalseBooleanValueDeserialiser()
            : base('F')
        {
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override bool Decode(byte[] data, int start, out int position)
        {
            position = start;
            return false;
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">
        ///     if set to <c>true</c> [value].
        /// </param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">This serialiser can only read</exception>
        public override int Encode(Stream output, bool value)
        {
            throw new NotSupportedException("This serialiser can only read");
        }
    }
}