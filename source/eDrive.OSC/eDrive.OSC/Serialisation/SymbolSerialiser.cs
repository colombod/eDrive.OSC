using System.IO;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Deserializers for Symbol Tag
    /// </summary>
    [CustomOscSerializer('S', typeof (OscSymbol))]
    public class SymbolSerializer : OscTypeSerializer<OscSymbol>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SymbolSerializer" /> class.
        /// </summary>
        public SymbolSerializer() : base('S')
        {
        }


        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override OscSymbol Decode(byte[] data, int start, out int position)
        {
            var value = SerializerFactory.StringSerializer.Decode(data, start, out position);
            return new OscSymbol {Value = value};
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, OscSymbol value)
        {
            return SerializerFactory.StringSerializer.Encode(output, value.Value);
        }
    }
}