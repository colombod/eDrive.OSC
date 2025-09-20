using System;

namespace eDrive.OSC.Attributes;

/// <summary>
///     Specifies the position that a property's value should occupy in the argument list 
///     of an OSC message when using automatic serialization from .NET objects.
/// </summary>
/// <remarks>
///     <para>
///     This attribute enables automatic mapping between .NET object properties and OSC message arguments.
///     When serializing objects to OSC messages, properties marked with this attribute will be placed
///     at the specified position in the message's argument array.
///     </para>
///     <para>
///     Usage example:
///     <code>
///     public class SynthParameters
///     {
///         [OscBodyPart(0)]
///         public float Frequency { get; set; }
///         
///         [OscBodyPart(1)]
///         public float Amplitude { get; set; }
///         
///         [OscBodyPart(2)]
///         public bool Gate { get; set; }
///     }
///     
///     // Results in OSC message: "/synth" frequency amplitude gate
///     </code>
///     </para>
///     <para>
///     This is particularly useful for creating strongly-typed representations of OSC messages
///     while maintaining control over the argument order in the serialized format.
///     </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class OscBodyPartAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="OscBodyPartAttribute" /> with the specified position.
    /// </summary>
    /// <param name="position">
    ///     The zero-based position where this property's value should appear in the OSC message argument list.
    ///     Must be non-negative and should be unique within the same class.
    /// </param>
    /// <remarks>
    ///     Positions should typically start at 0 and be consecutive for predictable serialization order.
    ///     Gaps in position numbers may result in null or default values being inserted in the argument list.
    /// </remarks>
    public OscBodyPartAttribute(int position)
    {
        Position = position;
    }

    /// <summary>
    ///     Gets the zero-based position where this property's value will appear in the OSC message argument list.
    /// </summary>
    /// <value>
    ///     The position index (0-based) for this property in the serialized OSC message arguments.
    /// </value>
    /// <remarks>
    ///     This position determines the order of arguments when the containing object is
    ///     automatically serialized to an OSC message. Lower position numbers appear
    ///     earlier in the argument list.
    /// </remarks>
    public int Position { get; private set; }
}