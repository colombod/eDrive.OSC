using Newtonsoft.Json;

namespace eDrive.OSC.Serialisation.Json
{
    /// <summary>
    ///     Deserialises Null payload
    /// </summary>
    [CustomOscJSonSerializer('N')]
    public class NilSerializer : OscTypeJsonSerializer<NilSerializer.Nil>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NilSerializer" /> class.
        /// </summary>
        public NilSerializer() : base('N')
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