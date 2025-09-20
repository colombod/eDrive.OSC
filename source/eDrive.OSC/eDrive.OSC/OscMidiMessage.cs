using System;

namespace eDrive.OSC;

/// <summary>
///     Represents a MIDI message that can be transmitted through OSC packets for musical device control.
///     This class encapsulates standard MIDI protocol data in a format suitable for OSC transmission.
/// </summary>
/// <remarks>
///     <para>
///     OSC MIDI messages follow the standard MIDI protocol format but are encapsulated for transmission
///     over OSC networks. This enables remote control of MIDI devices, software synthesizers, and
///     Digital Audio Workstations (DAWs) through OSC communication.
///     </para>
///     <para>
///     A MIDI message consists of:
///     <list type="bullet">
///         <item><description><strong>Port ID:</strong> Identifies the MIDI port/channel (0-15)</description></item>
///         <item><description><strong>Status Byte:</strong> Defines the message type (Note On/Off, Control Change, etc.)</description></item>
///         <item><description><strong>Data Byte 1:</strong> First parameter (note number, controller number, etc.)</description></item>
///         <item><description><strong>Data Byte 2:</strong> Second parameter (velocity, controller value, etc.)</description></item>
///     </list>
///     </para>
///     <para>
///     Common usage examples:
///     <code>
///     // Note On message: Channel 1, Middle C (60), Velocity 100
///     var noteOn = new OscMidiMessage(0, 0x90, 60, 100);
///     
///     // Control Change: Channel 1, Modulation Wheel (1), Value 64
///     var modWheel = new OscMidiMessage(0, 0xB0, 1, 64);
///     
///     // Send via OSC
///     var oscMsg = new OscMessage("/midi", noteOn);
///     </code>
///     </para>
/// </remarks>
public class OscMidiMessage
{
    private byte m_portId;
    private byte m_status;
    private byte m_data1;
    private byte m_data2;
    private int m_value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OscMidiMessage" /> class.
    /// </summary>
    /// <param name="portId">The port id.</param>
    /// <param name="status">The status.</param>
    /// <param name="data1">The data1.</param>
    /// <param name="data2">The data2.</param>
    public OscMidiMessage(byte portId, byte status, byte data1, byte data2)
    {
        m_portId = portId;
        m_status = status;
        m_data1 = data1;
        m_data2 = data2;
        UpdateValue();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OscMidiMessage" /> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public OscMidiMessage(int message)
    {
        m_value = message;
        UpdateParts();
    }

    /// <summary>
    ///     Gets or sets the port id.
    /// </summary>
    /// <value>
    ///     The port id.
    /// </value>
    public byte PortId
    {
        get { return m_portId; }
        set
        {
            m_portId = value;
            UpdateValue();
        }
    }

    /// <summary>
    ///     Gets or sets the status.
    /// </summary>
    /// <value>
    ///     The status.
    /// </value>
    public byte Status
    {
        get { return m_status; }
        set
        {
            m_status = value;
            UpdateValue();
        }
    }

    /// <summary>
    ///     Gets or sets the data1.
    /// </summary>
    /// <value>
    ///     The data1.
    /// </value>
    public byte Data1
    {
        get { return m_data1; }
        set
        {
            m_data1 = value;
            UpdateValue();
        }
    }

    /// <summary>
    ///     Gets or sets the data2.
    /// </summary>
    /// <value>
    ///     The data2.
    /// </value>
    public byte Data2
    {
        get { return m_data2; }
        set
        {
            m_data2 = value;
            UpdateValue();
        }
    }

    /// <summary>
    ///     The message as int 32
    /// </summary>
    /// <returns></returns>
    public int ToInt32()
    {
        return m_value;
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
        return obj.GetType() == GetType() && Equals((OscMidiMessage)obj);
    }

    public override int GetHashCode()
    {
        return m_value;
    }

    protected bool Equals(OscMidiMessage other)
    {
        return m_value == other.m_value;
    }
    private void UpdateValue()
    {
        m_value = BitConverter.ToInt32(new[] { m_data2, m_data1, m_status, m_portId }, 0);
    }

    private void UpdateParts()
    {
        var parts = BitConverter.GetBytes(m_value);
        m_portId = parts[3];
        m_status = parts[2];
        m_data1 = parts[1];
        m_data2 = parts[0];
    }


}