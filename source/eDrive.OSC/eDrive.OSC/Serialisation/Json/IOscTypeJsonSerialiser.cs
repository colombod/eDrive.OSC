using Newtonsoft.Json;

using System;

namespace eDrive.OSC.Serialisation.Json;

/// <summary>
///     The interface for all serializer strategies.
/// </summary>
public interface IOscTypeJsonSerializer
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
    /// <param name="reader">The reader.</param>
    /// <returns>
    ///     Deserialised data
    /// </returns>
    object Decode(JsonReader reader);

    /// <summary>
    ///     Encodes the specified output.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <param name="value">The value.</param>
    /// <returns>Number of bytes written</returns>
    void Encode(JsonWriter output, object value);

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
public interface IOscTypeJsonSerializer<T>
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
    /// <param name="reader">The reader.</param>
    /// <returns>
    ///     Deserialised data
    /// </returns>
    T Decode(JsonReader reader);

    /// <summary>
    ///     Encodes the specified output.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <param name="value">The value.</param>
    /// <returns>
    ///     Number of bytes written
    /// </returns>
    void Encode(JsonWriter output, T value);

    /// <summary>
    ///     Gets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    char GetTag(T value);
}