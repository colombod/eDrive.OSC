using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using eDrive.Osc;
using eDrive.Osc.Serialisation;

namespace eDrive.Network.Tcp
{
    public class OscOutboundTcpStream : OscOutboundStreamBase
    {
        private readonly Socket m_socket;
        private bool m_disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscOutboundTcpStream" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="transmissionType">Type of the transmission.</param>
        /// <param name="scheduler">The scheduler.</param>
        public OscOutboundTcpStream
            (IPAddress address, int port, TransmissionType transmissionType, IScheduler scheduler)
            : this(new OscEndPoint(address, port, TransportType.Tcp, transmissionType), scheduler)
        {
        }

        public OscOutboundTcpStream(OscEndPoint endPoint, IScheduler scheduler) : base(scheduler)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }

            if (endPoint.TransportType != TransportType.Tcp)
            {
                throw new InvalidOperationException("This Stream supports only tcp endpoints");
            }

            m_socket = new Socket(endPoint.EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Connect(endPoint.EndPoint);
        }

        public override void Dispose()
        {
            m_socket.Dispose();
            m_disposed = true;
        }

        protected override void DeliverPacket(OscPacket value)
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException("The stream has been disposed");
            }

            if (value != null)
            {
                var data = value.ToByteArray();
                if (data != null
                    && data.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        SerializerFactory.ByteArraySerializer.Encode(stream, data);
                        stream.Flush();
                        var msg = stream.ToArray();
                        m_socket.BeginSend(msg, 0, msg.Length, SocketFlags.None, null, null);
                    }
                }
            }
        }
    }
}