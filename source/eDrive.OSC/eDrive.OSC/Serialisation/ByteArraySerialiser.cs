using System.IO;

namespace eDrive.OSC.Serialisation;

/// <summary>
///     Serializer for binary blob.
/// </summary>
[CustomOscSerializer('b', typeof(byte[]))]
public class ByteArraySerializer : OscTypeSerializer<byte[]>
{
    private static IOscTypeSerializer<int> s_intSer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ByteArraySerializer" /> class.
    /// </summary>
    public ByteArraySerializer() : base('b')
    {
    }

    private static IOscTypeSerializer<int> IntSerializer
    {
        get { return s_intSer ?? (s_intSer = SerializerFactory.GetSerializer<int>()); }
    }

    /// <summary>
    ///     Decodes the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="start">The start.</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public override byte[] Decode(byte[] data, int start, out int position)
    {
        var length = IntSerializer.Decode(data, start, out position);

        var buffer = data.CopySubArray(position, length);

        position += buffer.Length;
        position += (4 - (position % 4));

        return buffer;
    }

    /// <summary>
    ///     Encodes the specified output.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public override int Encode(Stream output, byte[] value)
    {
        var ret = IntSerializer.Encode(output, value.Length);

        output.Write(value, 0, value.Length);

        ret += value.Length;

        ret = PadStream(output, ret, 4);

        return ret;
    }
}