using System;
using System.Runtime.Serialization;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Represents an unexpected error during serialisation or deserialisation.
    /// </summary>
    public class OscSerialiserException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscSerialiserException" /> class.
        /// </summary>
        /// <param name="unsupportedType">The unsupported type.</param>
        /// <param name="inner">The inner exception.</param>
        public OscSerialiserException(Type unsupportedType, Exception inner)
            : base(string.Format("Unsupported type: {0}", unsupportedType), inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscSerialiserException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OscSerialiserException(string message)
			:  base(message)
        {
        }

	
    }
}