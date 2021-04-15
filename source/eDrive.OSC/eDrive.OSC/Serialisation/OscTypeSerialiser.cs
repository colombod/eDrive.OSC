using System;
using System.IO;

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     This class can be used to create serialisation strategies for types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OscTypeSerializer<T> : IEquatable<OscTypeSerializer<T>>,
                                                 IOscTypeSerializer<T>,
                                                 IOscTypeSerializer
    {
        private readonly char m_tag;
        private readonly Type m_type = typeof (T);

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscTypeSerializer{T}" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        protected OscTypeSerializer(char tag)
        {
            m_tag = tag;
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public bool Equals(OscTypeSerializer<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Tag == other.Tag;
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position in the data array after deserialisng.</param>
        /// <returns>
        ///     Deserialised data
        /// </returns>
        object IOscTypeSerializer.Decode(byte[] data, int start, out int position)
        {
            return Decode(data, start, out position);
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     Number of bytes written
        /// </returns>
        public int Encode(Stream output, object value)
        {
            return Encode(output, (T) value);
        }

        /// <summary>
        ///     Gets the tag.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public char GetTag(object value)
        {
            return value == null ? m_tag : GetTag((T) value);
        }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        public Type Type
        {
            get { return m_type; }
        }

        /// <summary>
        ///     Gets or sets the tag.
        /// </summary>
        /// <value>
        ///     The tag.
        /// </value>
        public char Tag
        {
            get { return m_tag; }
        }

        /// <summary>
        ///     Decodes from the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position in the data array after deserialisng.</param>
        /// <returns>Deserialised data</returns>
        public abstract T Decode(byte[] data, int start, out int position);

        /// <summary>
        ///     Encodes in the specified stream.
        /// </summary>
        /// <param name="output">The stream.</param>
        /// <param name="value">The value.</param>
        /// <returns>Number of bytes lastWritten</returns>
        public abstract int Encode(Stream output, T value);

        /// <summary>
        ///     Gets the tag.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public char GetTag(T value)
        {
            return InternalGetTag(value);
        }

        /// <summary>
        ///     Internals the get tag.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected virtual char InternalGetTag(T value)
        {
            return m_tag;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
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
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((OscTypeSerializer<T>) obj);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Tag;
        }

        /// <summary>
        ///     ==s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator ==(OscTypeSerializer<T> left, OscTypeSerializer<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator !=(OscTypeSerializer<T> left, OscTypeSerializer<T> right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        ///     Adjusts for endian.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        protected static byte[] AdjustForEndian(byte[] data, ref int start, int size)
        {
            if (BitConverter.IsLittleEndian != OscPacket.LittleEndianByteOrder)
            {
                var buffer = data.CopySubArray(start, size);
                data = Utility.SwapEndian(buffer);
                start = 0;
            }
            return data;
        }

        /// <summary>
        ///     Adjusts for endian in place.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="size">The size.</param>
        protected static void AdjustForEndianInPlace(ref byte[] data, ref int start, int size)
        {
            if (BitConverter.IsLittleEndian != OscPacket.LittleEndianByteOrder)
            {
                Utility.SwapEndianInPlace(ref data, start, size);
            }
        }

        /// <summary>
        ///     Pads the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="lastWritten">The lastWritten.</param>
        /// <param name="alignment">The alignment.</param>
        /// <returns>Total stream lenght, as last written size + what is needed to pad.</returns>
        protected static int PadStream(Stream stream, int lastWritten, int alignment)
        {
            if (alignment <= 0)
            {
                return lastWritten;
            }

            var topad = alignment - (lastWritten%alignment);

            stream.Write(new byte[topad], 0, topad);

            return lastWritten + topad;
        }

        /// <summary>
        ///     Writes the int.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        protected static void WriteInt(Stream output, int value)
        {
            output.Write(BitConverter.GetBytes(value), 0, 4);
        }

        /// <summary>
        ///     Pads the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="alignment">The alignment.</param>
        /// <returns></returns>
        protected static int PadValue(int value, int alignment)
        {
            var topad = alignment > 1 ? (alignment - (value%alignment)) : 0;
            return value + topad;
        }
    }
}