using eDrive.OSC.Serialisation;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace eDrive.OSC
{
    /// <summary>
    ///     Represents a bundle of <see cref="OscMessage" /> and other <see cref="OscBundle" /> objects.
    /// </summary>
    public sealed class OscBundle : OscPacket
    {
        private const string BundlePrefix = "#bundle";

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscBundle" /> class.
        /// </summary>
        public OscBundle()
            : this(new OscTimeTag())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscBundle" /> class.
        /// </summary>
        /// <param name="timeStamp">The creation time of the bundle.</param>
        public OscBundle(OscTimeTag timeStamp)
            : base(BundlePrefix)
        {
            TimeStamp = timeStamp;
        }

        /// <summary>
        ///     Specifies if the packet is an Osc bundle.
        /// </summary>
        public override bool IsBundle => true;

        /// <summary>
        ///     Gets the creation time of the bundle.
        /// </summary>
        public OscTimeTag TimeStamp { get; private set; }

        /// <summary>
        ///     Gets the array of nested bundles.
        /// </summary>
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
        ///     Gets the array of contained messages.
        /// </summary>
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
}