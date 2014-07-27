#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using eDrive.Osc.Serialisation;

#endregion

namespace eDrive.Osc
{
    /// <summary>
    ///     Represents an Osc Message packet.
    /// </summary>
    public class OscMessage : OscPacket
    {
        /// <summary>
        ///     The prefix required by Osc address patterns.
        /// </summary>
        protected const string AddressPrefix = "/";

        private bool m_isEvent;
        private string m_typeTag;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscMessage" /> class.
        /// </summary>
        /// <param name="address">The Osc address pattern.</param>
        /// <param name="value">A value to append to the message.</param>
        public OscMessage(string address, object value)
            : this(address)
        {
            InitialiseWithValue(value);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscMessage" /> class.
        /// </summary>
        /// <param name="address">The Osc address pattern.</param>
        public OscMessage(string address)
            : base(address)
        {
            Assert.IsTrue(address.StartsWith(AddressPrefix));

			m_typeTag = SerialiserFactory.DefaultTag.ToString();
        }

        /// <summary>
        ///     Specifies if the packet is an Osc bundle.
        /// </summary>
        public override bool IsBundle
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets the type tag.
        /// </summary>
        /// <value>
        ///     The type tag.
        /// </value>
        public string TypeTag
        {
            get { return m_typeTag; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is event.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is event; otherwise, <c>false</c>.
        /// </value>
        public bool IsEvent
        {
            get { return m_isEvent; }
            set
            {
                m_isEvent = value;
                m_typeTag = m_typeTag.Replace(SerialiserFactory.EventTagString, string.Empty);

                if (m_isEvent)
                {
                    m_typeTag = m_typeTag + SerialiserFactory.EventTagString;
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
            var stringSer = SerialiserFactory.GetSerialiser<string>();
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
                            var ser = SerialiserFactory.GetSerialiser(component);
                            size += ser.Encode(stream, component);
                        }
                    }
                    else
                    {
                        var ser = SerialiserFactory.GetSerialiser(part);
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
            var sd = SerialiserFactory.StringSerialiser;

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

                    if (tag == SerialiserFactory.ArrayOpen)
                    {
                        // skip the '[' character.
                        index++;

                        // deserialise array of object
                        var ret = new List<object>();
                        while (tags[index] != SerialiserFactory.ArrayClose
                               && index < tags.Length)
                        {
                            int pos;

                            var des = SerialiserFactory.GetSerialiser(tags[index]);
                            ret.Add(des.Decode(data, start, out pos));

                            start = pos;
                            index++;
                        }

                        m_data.Add(ret.ToArray());
                    }
                    else if (tag != SerialiserFactory.DefaultTag)
                    {
                        if (tag != SerialiserFactory.EventTag)
                        {
                            int pos;

                            var des = SerialiserFactory.GetSerialiser(tag);
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
                typeTag = SerialiserFactory.GetTag(value);
            }
            catch (KeyNotFoundException e)
            {
                throw new OscSerialiserException(value.GetType(), e);
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
            m_typeTag = SerialiserFactory.DefaultTag.ToString();
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
            m_typeTag = SerialiserFactory.DefaultTag.ToString();
            m_data.Clear();
        }
    }
}