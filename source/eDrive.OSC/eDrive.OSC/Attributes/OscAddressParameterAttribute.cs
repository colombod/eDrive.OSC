#region

using System;

#endregion

namespace eDrive.Osc.Attributes
{
    /// <summary>
    ///     User this attribute to mark a property as part of the Osc Address if the address uses the parameter in its pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OscAddressParameterAttribute : Attribute
    {
        private readonly string m_parameter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscAddressParameterAttribute" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public OscAddressParameterAttribute(string parameter)
        {
            m_parameter = parameter;
        }

        /// <summary>
        ///     Gets the parameter.
        /// </summary>
        /// <value>
        ///     The parameter.
        /// </value>
        public string Parameter
        {
            get { return m_parameter; }
        }
    }
}