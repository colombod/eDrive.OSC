using System;

namespace eDrive.OSC.Attributes
{
    /// <summary>
    ///     Use this attribute on properties to specify the position urs value should have in the body of an Osc message
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OscBodyPartAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OscBodyPartAttribute" /> class.
        /// </summary>
        /// <param name="position">The position.</param>
        public OscBodyPartAttribute(int position)
        {
            Position = position;
        }

        /// <summary>
        ///     Gets the position.
        /// </summary>
        /// <value>
        ///     The position.
        /// </value>
        public int Position { get; private set; }
    }
}