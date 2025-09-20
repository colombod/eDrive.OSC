using System;
using System.Collections.Generic;
using System.IO;

namespace eDrive.OSC;

/// <summary>
///     Represents the base unit of transmission for the Open Sound Control (OSC) protocol.
///     This is the abstract base class for all OSC data packets, which can be either 
///     <see cref="OscMessage"/> objects containing addressable method calls with arguments,
///     or <see cref="OscBundle"/> objects containing multiple packets with timing information.
/// </summary>
/// <remarks>
///     OSC packets form the foundation of OSC communication. They encapsulate data that can be
///     serialized to binary format for network transmission or JSON format for web applications.
///     All packets have an address pattern and can contain typed arguments.
/// </remarks>
public abstract class OscPacket
{
    /// <summary>
    ///     Contains the actual data payload of the OSC packet.
    ///     This list stores the arguments/content in their original .NET types
    ///     before serialization or after deserialization.
    /// </summary>
    protected List<object> m_data;

    /// <summary>
    ///     Stores the raw binary data when the packet is in serialized form.
    ///     This enables lazy deserialization - the packet can hold binary data
    ///     until the actual content is accessed via the <see cref="Data"/> property.
    /// </summary>
    protected DataBag m_dataBag;

    /// <summary>
    ///     Initializes static configuration for the OSC packet system.
    ///     Sets the default byte order to big-endian as required by the OSC specification.
    /// </summary>
    static OscPacket()
    {
        LittleEndianByteOrder = false;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OscPacket" /> class with the specified OSC address.
    /// </summary>
    /// <param name="address">
    ///     The OSC address pattern that identifies this packet's destination or purpose.
    ///     Must follow OSC address pattern syntax (e.g., "/synth/frequency", "/mixer/channel/1/volume").
    ///     Cannot be null or empty.
    /// </param>
    /// <exception cref="ArgumentException">Thrown when address is null or empty.</exception>
    /// <remarks>
    ///     OSC addresses are hierarchical identifiers similar to URL paths, used to route
    ///     messages to specific handlers or organize related functionality.
    /// </remarks>
    protected OscPacket(string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            throw new ArgumentException("Cannot be null or empty", "address");
        }


        Address = address;
        m_data = new List<object>();
    }

    /// <summary>
    ///     Appends a typed value to this packet's argument list.
    ///     The value will be automatically serialized according to OSC type conventions.
    /// </summary>
    /// <typeparam name="T">The .NET type of the value being appended. Must be OSC-compatible.</typeparam>
    /// <param name="value">The value to append. Supported types include int, float, string, byte[], bool, etc.</param>
    /// <returns>The zero-based index where the value was inserted in the packet's data array.</returns>
    /// <remarks>
    ///     This method handles automatic type tag generation and ensures the value is properly
    ///     formatted for OSC transmission. Common types are automatically supported, while
    ///     custom types may require explicit serialization configuration.
    /// </remarks>
    public abstract int Append<T>(T value);

    /// <summary>
    ///     Retrieves a typed value from the packet's argument list at the specified index.
    /// </summary>
    /// <typeparam name="T">The expected .NET type of the value at the specified index.</typeparam>
    /// <param name="index">The zero-based index of the argument to retrieve.</param>
    /// <returns>The value at the specified index, cast to type T.</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown when the index is negative or exceeds the data array bounds.</exception>
    /// <exception cref="InvalidCastException">Thrown when the stored value cannot be cast to the requested type T.</exception>
    /// <remarks>
    ///     This provides type-safe access to packet arguments. The caller must know the expected
    ///     type at each index, or inspect the type information through other means before calling.
    ///     Use this method when you need specific type guarantees for the retrieved data.
    /// </remarks>
    public T At<T>(int index)
    {
        if (index > m_data.Count
            || index < 0)
        {
            throw new IndexOutOfRangeException();
        }

        if ((m_data[index] is T) == false)
        {
            throw new InvalidCastException();
        }

        return (T)m_data[index];
    }

    /// <summary>
    ///     Serializes this packet to its binary OSC representation for network transmission.
    /// </summary>
    /// <returns>A byte array containing the complete OSC packet in standard binary format.</returns>
    /// <remarks>
    ///     The resulting binary data follows the OSC 1.0 specification format and can be
    ///     transmitted over UDP, TCP, or other transport protocols. The binary format includes
    ///     the address pattern, type tags, and all argument data with proper alignment.
    /// </remarks>
    public abstract byte[] ToByteArray();

    /// <summary>
    ///     Writes the serialized packet data directly to the specified stream.
    /// </summary>
    /// <param name="stream">The target stream to write the OSC packet data to.</param>
    /// <returns>The number of bytes written to the stream.</returns>
    /// <remarks>
    ///     This method is more efficient than <see cref="ToByteArray"/> when writing directly
    ///     to network streams or files, as it avoids creating intermediate byte arrays.
    ///     The stream position will be advanced by the number of bytes written.
    /// </remarks>
    public abstract int Write(Stream stream);

