using System;

namespace eDrive.OSC;

/// <summary>
///     Represents an OSC time tag for precise timing control in OSC bundles and scheduled operations.
///     OSC time tags enable sample-accurate synchronization essential for musical and real-time applications.
/// </summary>
/// <remarks>
///     <para>
///     OSC time tags use a 64-bit fixed-point time format representing seconds since January 1, 1900 UTC.
///     The upper 32 bits represent whole seconds, while the lower 32 bits represent fractional seconds,
///     providing sub-microsecond precision.
///     </para>
///     <para>
///     Special values:
///     <list type="bullet">
///         <item><description><see cref="Immediate"/> (1): Execute as soon as possible</description></item>
///         <item><description>Future times: Schedule for precise execution</description></item>
///     </list>
///     </para>
///     <para>
///     Example usage:
///     <code>
///     // Immediate execution
///     var immediate = new OscTimeTag();
///     
///     // Execute in 1 second
///     var future = new OscTimeTag(DateTime.Now.AddSeconds(1));
///     
///     // Current time for scheduling
///     var now = OscTimeTag.Now;
///     </code>
///     </para>
/// </remarks>
public class OscTimeTag : IEquatable<OscTimeTag>
{
    /// <summary>
    ///     The special "immediate" time tag value that instructs OSC servers to execute bundles 
    ///     as quickly as possible without scheduling delays.
    /// </summary>
    /// <remarks>
    ///     This constant represents the OSC specification's reserved value for immediate execution.
    ///     When a bundle has this time tag, receiving OSC servers should process it immediately
    ///     rather than queuing it for future execution.
    /// </remarks>
    public const ulong Immediate = 1L;

    /// <summary>
    ///     The OSC epoch reference point (January 1, 1900 UTC) used for time tag calculations.
    ///     This differs from the Unix epoch and follows the OSC specification's time base.
    /// </summary>
    private static readonly DateTime s_dt1900 = new DateTime(0x76c, 1, 1);

    private readonly DateTime m_dateTime;
    private readonly ulong m_timeTag;

    /// <summary>
    ///     Initializes a new "immediate" time tag for as-soon-as-possible execution.
    /// </summary>
    /// <remarks>
    ///     This constructor creates a time tag with the special <see cref="Immediate"/> value,
    ///     instructing OSC servers to execute associated bundles without scheduling delays.
    ///     This is equivalent to creating a new OscTimeTag with <see cref="Immediate"/> as the parameter.
    /// </remarks>
    public OscTimeTag()
    {
        m_timeTag = Immediate;
    }

    /// <summary>
    ///     Initializes a new OSC time tag from a .NET DateTime for scheduled execution.
    /// </summary>
    /// <param name="tag">
    ///     The target execution time as a .NET DateTime. Times in the past or DateTime.MinValue
    ///     will be treated as "immediate" execution.
    /// </param>
    /// <remarks>
    ///     This constructor converts standard .NET DateTime values into OSC time tags.
    ///     The conversion accounts for the different epoch bases (OSC uses 1900, .NET uses 1601/1970).
    ///     Use this when you want to schedule bundle execution at a specific wall-clock time.
    /// </remarks>
    public OscTimeTag(DateTime tag)
    {
        if (tag > DateTime.MinValue)
        {
            m_dateTime = tag;
            m_timeTag = ToTimeStamp(DateTime);
        }
        else
        {
            m_dateTime = DateTime.MinValue;
            m_timeTag = Immediate;
        }
    }

