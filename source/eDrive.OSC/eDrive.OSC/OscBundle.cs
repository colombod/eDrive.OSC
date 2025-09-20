using eDrive.OSC.Serialisation;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace eDrive.OSC;

/// <summary>
///     Represents an OSC bundle that groups multiple <see cref="OscMessage"/> objects and nested 
///     <see cref="OscBundle"/> objects together with precise timing information.
/// </summary>
/// <remarks>
///     <para>
///     OSC bundles enable synchronized execution of multiple operations by associating them
///     with a specific time tag. This is essential for musical applications where precise
///     timing coordination between multiple messages is required.
///     </para>
///     <para>
///     Bundles can contain:
///     <list type="bullet">
///         <item><description>OSC messages with their arguments</description></item>
///         <item><description>Other OSC bundles (nested timing groups)</description></item>
///     </list>
///     </para>
///     <para>
///     Example usage:
///     <code>
///     // Create a bundle for synchronized execution
///     var bundle = new OscBundle(OscTimeTag.Now.AddSeconds(1));
///     bundle.Add(new OscMessage("/synth/note", 60, 0.8f));
///     bundle.Add(new OscMessage("/synth/filter", 1000.0f));
///     
///     // All messages will execute at the specified time
///     </code>
///     </para>
/// </remarks>
public sealed class OscBundle : OscPacket
{
    private const string BundlePrefix = "#bundle";

    /// <summary>
    ///     Initializes a new OSC bundle with immediate execution timing.
    ///     The bundle will be executed as soon as it's received by the target.
    /// </summary>
    /// <remarks>
    ///     This constructor creates a bundle with an "immediate" time tag, meaning
    ///     all contained messages should be processed as quickly as possible upon receipt.
    ///     Use the <see cref="OscBundle(OscTimeTag)"/> constructor for scheduled execution.
    /// </remarks>
    public OscBundle()
        : this(new OscTimeTag())
    {
    }

    /// <summary>
    ///     Initializes a new OSC bundle with the specified execution time.
    /// </summary>
    /// <param name="timeStamp">
    ///     The precise time when this bundle should be executed by the receiving OSC server.
    ///     Use <see cref="OscTimeTag.Now"/> for immediate execution, or create a future time
    ///     for scheduled/synchronized execution.
    /// </param>
    /// <remarks>
    ///     The time stamp enables precise timing control essential for musical and real-time
    ///     applications. All messages and nested bundles within this bundle will be associated
    ///     with this timing information for coordinated execution.
    /// </remarks>
    public OscBundle(OscTimeTag timeStamp)
        : base(BundlePrefix)
    {
        TimeStamp = timeStamp;
    }

    /// <summary>
    ///     Gets a value indicating whether this packet represents an OSC bundle.
    ///     Always returns <c>true</c> for bundles, as opposed to <see cref="OscMessage"/> objects.
    /// </summary>
    /// <value>Always <c>true</c> for <see cref="OscBundle"/> instances.</value>
    /// <remarks>
    ///     This property enables polymorphic handling of <see cref="OscPacket"/> objects
    ///     where you need to distinguish between individual messages and bundles containing
    ///     multiple messages or nested bundles.
    /// </remarks>
    public override bool IsBundle => true;

    /// <summary>
    ///     Gets the time tag specifying when this bundle should be executed.
    /// </summary>
    /// <value>
    ///     An <see cref="OscTimeTag"/> representing the precise execution time.
    ///     May be "immediate" for as-soon-as-possible execution, or a future time for scheduling.
    /// </value>
    /// <remarks>
    ///     <para>
    ///     The time stamp is crucial for synchronized execution of multiple OSC operations.
    ///     OSC servers typically queue bundles and execute them at the specified time,
    ///     enabling sample-accurate timing for audio and control applications.
    ///     </para>
    ///     <para>
    ///     If the time stamp is in the past when received, the bundle is typically
    ///     executed immediately by most OSC implementations.
    ///     </para>
    /// </remarks>
    public OscTimeTag TimeStamp { get; private set; }

    /// <summary>
    ///     Gets all nested OSC bundles contained within this bundle.
    /// </summary>
    /// <value>
    ///     A read-only list of <see cref="OscBundle"/> objects that are nested within this bundle.
    ///     Empty if this bundle contains no nested bundles.
    /// </value>
    /// <remarks>
    ///     <para>
    ///     Nested bundles allow for hierarchical timing structures where groups of operations
    ///     can have their own timing contexts while still being part of a larger timed sequence.
    ///     </para>
    ///     <para>
    ///     Each nested bundle maintains its own time stamp, which should typically be the same
    ///     or later than the parent bundle's time stamp for logical execution order.
    ///     </para>
    /// </remarks>
    public IList<OscBundle> Bundles
    {
        get
        {
            EnsureDataLoaded();

            var bundles = m_data.OfType<OscBundle>().ToList();

            return bundles;
        }
    }

