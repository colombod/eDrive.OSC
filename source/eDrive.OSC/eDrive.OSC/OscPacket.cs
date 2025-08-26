using System;
using System.Collections.Generic;
using System.IO;

namespace eDrive.OSC;

/// <summary>
///     Represents the base unit of transmission for the Open Sound Control protocol.
/// </summary>
public abstract class OscPacket
{
    /// <summary>
    ///     The contents of the packet.
    /// </summary>
    protected List<object> m_data;

    /// <summary>
    ///     The m_data bag
    /// </summary>
    protected DataBag m_dataBag;

    /// <summary>
    ///     Initialize static members.
    /// </summary>
    static OscPacket()
    {
        LittleEndianByteOrder = false;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OscPacket" /> class.
    /// </summary>
    /// <param name="address">The Osc address pattern.</param>
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
    ///     Appends a value to the packet.
    /// </summary>
    /// <typeparam name="T">The type of object being appended.</typeparam>
    /// <param name="value">The value to append.</param>
    /// <returns>The index of the newly added value within the Data property.</returns>
    /// <returns>The index of the newly appended value.</returns>
    public abstract int Append<T>(T value);

    /// <summary>
    ///     Return a entry in the packet.
    /// </summary>
    /// <typeparam name="T">The type of value expected at index.</typeparam>
    /// <param name="index">The index within the data array.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if specified index is out of range.</exception>
    /// <exception cref="InvalidCastException">Thrown if the specified T is incompatible with the data at index.</exception>
    /// <returns>The entry at the specified index.</returns>
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
    ///     Serialize the packet.
    /// </summary>
    /// <returns>The newly serialized packet.</returns>
    public abstract byte[] ToByteArray();

    /// <summary>
    ///     Serialises to the specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public abstract int Write(Stream stream);

    /// <summary>
    ///     Deserialize the packet.
    /// </summary>
    /// <param name="data">The serialized packet.</param>
    /// <returns>The newly deserialized packet.</returns>
    public static OscPacket FromByteArray(byte[] data)
    {
        Assert.ParamIsNotNull(data);

        var start = 0;
        return FromByteArray(data, ref start, data.Length);
    }

    /// <summary>
    ///     Deserialize the packet.
    /// </summary>
    /// <param name="data">The serialized packet.</param>
    /// <param name="start">The starting index into the serialized data stream.</param>
    /// <param name="end">The ending index into the serialized data stream.</param>
    /// <returns>The newly deserialized packet.</returns>
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
    ///     Gets or sets the expected endianness of integral value types.
    /// </summary>
    /// <remarks>Defaults to false (big endian).</remarks>
    public static bool LittleEndianByteOrder { get; set; }

    /// <summary>
    ///     Specifies if the packet is an Osc bundle.
    /// </summary>
    public abstract bool IsBundle { get; }


    /// <summary>
    ///     Gets the Osc address pattern.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    ///     Gets a value indicating whether this instance is deserialised.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is deserialised; otherwise, <c>false</c>.
    /// </value>
    public bool IsDeserialised
    {
        get { return m_dataBag == null; }
    }

    /// <summary>
    ///     Gets the contents of the packet.
    /// </summary>
    public IList<object> Data
    {
        get
        {
            EnsureDataLoaded();
            return m_data;
        }
    }


    /// <summary>
    ///     Ensures that the data is loaded.
    /// </summary>
    protected abstract void EnsureDataLoaded();
}