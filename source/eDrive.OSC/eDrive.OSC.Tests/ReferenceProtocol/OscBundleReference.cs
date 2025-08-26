using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace eDrive.OSC.Tests.ReferenceProtocol
{
    public sealed class OscBundleReference : OscPacketReference
    {
        private const string BundlePrefix = "#bundle";

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscBundle" /> class.
        /// </summary>
        /// <param name="sourceEndPoint">The packet origin.</param>
        public OscBundleReference(IPEndPoint sourceEndPoint)
            : this(sourceEndPoint, new OscTimeTag())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscBundle" /> class.
        /// </summary>
        /// <param name="sourceEndPoint">The packet origin.</param>
        /// <param name="timeStamp">The creation time of the bundle.</param>
        public OscBundleReference(IPEndPoint sourceEndPoint, OscTimeTag timeStamp)
            : base(sourceEndPoint, BundlePrefix)
        {
            TimeStamp = timeStamp;
        }

        /// <summary>
        ///     Specifies if the packet is an Osc bundle.
        /// </summary>
        public override bool IsBundle
        {
            get { return true; }
        }

        /// <summary>
        ///     Gets the creation time of the bundle.
        /// </summary>
        public OscTimeTag TimeStamp { get; private set; }

        /// <summary>
        ///     Gets the array of nested bundles.
        /// </summary>
        public IList<OscBundleReference> Bundles
        {
            get
            {
                var bundles = m_data.OfType<OscBundleReference>().ToList();

                return bundles.AsReadOnly();
            }
        }

        /// <summary>
        ///     Gets the array of contained messages.
        /// </summary>
        public IList<OscMessageReference> Messages
        {
            get
            {
                var messages = m_data.OfType<OscMessageReference>().ToList();

                return messages.AsReadOnly();
            }
        }

        /// <summary>
        ///     Serialize the packet.
        /// </summary>
        /// <returns>The newly serialized packet.</returns>
        public override byte[] ToByteArray()
        {
            var data = new List<byte>();

            data.AddRange(ValueToByteArray(Address));
            PadNull(data);

            data.AddRange(ValueToByteArray(TimeStamp));

            foreach (
                var packetBytes in
                    m_data.OfType<OscPacketReference>().Select(packet => packet.ToByteArray()))
            {
                Assert.IsTrue(packetBytes.Length % 4 == 0);

                data.AddRange(ValueToByteArray(packetBytes.Length));
                data.AddRange(packetBytes);
            }

            return data.ToArray();
        }

        /// <summary>
        ///     Deserialize the packet.
        /// </summary>
        /// <param name="sourceEndPoint">The packet origin.</param>
        /// <param name="data">The serialized packet.</param>
        /// <param name="start">The starting index into the serialized data stream.</param>
        /// <param name="end">The ending index into the serialized data stream.</param>
        /// <returns>The newly deserialized packet.</returns>
        public new static OscBundleReference FromByteArray
            (IPEndPoint sourceEndPoint, byte[] data, ref int start, int end)
        {
            var address = ValueFromByteArray<string>(data, ref start);
            Assert.IsTrue(address == BundlePrefix);

            var timeStamp = ValueFromByteArray<OscTimeTag>(data, ref start);
            var bundle = new OscBundleReference(sourceEndPoint, timeStamp);

            while (start < end)
            {
                var length = ValueFromByteArray<int>(data, ref start);
                var packetEnd = start + length;
                bundle.Append(OscPacketReference.FromByteArray(sourceEndPoint, data, ref start, packetEnd));
            }

            return bundle;
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
            Assert.IsTrue(value is OscPacketReference);

            var nestedBundle = value as OscBundleReference;
            if (nestedBundle != null)
            {
                Assert.IsTrue(nestedBundle.TimeStamp >= TimeStamp);
            }

            m_data.Add(value);

            return m_data.Count - 1;
        }
    }
}