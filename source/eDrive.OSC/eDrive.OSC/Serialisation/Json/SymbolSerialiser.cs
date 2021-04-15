using Newtonsoft.Json;
using eDrive.Osc;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Deserializers for Symbol Tag
    /// </summary>
    [CustomOscJSonSerializer('S', typeof (OscSymbol))]
    public class SymbolSerializer : OscTypeJsonSerializer<OscSymbol>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SymbolSerializer" /> class.
        /// </summary>
        public SymbolSerializer() : base('S')
        {
        }

        public override OscSymbol Decode(JsonReader reader)
        {
            var value = JsonSerializerFactory.StringSerializer.Decode(reader);
            return new OscSymbol {Value = value};
        }

        public override void Encode(JsonWriter output, OscSymbol value)
        {
            JsonSerializerFactory.StringSerializer.Encode(output, value.Value);
        }
    }
}