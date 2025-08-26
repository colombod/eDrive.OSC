using System;

namespace eDrive.OSC.Serialisation
{
    /// <summary>
    ///     Represents an unexpected error during serialisation or deserialisation.
    /// </summary>
    public class OscSerializerException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscSerializerException" /> class.
        /// </summary>
        /// <param name="unsupportedType">The unsupported type.</param>
        /// <param name="inner">The inner exception.</param>
        public OscSerializerException(Type unsupportedType, Exception inner)
            : base(string.Format("Unsupported type: {0}", unsupportedType), inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscSerializerException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OscSerializerException(string message)
            : base(message)
        {
        }


    }
}