    /// <summary>
    ///     Initializes a new OSC time tag from a raw 64-bit OSC time tag value.
    /// </summary>
    /// <param name="tag">
    ///     The raw OSC time tag as a 64-bit unsigned integer. Values of <see cref="Immediate"/> (1)
    ///     or less are treated as immediate execution. Higher values represent seconds since 1900.
    /// </param>
    /// <remarks>
    ///     This constructor is useful when working with raw OSC data or when you need to recreate
    ///     a time tag from serialized data. The 64-bit format provides sub-microsecond precision
    ///     with 32 bits for seconds and 32 bits for fractional seconds.
    /// </remarks>
    public OscTimeTag(ulong tag)
    {
        if (tag > Immediate)
        {
            m_timeTag = tag;
            m_dateTime = ToDateTime(TimeTag);
        }
        else
        {
            m_dateTime = DateTime.MinValue;
            m_timeTag = Immediate;
        }
    }

    /// <summary>
    ///     Gets the time tag value as a .NET DateTime for easy integration with .NET timing APIs.
    /// </summary>
    /// <value>
    ///     The DateTime representation of this time tag, or DateTime.MinValue for immediate execution.
    /// </value>
    /// <remarks>
    ///     This property provides convenient access to the time tag in familiar .NET DateTime format.
    ///     Note that DateTime.MinValue indicates "immediate" execution rather than an actual time.
    ///     Use <see cref="IsImmediate"/> to check for the special immediate case.
    /// </remarks>
    public DateTime DateTime
    {
        get { return m_dateTime; }
    }

    /// <summary>
    ///     Gets the raw OSC time tag value as a 64-bit unsigned integer.
    /// </summary>
    /// <value>
    ///     The time tag in OSC's native 64-bit format, where the upper 32 bits represent seconds
    ///     since 1900 and the lower 32 bits represent fractional seconds. Value of 1 indicates immediate execution.
    /// </value>
    /// <remarks>
    ///     This property provides access to the time tag in its native OSC binary format.
    ///     This is useful for serialization, network transmission, or when working with
    ///     OSC implementations that expect raw time tag values.
    /// </remarks>
    public ulong TimeTag
    {
        get { return m_timeTag; }
    }

    /// <summary>
    ///     Gets a value indicating whether this time tag represents "immediate" execution.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this time tag has the special "immediate" value, meaning bundles should be
    ///     executed as quickly as possible; <c>false</c> if this represents a scheduled future time.
    /// </value>
    /// <remarks>
    ///     The immediate flag is essential for determining how OSC servers should handle bundle execution.
    ///     Immediate bundles bypass scheduling and are processed as fast as possible, while
    ///     non-immediate bundles are queued for execution at their specified time.
    /// </remarks>
    public bool IsImmediate
    {
        get { return m_timeTag == Immediate; }
    }

    public bool Equals(OscTimeTag other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return m_dateTime.Equals(other.m_dateTime);
    }

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
        if (obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((OscTimeTag)obj);
    }

    public override int GetHashCode()
    {
        return m_dateTime.GetHashCode();
    }

    public static bool operator ==(OscTimeTag left, OscTimeTag right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(OscTimeTag left, OscTimeTag right)
    {
        return !Equals(left, right);
    }

    /// <summary>
    ///     To the date time.
    /// </summary>
    /// <param name="timeStamp">The time stamp.</param>
    /// <returns></returns>
    public static DateTime ToDateTime(ulong timeStamp)
    {
        if (timeStamp <= 1L)
        {
            return DateTime.MinValue;
        }
        var num = timeStamp >> 0x20;
        var num2 = timeStamp & 0xffffffffL;
        var num3 = (num * (0x989680L)) + ((num2 * (0x989680L)) / (0x100000000L));
        return (s_dt1900 + new TimeSpan((long)num3));
    }

    /// <summary>
    ///     To the time stamp.
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns></returns>
    public static ulong ToTimeStamp(DateTime dateTime)
    {
        if (dateTime < s_dt1900)
        {
            return 1L;
        }
        var span = (dateTime - s_dt1900);
        var ticks = (ulong)span.Ticks;
        var num2 = ticks / 0x989680L;
        var num3 = ((ticks % 0x989680L) * 0x100000000L) / 0x989680L;
        return ((num2 << 0x20) | num3);
    }
}