namespace eDrive.OSC;

/// <summary>
///     Provides global configuration settings for OSC session behavior and debugging capabilities.
///     These settings affect all OSC operations within the application session.
/// </summary>
/// <remarks>
///     <para>
///     This class contains static properties that control global OSC behavior, particularly
///     useful for debugging, development, and troubleshooting OSC communication issues.
///     </para>
///     <para>
///     Settings are application-wide and persist for the lifetime of the application domain.
///     Consider the thread-safety implications when modifying these settings in multi-threaded scenarios.
///     </para>
/// </remarks>
public class OscSessionSettings
{
    /// <summary>
    ///     Gets or sets a value indicating whether detailed debugging information should be dumped
    ///     during OSC packet processing and serialization operations.
    /// </summary>
    /// <value>
    ///     <c>true</c> to enable verbose debug output including packet contents, serialization steps,
    ///     and processing details; <c>false</c> to disable debug output for normal operation.
    /// </value>
    /// <remarks>
    ///     <para>
    ///     Enabling debug dumping can significantly help during development and troubleshooting
    ///     by providing detailed information about OSC packet structure, type conversions,
    ///     and processing flow.
    ///     </para>
    ///     <para>
    ///     <strong>Performance Impact:</strong> Debug output can impact performance and should
    ///     typically be disabled in production environments. The debug information is typically
    ///     written to the system debug output or console.
    ///     </para>
    /// </remarks>
    public static bool DebugDump { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether parsing exceptions should be thrown immediately
    ///     when malformed or invalid OSC data is encountered.
    /// </summary>
    /// <value>
    ///     <c>true</c> to throw exceptions immediately when parsing errors occur, providing
    ///     detailed error information for debugging; <c>false</c> to handle parsing errors
    ///     gracefully and continue processing when possible.
    /// </value>
    /// <remarks>
    ///     <para>
    ///     This setting controls the error handling strategy during OSC packet deserialization.
    ///     When enabled, any malformed data, invalid type tags, or structural problems will
    ///     result in immediate exceptions with detailed error information.
    ///     </para>
    ///     <para>
    ///     <strong>Development vs. Production:</strong> Enable this during development to catch
    ///     data format issues early. In production, consider disabling it to provide more
    ///     resilient operation when dealing with potentially unreliable network sources.
    ///     </para>
    ///     <para>
    ///     When disabled, the library will attempt to continue processing and may use default
    ///     values or skip problematic sections of the data stream.
    ///     </para>
    /// </remarks>
    public static bool RaiseParsingExceptions { get; set; }
}