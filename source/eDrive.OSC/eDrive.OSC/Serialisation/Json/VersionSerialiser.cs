using Newtonsoft.Json;

using System;

namespace eDrive.OSC.Serialisation.Json
{
    /// <summary>
    ///     Serializer for <see cref="Version" />
    /// </summary>
    [CustomOscJSonSerializer('v', typeof(Version))]
    public class VersionSerializer : OscTypeJsonSerializer<Version>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VersionSerializer" /> class.
        /// </summary>
        public VersionSerializer() : base('v')
        {
        }


        public override Version Decode(JsonReader reader)
        {
            var vs = JsonSerializerFactory.StringSerializer.Decode(reader);
            return Version.Parse(vs);
        }

        public override void Encode(JsonWriter output, Version value)
        {
            var d = value.ToString();
            JsonSerializerFactory.StringSerializer.Encode(output, d);
        }
    }
}