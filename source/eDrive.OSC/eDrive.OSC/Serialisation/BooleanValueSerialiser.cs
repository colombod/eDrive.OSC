#region

using System;
using System.IO;

#endregion

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     This serialiser is used to write <see cref="bool" /> values.
    ///     No payload will be serialised only the Tag is used.
    /// </summary>
    [CustomOscSerialiser(typeof (bool))]
    public class BooleanValueSerialiser : OscTypeSerialiser<bool>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BooleanValueSerialiser" /> class.
        /// </summary>
        public BooleanValueSerialiser() : base(' ')
        {
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">This serialiser can only write</exception>
        public override bool Decode(byte[] data, int start, out int position)
        {
            throw new NotSupportedException("This serialiser can only write");
        }

        /// <summary>
        ///     Internals the get tag.
        /// </summary>
        /// <param name="value">
        ///     if set to <c>true</c> [value].
        /// </param>
        /// <returns></returns>
        protected override char InternalGetTag(bool value)
        {
            return value ? 'T' : 'F';
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">
        ///     if set to <c>true</c> [value].
        /// </param>
        /// <returns></returns>
        public override int Encode(Stream output, bool value)
        {
            return 0;
        }
    }
}