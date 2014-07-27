using System;
using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     This serialiser is used to write <see cref="bool" /> values.
    ///     No payload will be serialised only the Tag is used.
    /// </summary>
    [CustomOscJSonSerialiser(typeof (bool))]
    public class BooleanValueSerialiser : OscTypeJsonSerialiser<bool>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BooleanValueSerialiser" /> class.
        /// </summary>
        public BooleanValueSerialiser() : base(' ')
        {
        }


        /// <summary>
        ///     Decodes the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">This serialiser can only write</exception>
        public override bool Decode(JsonReader reader)
        {
            throw new NotSupportedException("This serialiser can only write");
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">
        ///     if set to <c>true</c> [value].
        /// </param>
        public override void Encode(JsonWriter output, bool value)
        {
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
    }
}