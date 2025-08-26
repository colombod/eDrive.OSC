using System.IO;

namespace eDrive.OSC.Serialisation;

/// <summary>
///     Deserialises Null payload
/// </summary>
[CustomOscSerializer('N')]
public class NilSerializer : OscTypeSerializer<NilSerializer.Nil>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NilSerializer" /> class.
    /// </summary>
    public NilSerializer() : base('N')
    {
    }

    /// <summary>
    ///     Decodes the specified data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="start">The start.</param>
    /// <param name="position">The position.</param>
    /// <returns></returns>
    public override Nil Decode(byte[] data, int start, out int position)
    {
        position = start;
        return null;
    }

    /// <summary>
    ///     Encodes the specified output.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public override int Encode(Stream output, Nil value)
    {
        return 0;
    }

    /// <summary>
    ///     Tag Type for Nil.
    /// </summary>
    public class Nil
    {
    }
}