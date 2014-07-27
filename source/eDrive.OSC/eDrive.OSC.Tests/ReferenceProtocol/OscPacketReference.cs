// File :		OscPacketReference.cs
// Copyright :  	2012-2014 Diego Colombo ltd.
// Created : 		06-2014

#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;

#endregion

namespace eDrive.Osc.Tests.ReferenceProtocol
{
    public abstract class OscPacketReference
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the expected endianness of integral value types.
        /// </summary>
        /// <remarks>Defaults to false (big endian).</remarks>
        public static bool LittleEndianByteOrder { get; set; }

        /// <summary>
        ///     Specifies if the packetReference is an OSC bundle.
        /// </summary>
        public abstract bool IsBundle { get; }

        /// <summary>
        ///     Gets the origin of the packetReference.
        /// </summary>
        public IPEndPoint SourceEndPoint { get; protected set; }

        /// <summary>
        ///     Gets the Osc address pattern.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Gets the contents of the packetReference.
        /// </summary>
        public IList<object> Data
        {
            get { return m_data.AsReadOnly(); }
        }

        #endregion

        /// <summary>
        ///     The contents of the packetReference.
        /// </summary>
        protected List<object> m_data;

        /// <summary>
        ///     Initialize static members.
        /// </summary>
        static OscPacketReference()
        {
            LittleEndianByteOrder = false;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscPacketReference" /> class.
        /// </summary>
        /// <param name="sourceEndPoint">The packetReference origin.</param>
        /// <param name="address">The OSC address pattern.</param>
        protected OscPacketReference(IPEndPoint sourceEndPoint, string address)
        {
            Assert.IsFalse(string.IsNullOrEmpty(address));

            SourceEndPoint = sourceEndPoint;
            Address = address;
            m_data = new List<object>();
        }

        /// <summary>
        ///     Appends a value to the packetReference.
        /// </summary>
        /// <typeparam name="T">The type of object being appended.</typeparam>
        /// <param name="value">The value to append.</param>
        /// <returns>The index of the newly added value within the Data property.</returns>
        /// <returns>The index of the newly appended value.</returns>
        public abstract int Append<T>(T value);

        /// <summary>
        ///     Return a entry in the packetReference.
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

            return (T) m_data[index];
        }

        /// <summary>
        ///     Serialize the packetReference.
        /// </summary>
        /// <returns>The newly serialized packetReference.</returns>
        public abstract byte[] ToByteArray();

        /// <summary>
        ///     Deserialize the packetReference.
        /// </summary>
        /// <param name="sourceEndPoint">The packetReference origin.</param>
        /// <param name="data">The serialized packetReference.</param>
        /// <returns>The newly deserialized packetReference.</returns>
        public static OscPacketReference FromByteArray(IPEndPoint sourceEndPoint, byte[] data)
        {
            Assert.ParamIsNotNull(data);

            var start = 0;
            return FromByteArray(sourceEndPoint, data, ref start, data.Length);
        }

        /// <summary>
        ///     Deserialize the packetReference.
        /// </summary>
        /// <param name="sourceEndPoint">The packetReference origin.</param>
        /// <param name="data">The serialized packetReference.</param>
        /// <param name="start">The starting index into the serialized data stream.</param>
        /// <param name="end">The ending index into the serialized data stream.</param>
        /// <returns>The newly deserialized packetReference.</returns>
        public static OscPacketReference FromByteArray(IPEndPoint sourceEndPoint, byte[] data, ref int start, int end)
        {
            return data[start] == '#'
                       ? (OscPacketReference) OscBundleReference.FromByteArray(sourceEndPoint, data, ref start, end)
                       : OscMessageReference.FromByteArray(sourceEndPoint, data, ref start);
        }


        /// <summary>
        ///     Deserialize a value.
        /// </summary>
        /// <typeparam name="T">The value's data type.</typeparam>
        /// <param name="data">The serialized data source.</param>
        /// <param name="start">The starting index into the serialized data stream.</param>
        /// <returns>The newly deserialized value.</returns>
        public static T ValueFromByteArray<T>(byte[] data, ref int start)
        {
            var type = typeof (T);
            object value;

            switch (type.Name)
            {
                case "String":
                    {
                        var count = 0;
                        for (var index = start; index < data.Length && data[index] != 0; index++)
                        {
                            count++;
                        }
                        value = Encoding.ASCII.GetString(data, start, count);
                        start += count + 1;
                        start = ((start + 3)/4)*4;
                        break;
                    }

                case "Byte[]":
                    {
                        var length = ValueFromByteArray<int>(data, ref start);
                        var buffer = data.CopySubArray(start, length);

                        value = buffer;
                        start += buffer.Length + 1;
                        start = ((start + 3)/4)*4;
                        break;
                    }

                case "OscTimeTag":
                    {
                        var buffer = data.CopySubArray(start, 8);
                        value = new OscTimeTag(buffer);
                        start += buffer.Length;
                        break;
                    }

                case "Char":
                    {
                        value = Convert.ToChar(ValueFromByteArray<int>(data, ref start));
                        break;
                    }

                case "Color":
                    {
                        var buffer = data.CopySubArray(start, 4);
                        start += buffer.Length;

                        value = Color.FromArgb(buffer[3], buffer[0], buffer[1], buffer[2]);
                        break;
                    }

                default:
                    {
                        value = ByteArrayToNumeric(data, ref start, type);
                        break;
                    }
            }

            return (T) value;
        }

