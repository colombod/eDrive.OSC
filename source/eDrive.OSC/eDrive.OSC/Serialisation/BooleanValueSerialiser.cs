using System;
using System.IO;

namespace eDrive.OSC.Serialisation;

/// <summary>
///     This serializer is used to write <see cref="bool" /> values.
///     No payload will be serialised only the Tag is used.
/// </summary>
[CustomOscSerializer(typeof(bool))]
public class BooleanValueSerializer : OscTypeSerializer<bool>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BooleanValueSerializer" /> class.
    /// </summary>
    public BooleanValueSerializer() : base(' ')
    {
    }

    /// <summary>
    ///     Decodes the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="start">The start.</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    /// <exception cref="System.NotSupportedException">This serializer can only write</exception>
    public override bool Decode(byte[] data, int start, out int position)
    {
        throw new NotSupportedException("This serializer can only write");
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