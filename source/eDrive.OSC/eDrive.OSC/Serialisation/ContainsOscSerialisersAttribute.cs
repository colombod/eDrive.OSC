using System;

namespace eDrive.OSC.Serialisation;

/// <summary>
///     Marks the assembly as containing custom osc serializers
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
public sealed class ContainsOscSerializersAttribute : Attribute
{
}