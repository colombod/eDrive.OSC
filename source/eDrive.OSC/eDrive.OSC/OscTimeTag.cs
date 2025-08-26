using System;

namespace eDrive.OSC;

/// <summary>
///     Represents an Osc Time Tag.
/// </summary>
public class OscTimeTag : IEquatable<OscTimeTag>
{
    /// <summary>
    ///     The immediate
    /// </summary>
    public const ulong Immediate = 1L;

    /// <summary>
    ///     The D T1900 offset
    /// </summary>
    private static readonly DateTime s_dt1900 = new DateTime(0x76c, 1, 1);

    private readonly DateTime m_dateTime;
    private readonly ulong m_timeTag;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OscTimeTag" /> class.
    /// </summary>
    public OscTimeTag()
    {
        m_timeTag = Immediate;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OscTimeTag" /> class.
    /// </summary>
    /// <param name="tag">The tag.</param>
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
    ///     Initializes a new instance of the <see cref="OscTimeTag" /> class.
    /// </summary>
    /// <param name="tag">The tag.</param>
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
    ///     Gets the date time.
    /// </summary>
    /// <value>
    ///     The date time.
    /// </value>
    public DateTime DateTime
    {
        get { return m_dateTime; }
    }

    /// <summary>
    ///     Gets the time tag.
    /// </summary>
    /// <value>
    ///     The time tag.
    /// </value>
    public ulong TimeTag
    {
        get { return m_timeTag; }
    }

    /// <summary>
    ///     Gets a value indicating whether this instance is an "immediate" value.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is an "immediate" value; otherwise, <c>false</c>.
    /// </value>
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