    /// <summary>
    ///     Gets all OSC messages contained directly within this bundle (not including nested bundle contents).
    /// </summary>
    /// <value>
    ///     A read-only list of <see cref="OscMessage"/> objects contained in this bundle.
    ///     Empty if this bundle contains no direct messages.
    /// </value>
    /// <remarks>
    ///     <para>
    ///     This property returns only the messages that are direct children of this bundle.
    ///     Messages contained in nested bundles are not included - access those through
    ///     the <see cref="Bundles"/> property.
    ///     </para>
    ///     <para>
    ///     All returned messages will be executed at this bundle's <see cref="TimeStamp"/>
    ///     when processed by an OSC server.
    ///     </para>
    /// </remarks>
    public IList<OscMessage> Messages
    {
        get
        {
            EnsureDataLoaded();

            var messages = m_data.OfType<OscMessage>().ToList();

            return messages;
        }
    }

    /// <summary>
    ///     Serialize the packet.
    /// </summary>
    /// <returns>The newly serialized packet.</returns>
    public override byte[] ToByteArray()
    {
        return NewSerializer();
    }

    private byte[] NewSerializer()
    {
        using (var s = new MemoryStream())
        {
            Write(s);
            s.Flush();
            return s.ToArray();
        }
    }


    /// <summary>
    ///     Serialises to the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns></returns>
    public override int Write(Stream stream)
    {
        var size = 0;
        size += SerializerFactory.StringSerializer.Encode(stream, Address);
        size += SerializerFactory.TimeTagSerializer.Encode(stream, TimeStamp);

        if (m_dataBag != null)
        {
            size += m_dataBag.Bytes.Length;
            stream.Write(m_dataBag.Bytes, 0, m_dataBag.Bytes.Length);
        }
        else
        {
            foreach (
                var packetBytes in
                m_data.OfType<OscPacket>().Select(packet => packet.ToByteArray()))
            {
                System.Diagnostics.Debug.Assert(packetBytes.Length % 4 == 0);

                SerializerFactory.IntSerializer.Encode(stream, packetBytes.Length);

                stream.Write(packetBytes, 0, packetBytes.Length);

                size += (sizeof(int) + packetBytes.Length);
            }
        }

        return size;
    }

    /// <summary>
    ///     Deserialize the packet.
    /// </summary>
    /// <param name="sourceEndPoint">The packet origin.</param>
    /// <param name="data">The serialized packet.</param>
    /// <param name="start">The starting index into the serialized data stream.</param>
    /// <param name="end">The ending index into the serialized data stream.</param>
    /// <returns>The newly deserialized packet.</returns>
    public static OscBundle BundleFromByteArray(byte[] data, ref int start, int end)
    {
        return Deserialise(data, ref start, end);
    }

    private static OscBundle Deserialise(byte[] data, ref int start, int end)
    {
        var str = SerializerFactory.StringSerializer;
        var tt = SerializerFactory.TimeTagSerializer;

        var prefix = str.Decode(data, start, out start);
        Assert.IsTrue(prefix == BundlePrefix);
        var timeTag = tt.Decode(data, start, out start);

        var bundle = new OscBundle(timeTag);

        if (end - start > 0)
        {
            bundle.m_dataBag = new DataBag
            {
                Bytes = data.CopySubArray(start, end - start)
            };

            start += bundle.m_dataBag.Bytes.Length;
        }
        else
        {
            bundle.m_dataBag = new DataBag
            {
                Bytes = new byte[0]
            };
        }

        return bundle;
    }


    /// <summary>
    ///     Ensures that the data is loaded.
    /// </summary>
    protected override void EnsureDataLoaded()
    {
        if (m_dataBag != null)
        {
            var start = 0;
            var end = m_dataBag.Bytes.Length;
            var data = m_dataBag.Bytes;

            m_dataBag = null;

            while (start < end)
            {
                var length = SerializerFactory.IntSerializer.Decode(data, start, out start);
                var packetEnd = start + length;
                Append(FromByteArray(data, ref start, packetEnd));
            }
        }
    }

    /// <summary>
    ///     Appends a value to the packet.
    /// </summary>
    /// <typeparam name="T">The type of object being appended.</typeparam>
    /// <param name="value">The value to append.</param>
    /// <returns>The index of the newly added value within the Data property.</returns>
    /// <remarks>The value must be of type OscPacket.</remarks>
    public override int Append<T>(T value)
    {
        EnsureDataLoaded();
        Assert.IsTrue(value is OscPacket);

        if (value is OscBundle nestedBundle)
        {
            Assert.IsTrue(nestedBundle.TimeStamp.DateTime >= TimeStamp.DateTime);
        }

        m_data.Add(value);

        return m_data.Count - 1;
    }
}