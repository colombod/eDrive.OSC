using System.Runtime.InteropServices;

namespace eDrive.OSC
{
    /// <summary>
    ///     Represents an osc colour.
    /// </summary>
    public class OscColour
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscColour" /> class.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <param name="a">A.</param>
        public OscColour(byte r, byte g, byte b, byte a)
        {
            m_storage.A = a;
            m_storage.R = r;
            m_storage.G = g;
            m_storage.B = b;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscColour" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OscColour(int message)
        {
            m_storage.Value = message;
        }

        /// <summary>
        ///     Gets or sets the A.
        /// </summary>
        /// <value>
        ///     The A.
        /// </value>
        public byte A
        {
            get => m_storage.A;
            set => m_storage.A = value;
        }

        /// <summary>
        ///     Gets or sets the B.
        /// </summary>
        /// <value>
        ///     The B.
        /// </value>
        public byte B
        {
            get => m_storage.B;
            set => m_storage.B = value;
        }

        /// <summary>
        ///     Gets or sets the G.
        /// </summary>
        /// <value>
        ///     The G.
        /// </value>
        public byte G
        {
            get => m_storage.G;
            set => m_storage.G = value;
        }

        /// <summary>
        ///     Gets or sets the R.
        /// </summary>
        /// <value>
        ///     The R.
        /// </value>
        public byte R
        {
            get => m_storage.R;
            set => m_storage.R = value;
        }

        /// <summary>
        ///     The message as int 32
        /// </summary>
        /// <returns></returns>
        public int ToInt32()
        {
            return m_storage.Value;
        }

        private Colour m_storage;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == GetType() && Equals((OscColour)obj);
        }

        public override int GetHashCode()
        {
            return m_storage.GetHashCode();
        }

        protected bool Equals(OscColour other)
        {
            return m_storage.Equals(other.m_storage);
        }

        /// <summary>
        ///     Struct that represents a colour.
        ///     A little-endian representation is used http://opensoundcontrol.org/spec-1_0
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct Colour
        {
            /// <summary>
            ///     The A
            /// </summary>
            [FieldOffset(3)] public byte A;

            /// <summary>
            ///     The B
            /// </summary>
            [FieldOffset(2)] public byte B;

            /// <summary>
            ///     The G
            /// </summary>
            [FieldOffset(1)] public byte G;

            /// <summary>
            ///     The R
            /// </summary>
            [FieldOffset(0)] public byte R;

            /// <summary>
            ///     The value
            /// </summary>
            [FieldOffset(0)] public int Value;
        }
    }
}