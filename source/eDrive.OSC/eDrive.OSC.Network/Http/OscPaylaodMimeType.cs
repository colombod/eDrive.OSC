namespace eDrive.OSC.Network.Http;

/// <summary>
///     Provides standard MIME type definitions for OSC payload formats used in HTTP transport.
///     This class defines the content types for both binary OSC data and JSON-serialized OSC data.
/// </summary>
/// <remarks>
///     <para>
///     When transmitting OSC data over HTTP, proper MIME types ensure that web servers,
///     proxies, and clients handle the content correctly. This class provides the standard
///     MIME types for the two primary OSC serialization formats.
///     </para>
///     <para>
///     The two supported formats are:
///     <list type="bullet">
///         <item><description><see cref="Osc"/>: Binary OSC format for maximum efficiency</description></item>
///         <item><description><see cref="Json"/>: JSON format for web application integration</description></item>
///     </list>
///     </para>
/// </remarks>
public class OscPaylaodMimeType
{
    static OscPaylaodMimeType()
    {
        Json = new OscPaylaodMimeType("application/json; charset=utf-8");
        Osc = new OscPaylaodMimeType("application/octet-stream");
    }

    private OscPaylaodMimeType(string type)
    {
        Type = type;
    }

    /// <summary>
    ///     Gets the MIME type for JSON-serialized OSC data suitable for web applications and REST APIs.
    /// </summary>
    /// <value>
    ///     The MIME type "application/json; charset=utf-8" for JSON-formatted OSC content.
    /// </value>
    /// <remarks>
    ///     Use this MIME type when transmitting OSC data in JSON format over HTTP.
    ///     JSON format provides human-readable data that's easily consumable by web browsers,
    ///     JavaScript applications, and REST clients, though it's less efficient than binary format.
    /// </remarks>
    public static OscPaylaodMimeType Json { get; private set; }
    /// <summary>
    ///     Gets the MIME type for binary OSC data in its native serialized format.
    /// </summary>
    /// <value>
    ///     The MIME type "application/octet-stream" for binary OSC content.
    /// </value>
    /// <remarks>
    ///     Use this MIME type when transmitting OSC data in its native binary format over HTTP.
    ///     Binary format provides maximum efficiency and preserves all OSC type information,
    ///     making it ideal for high-performance applications and direct OSC client communication.
    /// </remarks>
    public static OscPaylaodMimeType Osc { get; private set; }

    /// <summary>
    ///     Gets the MIME type string for this OSC payload format.
    /// </summary>
    /// <value>
    ///     The complete MIME type string including any parameters (e.g., charset for JSON).
    /// </value>
    /// <remarks>
    ///     This property provides the ready-to-use MIME type string for HTTP headers,
    ///     typically used in Content-Type or Accept headers when transmitting OSC data over HTTP.
    /// </remarks>
    public string Type { get; private set; }
}