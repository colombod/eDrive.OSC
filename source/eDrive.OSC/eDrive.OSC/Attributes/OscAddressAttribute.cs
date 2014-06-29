#region

using System;

#endregion

namespace eDrive.Osc.Attributes
{
    /// <summary>
    ///     Use this attribute to bind a class to an OSC address pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class OscAddressAttribute : Attribute
    {
        private readonly string m_address;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscAddressAttribute" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        public OscAddressAttribute(string address)
        {
            m_address = address;
            var parts = Address.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            var depth = parts.Length;
            Depth = depth;
        }

        /// <summary>
        ///     Gets the depth.
        /// </summary>
        /// <value>
        ///     The depth.
        /// </value>
        public int Depth { get; private set; }

        /// <summary>
        ///     Gets the address.
        /// </summary>
        /// <value>
        ///     The address.
        /// </value>
        public string Address
        {
            get { return m_address; }
        }
    }
}