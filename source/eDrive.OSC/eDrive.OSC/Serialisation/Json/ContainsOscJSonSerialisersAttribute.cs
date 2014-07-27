using System;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Marks the assembly as containing custom osc serialisers
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
	public sealed class ContainsOscJsonSerialisersAttribute : Attribute
    {
    }
}