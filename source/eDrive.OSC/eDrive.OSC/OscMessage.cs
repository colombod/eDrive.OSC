using eDrive.OSC.Serialisation;

using System;
using System.Collections.Generic;
using System.IO;

namespace eDrive.OSC;

/// <summary>
///     Represents an OSC message packet that encapsulates a method call with typed arguments.
///     OSC messages are the primary means of communication in the OSC protocol, containing
///     an address pattern that identifies the target method and optional typed arguments.
/// </summary>
/// <remarks>
///     <para>
///     OSC messages follow the format: address + type_tags + arguments
///     </para>
///     <para>
///     Example usage:
///     <code>
///     // Create a message to control synthesizer frequency
///     var msg = new OscMessage("/synth/osc/1/frequency", 440.0f);
///     
///     // Create a message with multiple parameters
///     var msg = new OscMessage("/mixer/channel/1");
///     msg.Append(0.8f);  // volume
///     msg.Append(true);  // mute
///     </code>
///     </para>
/// </remarks>
public class OscMessage : OscPacket
{
    /// <summary>
    ///     The mandatory prefix for all OSC address patterns.
    ///     All valid OSC addresses must begin with this forward slash character.
    /// </summary>
    protected const string AddressPrefix = "/";

    private bool m_isEvent;
    private string m_typeTag;

    /// <summary>
    ///     Initializes a new OSC message with the specified address and a single argument.
    /// </summary>
    /// <param name="address">
    ///     The OSC address pattern (e.g., "/synth/frequency", "/mixer/volume").
    ///     Must start with "/" and follow OSC address conventions.
    /// </param>
    /// <param name="value">
    ///     The initial argument to include in the message. Can be any OSC-compatible type
    ///     such as int, float, string, bool, byte[], etc.
    /// </param>
    /// <remarks>
    ///     This constructor is convenient for creating messages with a single argument.
    ///     Additional arguments can be added using the <see cref="OscPacket.Append{T}"/> method.
    /// </remarks>
    public OscMessage(string address, object value)
        : this(address)
    {
        InitialiseWithValue(value);
    }

    /// <summary>
    ///     Initializes a new OSC message with the specified address and no arguments.
    /// </summary>
    /// <param name="address">
    ///     The OSC address pattern (e.g., "/start", "/stop", "/reset").
    ///     Must start with "/" as required by the OSC specification.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when the address doesn't start with "/".</exception>
    /// <remarks>
    ///     This creates an empty message that can have arguments added later using
    ///     <see cref="OscPacket.Append{T}"/>. Some OSC messages (like events or triggers)
    ///     don't require arguments and use only the address for identification.
    /// </remarks>
    public OscMessage(string address)
        : base(address)
    {
        Assert.IsTrue(address.StartsWith(AddressPrefix));

        m_typeTag = SerializerFactory.DefaultTag.ToString();
    }

    /// <summary>
    ///     Gets a value indicating whether this packet represents an OSC bundle.
    ///     Always returns <c>false</c> for messages, as opposed to <see cref="OscBundle"/> objects.
    /// </summary>
    /// <value>Always <c>false</c> for <see cref="OscMessage"/> instances.</value>
    /// <remarks>
    ///     This property enables polymorphic handling of <see cref="OscPacket"/> objects
    ///     where you need to distinguish between individual messages and bundles containing
    ///     multiple messages.
    /// </remarks>
    public override bool IsBundle
    {
        get { return false; }
    }

