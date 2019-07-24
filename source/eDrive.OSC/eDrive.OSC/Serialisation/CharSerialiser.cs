using System;
using System.IO;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serializer for <see cref="char" />
    /// </summary>
    [CustomOscSerializer('c', typeof (char))]
    public class CharSerializer : OscTypeSerializer<char>
    {
        private static IOscTypeSerializer<int> s_intSer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CharSerializer" /> class.
        /// </summary>
        public CharSerializer() : base('c')
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
        public override char Decode(byte[] data, int start, out int position)
        {
            var iv = IntSerializer.Decode(data, start, out position);
            var ret = Convert.ToChar(iv);
            return ret;
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, char value)
        {
            var v = Convert.ToInt32(value);
            return IntSerializer.Encode(output, v);
        }
    }
}