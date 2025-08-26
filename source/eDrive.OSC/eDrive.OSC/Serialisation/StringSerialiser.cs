using System.IO;
using System.Text;

namespace eDrive.OSC.Serialisation;

/// <summary>
///     Serializer for <see cref="string" />
/// </summary>
[CustomOscSerializer('s', typeof(string))]
public class StringSerializer : OscTypeSerializer<string>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StringSerializer" /> class.
    /// </summary>
    public StringSerializer() : base('s')
    {
    }

    /// <summary>
    ///     Decodes the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="start">The start.</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public override string Decode(byte[] data, int start, out int position)
    {
        var count = 0;
        for (var index = start; index < data.Length && data[index] != 0; index++)
        {
            count++;
        }
        var ret = Encoding.UTF8.GetString(data, start, count);

        position = start + ret.Length;
        position += (4 - (position % 4));

        return ret;
    }

    /// <summary>
    ///     Encodes the specified output.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public override int Encode(Stream output, string value)
    {
        var data = Encoding.UTF8.GetBytes(value);
        output.Write(data, 0, data.Length);

        var ret = data.Length;
        ret = PadStream(output, ret, 4);

        return ret;
    }
}