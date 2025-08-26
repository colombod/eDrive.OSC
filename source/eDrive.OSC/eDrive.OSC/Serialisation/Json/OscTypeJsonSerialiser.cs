using Newtonsoft.Json;

using System;

namespace eDrive.OSC.Serialisation.Json
{
    /// <summary>
    ///     This class can be used to create serialisation strategies for types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OscTypeJsonSerializer<T> : IEquatable<OscTypeJsonSerializer<T>>,
                                                     IOscTypeJsonSerializer<T>,
                                                     IOscTypeJsonSerializer
    {
        private readonly char m_tag;
        private readonly Type m_type = typeof(T);

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscTypeJsonSerializer{T}" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        protected OscTypeJsonSerializer(char tag)
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
        public bool Equals(OscTypeJsonSerializer<T> other)
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
        /// <param name="reader">The reader.</param>
        /// <returns>
        ///     Deserialised data
        /// </returns>
        object IOscTypeJsonSerializer.Decode(JsonReader reader)
        {
            return Decode(reader);
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     Number of bytes written
        /// </returns>
        public void Encode(JsonWriter output, object value)
        {
            Encode(output, (T)value);
        }

        /// <summary>
        ///     Gets the tag.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public char GetTag(object value)
        {
            return value == null ? m_tag : GetTag((T)value);
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
        /// <param name="reader">The reader.</param>
        /// <returns>
        ///     Deserialised data
        /// </returns>
        public abstract T Decode(JsonReader reader);

        /// <summary>
        ///     Encodes in the specified stream.
        /// </summary>
        /// <param name="output">The stream.</param>
        /// <param name="value">The value.</param>
        public abstract void Encode(JsonWriter output, T value);

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
            return Equals((OscTypeSerializer<T>)obj);
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
        public static bool operator ==(OscTypeJsonSerializer<T> left, OscTypeJsonSerializer<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        ///     !=s the specified left.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        public static bool operator !=(OscTypeJsonSerializer<T> left, OscTypeJsonSerializer<T> right)
        {
            return !Equals(left, right);
        }
    }
}