        private static object ByteArrayToNumeric(byte[] data, ref int start, Type type)
        {
            object value;
            int bufferLength;
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Int32:
                case TypeCode.Single:
                    bufferLength = 4;
                    break;

                case TypeCode.Int64:
                case TypeCode.Double:
                    bufferLength = 8;
                    break;

                default:
                    throw new Exception("Unsupported data type.");
            }

            var buffer = data.CopySubArray(start, bufferLength);
            start += buffer.Length;

            if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
            {
                buffer = Utility.SwapEndian(buffer);
            }

            switch (code)
            {
                case TypeCode.Int32:
                    value = BitConverter.ToInt32(buffer, 0);
                    break;

                case TypeCode.Int64:
                    value = BitConverter.ToInt64(buffer, 0);
                    break;

                case TypeCode.Single:
                    value = BitConverter.ToSingle(buffer, 0);
                    break;

                case TypeCode.Double:
                    value = BitConverter.ToDouble(buffer, 0);
                    break;

                default:
                    throw new Exception("Unsupported data type.");
            }
            return value;
        }

        /// <summary>
        ///     Serialize a value.
        /// </summary>
        /// <typeparam name="T">The value's data type.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <returns>The serialized version of the value.</returns>
        public static byte[] ValueToByteArray<T>(T value)
        {
            byte[] data = null;
            object valueObject = value;

            if (valueObject != null)
            {
                var type = value.GetType();

                switch (type.Name)
                {
                    case "String":
                        {
                            data = Encoding.ASCII.GetBytes((string) valueObject);
                            break;
                        }

                    case "Byte[]":
                        {
                            var valueData = ((byte[]) valueObject);
                            var bytes = new List<byte>();
                            bytes.AddRange(ValueToByteArray(valueData.Length));
                            bytes.AddRange(valueData);
                            data = bytes.ToArray();
                            break;
                        }

                    case "OscTimeTag":
                        {
                            data = ((OscTimeTag) valueObject).ToByteArray();
                            break;
                        }

                    case "Char":
                        {
                            data = ValueToByteArray(Convert.ToInt32((char) valueObject));
                            break;
                        }

                    case "Color":
                        {
                            var color = (Color) valueObject;
                            byte[] bytes = {color.R, color.G, color.B, color.A};

                            data = bytes;
                            break;
                        }

                    case "Boolean":
                        {
                            // No payload for Boolean data tag.
                            break;
                        }

                    default:
                        data = NumericToByteArray(type, valueObject);
                        break;
                }
            }

            return data;
        }

        private static byte[] NumericToByteArray(Type type, object value)
        {
            var data = new byte[0];
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Int32:
                    data = BitConverter.GetBytes((int) value);
                    if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
                    {
                        data = Utility.SwapEndian(data);
                    }
                    break;

                case TypeCode.Int64:
                    data = BitConverter.GetBytes((long) value);
                    if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
                    {
                        data = Utility.SwapEndian(data);
                    }
                    break;

                case TypeCode.Single:
                    var floatValue = (float) value;
                    // No payload for Infinitum data tag.
                    if (float.IsPositiveInfinity(floatValue) == false)
                    {
                        data = BitConverter.GetBytes(floatValue);
                        if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
                        {
                            data = Utility.SwapEndian(data);
                        }
                    }
                    break;

                case TypeCode.Double:
                    data = BitConverter.GetBytes((double) value);
                    if (BitConverter.IsLittleEndian != LittleEndianByteOrder)
                    {
                        data = Utility.SwapEndian(data);
                    }
                    break;

                default:
                    throw new Exception("Unsupported data type.");
            }
            return data;
        }

        /// <summary>
        ///     Pad a series of 0-3 null (zero) values.
        /// </summary>
        /// <param name="data">The source data to pad.</param>
        public static void PadNull(List<byte> data)
        {
            const int zero = 0;
            var pad = 4 - (data.Count%4);
            for (var i = 0; i < pad; i++)
            {
                data.Add(zero);
            }
        }
    }
}