    /// <summary>
    ///     Deserializes an OSC packet from its binary representation.
    /// </summary>
    /// <param name="data">The byte array containing the complete serialized OSC packet.</param>
    /// <returns>
    ///     An <see cref="OscMessage"/> if the data represents a message,
    ///     or an <see cref="OscBundle"/> if the data represents a bundle.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
    /// <remarks>
    ///     This factory method automatically detects the packet type by examining the data format.
    ///     OSC bundles are identified by starting with "#bundle", while messages start with
    ///     an address pattern beginning with "/".
    /// </remarks>
    public static OscPacket FromByteArray(byte[] data)
    {
        Assert.ParamIsNotNull(data);

        var start = 0;
        return FromByteArray(data, ref start, data.Length);
    }

    /// <summary>
    ///     Deserializes an OSC packet from a portion of a byte array.
    /// </summary>
    /// <param name="data">The byte array containing one or more serialized OSC packets.</param>
    /// <param name="start">
    ///     The starting index within the data array. Will be updated to point past
    ///     the end of the parsed packet.
    /// </param>
    /// <param name="end">The ending index (exclusive) within the data array.</param>
    /// <returns>
    ///     An <see cref="OscMessage"/> if the data represents a message,
    ///     or an <see cref="OscBundle"/> if the data represents a bundle.
    /// </returns>
    /// <remarks>
    ///     This overload is useful when parsing multiple packets from a single buffer
    ///     or when working with streaming data. The start parameter is updated to
    ///     facilitate parsing multiple consecutive packets.
    /// </remarks>
    public static OscPacket FromByteArray(byte[] data, ref int start, int end)
    {
        return data[start] == '#'
            ? (OscPacket)OscBundle.BundleFromByteArray(data, ref start, end)
            : OscMessage.MessageFromByteArray(data, ref start, end);
    }

    protected class DataBag
    {
        public byte[] Bytes { get; set; }
    }

    /// <summary>
    ///     Gets or sets the byte order used for serializing integral values in OSC packets.
    /// </summary>
    /// <value>
    ///     <c>true</c> for little-endian byte order (Intel/AMD standard);
    ///     <c>false</c> for big-endian byte order (network/OSC standard).
    /// </value>
    /// <remarks>
    ///     The OSC specification requires big-endian byte order (false), which is the default.
    ///     This setting affects all integer, float, and other multi-byte numeric types.
    ///     Changing this setting affects all subsequent packet serialization operations.
    /// </remarks>
    public static bool LittleEndianByteOrder { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this packet represents an OSC bundle or a single message.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this packet is an <see cref="OscBundle"/> containing multiple packets;
    ///     <c>false</c> if this packet is an <see cref="OscMessage"/> containing a single addressable call.
    /// </value>
    /// <remarks>
    ///     This property allows polymorphic handling of different OSC packet types.
    ///     Bundles can contain other bundles and messages, while messages contain arguments.
    /// </remarks>
    public abstract bool IsBundle { get; }


    /// <summary>
    ///     Gets or sets the OSC address pattern that identifies this packet's purpose or destination.
    /// </summary>
    /// <value>
    ///     A string representing the hierarchical address (e.g., "/synth/osc/1/frequency").
    ///     For messages, this specifies the target method or parameter.
    ///     For bundles, this is typically "#bundle".
    /// </value>
    /// <remarks>
    ///     OSC addresses follow a hierarchical naming convention similar to file system paths
    ///     or URLs. They enable precise routing of messages to specific handlers and support
    ///     pattern matching for wildcards and multiple targets.
    /// </remarks>
    public string Address { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this packet has been fully deserialized from binary data.
    /// </summary>
    /// <value>
    ///     <c>true</c> if the packet data has been parsed and is available in the <see cref="Data"/> collection;
    ///     <c>false</c> if the packet still contains raw binary data that needs to be parsed.
    /// </value>
    /// <remarks>
    ///     OSC packets support lazy deserialization to improve performance when handling large
    ///     amounts of data. The binary data is only parsed when the actual content is accessed.
    ///     This property helps determine the current state of the packet.
    /// </remarks>
    public bool IsDeserialised
    {
        get { return m_dataBag == null; }
    }

    /// <summary>
    ///     Gets the argument data contained in this packet as a read-only collection.
    /// </summary>
    /// <value>
    ///     A collection of objects representing the packet's arguments in their original .NET types.
    ///     For messages, these are the method arguments. For bundles, this may contain metadata.
    /// </value>
    /// <remarks>
    ///     Accessing this property will trigger deserialization if the packet is currently in
    ///     binary form. The returned objects maintain their original types (int, float, string, etc.)
    ///     for easy consumption by application code.
    /// </remarks>
    public IList<object> Data
    {
        get
        {
            EnsureDataLoaded();
            return m_data;
        }
    }


    /// <summary>
    ///     Ensures that the packet's raw binary data has been parsed into the <see cref="Data"/> collection.
    ///     This method handles lazy deserialization by converting stored binary data into typed objects.
    /// </summary>
    /// <remarks>
    ///     This is called automatically when accessing packet content, but can be called explicitly
    ///     to control when the potentially expensive parsing operation occurs. Once called,
    ///     <see cref="IsDeserialised"/> will return true.
    /// </remarks>
    protected abstract void EnsureDataLoaded();
}