#region

using System;

#endregion

namespace eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Marks classes as custom osc sersialiser
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CustomOscSerialiserAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomOscSerialiserAttribute" /> class.
        /// </summary>
        /// <param name="typeTag">The type tag.</param>
        /// <param name="type">The type tag.</param>
        public CustomOscSerialiserAttribute(char typeTag, Type type)
        {
            TypeTag = typeTag;
            Type = type;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomOscSerialiserAttribute" /> class.
        /// </summary>
        /// <param name="type">The type tag.</param>
        public CustomOscSerialiserAttribute(Type type)
        {
            TypeTag = ' ';
            Type = type;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomOscSerialiserAttribute" /> class.
        /// </summary>
        /// <param name="typeTag">The type tag.</param>
        public CustomOscSerialiserAttribute(char typeTag)
        {
            TypeTag = typeTag;
            Type = null;
        }

        /// <summary>
        ///     Gets the type tag.
        /// </summary>
        /// <value>
        ///     The type tag.
        /// </value>
        public char TypeTag { get; private set; }

        /// <summary>
        ///     Gets the type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        public Type Type { get; private set; }
    }
}