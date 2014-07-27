using System;
using Newtonsoft.Json;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for <see cref="Version" />
    /// </summary>
    [CustomOscJSonSerialiser('v', typeof (Version))]
    public class VersionSerialiser : OscTypeJsonSerialiser<Version>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VersionSerialiser" /> class.
        /// </summary>
        public VersionSerialiser() : base('v')
        {
        }


        public override Version Decode(JsonReader reader)
        {
            var vs = JsonSerialiserFactory.StringSerialiser.Decode(reader);
            return Version.Parse(vs);
        }

        public override void Encode(JsonWriter output, Version value)
        {
            var d = value.ToString();
            JsonSerialiserFactory.StringSerialiser.Encode(output, d);
        }
    }
}