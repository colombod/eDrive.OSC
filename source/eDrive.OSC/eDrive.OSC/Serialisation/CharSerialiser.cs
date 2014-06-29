#region

using System;
using System.IO;

#endregion

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serialiser for <see cref="char" />
    /// </summary>
    [CustomOscSerialiser('c', typeof (char))]
    public class CharSerialiser : OscTypeSerialiser<char>
    {
        private static IOscTypeSerialiser<int> s_intSer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CharSerialiser" /> class.
        /// </summary>
        public CharSerialiser() : base('c')
        {
        }

        private static IOscTypeSerialiser<int> IntSerialiser
        {
            get { return s_intSer ?? (s_intSer = SerialiserFactory.GetSerialiser<int>()); }
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
            var iv = IntSerialiser.Decode(data, start, out position);
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
            return IntSerialiser.Encode(output, v);
        }
    }
}