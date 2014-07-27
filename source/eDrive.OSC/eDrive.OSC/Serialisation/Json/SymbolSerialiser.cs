using Newtonsoft.Json;
using eDrive.Osc;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Deserialisers for Symbol Tag
    /// </summary>
    [CustomOscJSonSerialiser('S', typeof (OscSymbol))]
    public class SymbolSerialiser : OscTypeJsonSerialiser<OscSymbol>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SymbolSerialiser" /> class.
        /// </summary>
        public SymbolSerialiser() : base('S')
        {
        }

        public override OscSymbol Decode(JsonReader reader)
        {
            var value = JsonSerialiserFactory.StringSerialiser.Decode(reader);
            return new OscSymbol {Value = value};
        }

        public override void Encode(JsonWriter output, OscSymbol value)
        {
            JsonSerialiserFactory.StringSerialiser.Encode(output, value.Value);
        }
    }
}