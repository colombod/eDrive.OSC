using System;
using System.IO;

namespace eDrive.OSC.Serialisation
{
    /// <summary>
    ///     The interface for all serializer strategies.
    /// </summary>
    public interface IOscTypeSerializer
    {
        /// <summary>
        ///     Gets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        Type Type { get; }

        /// <summary>
        ///     Gets or sets the tag.
        /// </summary>
        /// <value>
        ///     The tag.
        /// </value>
        char Tag { get; }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position in the data array after deserialisng.</param>
        /// <returns>Deserialised data</returns>
        object Decode(byte[] data, int start, out int position);

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of bytes written</returns>
        int Encode(Stream output, object value);

        /// <summary>
        ///     Gets the tag.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        char GetTag(object value);
    }

    /// <summary>
    ///     Contract for custom osc serializer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOscTypeSerializer<T>
    {
        /// <summary>
        ///     Gets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        Type Type { get; }

        /// <summary>
        ///     Gets or sets the tag.
        /// </summary>
        /// <value>
        ///     The tag.
        /// </value>
        char Tag { get; }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position in the data array after deserialising.</param>
        /// <returns>
        ///     Deserialised data
        /// </returns>
        T Decode(byte[] data, int start, out int position);

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of bytes written</returns>
        int Encode(Stream output, T value);

        /// <summary>
        ///     Gets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        char GetTag(T value);
    }
}