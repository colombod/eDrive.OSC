using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Deserialises Null payload
    /// </summary>
    [CustomOscJSonSerialiser('N')]
    public class NilSerialiser : OscTypeJsonSerialiser<NilSerialiser.Nil>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NilSerialiser" /> class.
        /// </summary>
        public NilSerialiser() : base('N')
        {
        }

        public override Nil Decode(JsonReader reader)
        {
            return null;
        }

        public override void Encode(JsonWriter output, Nil value)
        {
        }

        #region Nested type: Nil

        /// <summary>
        ///     Tag Type for Nil.
        /// </summary>
        public class Nil
        {
        }

        #endregion
    }
}