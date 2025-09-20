using System.Runtime.InteropServices;

namespace eDrive.OSC;

/// <summary>
///     Represents an OSC color value with red, green, blue, and alpha components.
///     OSC colors are 32-bit values that can be transmitted efficiently in OSC messages
///     and are particularly useful for lighting control, visual applications, and UI parameters.
/// </summary>
/// <remarks>
///     <para>
///     OSC colors use the standard RGBA format where each component (Red, Green, Blue, Alpha)
///     is represented as an 8-bit value (0-255). The color can be constructed from individual
///     components or from a packed 32-bit integer value.
///     </para>
///     <para>
///     This type is commonly used in:
///     <list type="bullet">
///         <item><description>Lighting control systems (DMX, LED arrays)</description></item>
///         <item><description>Visual programming environments</description></item>
///         <item><description>UI color parameter control</description></item>
///         <item><description>Computer graphics applications</description></item>
///     </list>
///     </para>
///     <para>
///     Example usage:
///     <code>
///     // Create a red color with full opacity
///     var red = new OscColour(255, 0, 0, 255);
///     
///     // Create from packed integer (0xRRGGBBAA format)
///     var blue = new OscColour(0x0000FFFF);
///     
///     // Use in OSC message
///     var msg = new OscMessage("/light/color", red);
///     </code>
///     </para>
/// </remarks>
public class OscColour
{
    /// <summary>
    ///     Initializes a new OSC color from individual red, green, blue, and alpha components.
    /// </summary>
    /// <param name="r">The red component intensity (0-255).</param>
    /// <param name="g">The green component intensity (0-255).</param>
    /// <param name="b">The blue component intensity (0-255).</param>
    /// <param name="a">The alpha (transparency) component (0-255, where 255 is fully opaque).</param>
    /// <remarks>
    ///     This constructor provides the most intuitive way to create colors by specifying
    ///     each component individually. Values outside the 0-255 range will be clamped
    ///     to fit within the byte range during assignment.
    /// </remarks>
    public OscColour(byte r, byte g, byte b, byte a)
    {
        m_storage.A = a;
        m_storage.R = r;
        m_storage.G = g;
        m_storage.B = b;
    }

    /// <summary>
    ///     Initializes a new OSC color from a packed 32-bit integer representation.
    /// </summary>
    /// <param name="message">
    ///     A 32-bit integer containing the packed color data in RGBA format.
    ///     The format is typically 0xRRGGBBAA where RR=red, GG=green, BB=blue, AA=alpha.
    /// </param>
    /// <remarks>
    ///     This constructor is useful when receiving color data from external sources,
    ///     deserializing from binary formats, or when working with APIs that use
    ///     packed color representations. The bit layout depends on the system's endianness.
    /// </remarks>
    public OscColour(int message)
    {
        m_storage.Value = message;
    }

    /// <summary>
    ///     Gets or sets the alpha (transparency) component of the color.
    /// </summary>
    /// <value>
    ///     An 8-bit value (0-255) representing the alpha channel, where 0 is fully transparent
    ///     and 255 is fully opaque.
    /// </value>
    /// <remarks>
    ///     The alpha component controls the transparency of the color. This is particularly
    ///     important for lighting applications where you might want to fade colors in and out,
    ///     or for UI applications where semi-transparent overlays are needed.
    /// </remarks>
    public byte A
    {
        get => m_storage.A;
        set => m_storage.A = value;
    }

    /// <summary>
    ///     Gets or sets the blue component of the color.
    /// </summary>
    /// <value>
    ///     An 8-bit value (0-255) representing the intensity of the blue component,
    ///     where 0 is no blue and 255 is maximum blue intensity.
    /// </value>
    public byte B
    {
        get => m_storage.B;
        set => m_storage.B = value;
    }

    /// <summary>
    ///     Gets or sets the green component of the color.
    /// </summary>
    /// <value>
    ///     An 8-bit value (0-255) representing the intensity of the green component,
    ///     where 0 is no green and 255 is maximum green intensity.
    /// </value>
    public byte G
    {
        get => m_storage.G;
        set => m_storage.G = value;
    }

    /// <summary>
    ///     Gets or sets the red component of the color.
    /// </summary>
    /// <value>
    ///     An 8-bit value (0-255) representing the intensity of the red component,
    ///     where 0 is no red and 255 is maximum red intensity.
    /// </value>
    public byte R
    {
        get => m_storage.R;
        set => m_storage.R = value;
    }

    /// <summary>
    ///     Converts this OSC color to its packed 32-bit integer representation.
    /// </summary>
    /// <returns>
    ///     A 32-bit signed integer containing the packed RGBA color data.
    /// </returns>
    /// <remarks>
    ///     This method provides the color in a format suitable for serialization,
    ///     network transmission, or interoperability with systems that expect
    ///     packed color values. The exact bit layout depends on the system's endianness.
    /// </remarks>
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