    /// <summary>
    ///     Gets the OSC type tag string that describes the argument types in this message.
    /// </summary>
    /// <value>
    ///     A string where each character represents the type of one argument.
    ///     Common type tags include: 'i' (int), 'f' (float), 's' (string), 'b' (blob), 'T'/'F' (bool).
    /// </value>
    /// <remarks>
    ///     <para>
    ///     The type tag string is automatically maintained as arguments are added to the message.
    ///     It's used during serialization to ensure proper encoding and during deserialization
    ///     to correctly interpret the binary data.
    ///     </para>
    ///     <para>
    ///     Example: A message with an int and a float would have TypeTag = ",if"
    ///     (the comma prefix is part of the OSC specification).
    ///     </para>
    /// </remarks>
    public string TypeTag
    {
        get { return m_typeTag; }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether this message represents an event (trigger) 
    ///     rather than a method call with return value expectations.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this message is an event/trigger that doesn't expect a response;
    ///     <c>false</c> if this message is a method call that may have a return value.
    /// </value>
    /// <remarks>
    ///     <para>
    ///     Event messages are fire-and-forget communications typically used for notifications,
    ///     triggers, or state changes where no response is expected. Setting this to true
    ///     affects the type tag string by appending an event marker.
    ///     </para>
    ///     <para>
    ///     This distinction helps receiving applications understand whether to process
    ///     the message as a command requiring response or as a notification.
    ///     </para>
    /// </remarks>
    public bool IsEvent
    {
        get { return m_isEvent; }
        set
        {
            m_isEvent = value;
            m_typeTag = m_typeTag.Replace(SerializerFactory.EventTagString, string.Empty);

            if (m_isEvent)
            {
                m_typeTag = m_typeTag + SerializerFactory.EventTagString;
            }
        }
    }

    private void InitialiseWithValue(object value)
    {
        Append(value);
    }

    /// <summary>
    ///     Serialize the packet.
    /// </summary>
    /// <returns>The newly serialized packet.</returns>
    public override byte[] ToByteArray()
    {
        return NewSerialisier();
    }

    private byte[] NewSerialisier()
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
    public override int Write(Stream stream)
    {
        var size = 0;
        var stringSer = SerializerFactory.GetSerializer<string>();
        size += stringSer.Encode(stream, Address);
        size += stringSer.Encode(stream, TypeTag);

        if (m_dataBag != null)
        {
            size += m_dataBag.Bytes.Length;
            stream.Write(m_dataBag.Bytes, 0, m_dataBag.Bytes.Length);
        }
        else
        {
            foreach (var part in m_data)
            {
                if (part is Array
                    && !(part is byte[])) // NB: blobs are handled with a specific serialisator.)
                {
                    var collection = part as Array;
                    foreach (var component in collection)
                    {
                        var ser = SerializerFactory.GetSerializer(component);
                        size += ser.Encode(stream, component);
                    }
                }
                else
                {
                    var ser = SerializerFactory.GetSerializer(part);
                    size += ser.Encode(stream, part);
                }
            }
        }

        return size;
    }


    /// <summary>
    ///     Deserialize the packet.
    /// </summary>
    /// <param name="data">The serialized packet.</param>
    /// <param name="start">The starting index into the serialized data stream.</param>
    /// <param name="end">The end.</param>
    /// <returns>
    ///     The newly deserialized packet.
    /// </returns>
    public static OscMessage MessageFromByteArray(byte[] data, ref int start, int end)
    {
        var sd = SerializerFactory.StringSerializer;

        var address = sd.Decode(data, start, out start);

        var message = new OscMessage(address);

        var tags = sd.Decode(data, start, out start);


        if (end - start > 0)
        {
            message.m_dataBag = new DataBag
            {
                Bytes = data.CopySubArray(start, end - start)
            };

            start += message.m_dataBag.Bytes.Length;
        }
        else
        {
            message.m_dataBag = new DataBag
            {
                Bytes = new byte[0]
            };
        }

        message.m_typeTag = tags;


        return message;
    }

    /// <summary>
    ///     Deserialize the packet.
    /// </summary>
    /// <param name="data">The serialized packet.</param>
    /// <param name="start">The starting index into the serialized data stream.</param>
    /// <returns>
    ///     The newly deserialized packet.
    /// </returns>
    public static OscMessage MessageFromByteArray(byte[] data, ref int start)
    {
        return MessageFromByteArray(data, ref start, data.Length);
    }

    private void DeserialiseFromData()
    {
        Deserialise();
    }

    private void Deserialise()
    {
        if (m_dataBag != null)
        {
            var start = 0;
            var data = m_dataBag.Bytes;
            var tags = m_typeTag;

            m_dataBag = null;

            for (var index = 0; index < tags.Length; index++)
            {
                var tag = tags[index];

                if (tag == SerializerFactory.ArrayOpen)
                {
                    // skip the '[' character.
                    index++;

                    // deserialise array of object
                    var ret = new List<object>();
                    while (tags[index] != SerializerFactory.ArrayClose
                           && index < tags.Length)
                    {
                        int pos;

                        var des = SerializerFactory.GetSerializer(tags[index]);
                        ret.Add(des.Decode(data, start, out pos));

                        start = pos;
                        index++;
                    }

                    m_data.Add(ret.ToArray());
                }
                else if (tag != SerializerFactory.DefaultTag)
                {
                    if (tag != SerializerFactory.EventTag)
                    {
                        int pos;

                        var des = SerializerFactory.GetSerializer(tag);
                        m_data.Add(des.Decode(data, start, out pos));

                        start = pos;
                    }
                    else
                    {
                        IsEvent = true;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Ensures that the data is loaded.
    /// </summary>
    protected override void EnsureDataLoaded()
    {
        if (m_dataBag != null)
        {
            DeserialiseFromData();
        }
    }

    /// <summary>
    ///     Appends a value to the message.
    /// </summary>
    /// <typeparam name="T">The type of object being appended.</typeparam>
    /// <param name="value">The value to append.</param>
    /// <returns>The index of the newly appended Data value.</returns>
    public override int Append<T>(T value)
    {
        EnsureDataLoaded();

        return NewAdd(value);
    }

    private int NewAdd<T>(T value)
    {
        var typeTag = GetTag(value);

        m_typeTag = TypeTag + typeTag;
        m_data.Add(value);

        return m_data.Count - 1;
    }

    private static string GetTag<T>(T value)
    {
        string typeTag;
        try
        {
            typeTag = SerializerFactory.GetTag(value);
        }
        catch (KeyNotFoundException e)
        {
            throw new OscSerializerException(value.GetType(), e);
        }

        return typeTag;
    }

    /// <summary>
    ///     Appends a Nil value to the message.
    /// </summary>
    /// <returns>The index of the newly appended Data value.</returns>
    public int AppendNil()
    {
        return Append<object>(null);
    }

    /// <summary>
    ///     Update a value within the message at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to update.</param>
    /// <param name="value">The value to update the element with.</param>
    public void UpdateDataAt<T>(int index, T value)
    {
        EnsureDataLoaded();

        if (m_data.Count == 0
            || m_data.Count <= index)
        {
            throw new ArgumentOutOfRangeException();
        }

        m_data[index] = value;

        RebuldTtypeTag();
    }

    private void RebuldTtypeTag()
    {
        m_typeTag = SerializerFactory.DefaultTag.ToString();
        foreach (var o in m_data)
        {
            m_typeTag += GetTag(o);
        }
    }

    /// <summary>
    ///     Remove all data from the message.
    /// </summary>
    public void ClearData()
    {
        m_dataBag = null;
        m_typeTag = SerializerFactory.DefaultTag.ToString();
        m_data.Clear();
    }
}