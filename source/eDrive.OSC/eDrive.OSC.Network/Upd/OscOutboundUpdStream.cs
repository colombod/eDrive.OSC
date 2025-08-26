using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;

namespace eDrive.OSC.Network.Upd
{
    /// <summary>
    ///     Upd stream for udp communication
    /// </summary>
    public class OscOutboundUpdStream : OscOutboundStreamBase
    {
        private readonly OscEndPoint m_endPoint;
        private readonly Socket m_socket;
        private bool m_disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscOutboundUpdStream" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="transmissionType">Type of the transmission.</param>
        /// <param name="scheduler">The scheduler.</param>
        public OscOutboundUpdStream
            (IPAddress address, int port, TransmissionType transmissionType, IScheduler scheduler)
            : this(new OscEndPoint(address, port, TransportType.Udp, transmissionType), scheduler)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscOutboundUpdStream" /> class.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <exception cref="System.ArgumentNullException">endPoint</exception>
        /// <exception cref="System.InvalidOperationException">This Stream supports only unicat udp endpoints</exception>
        public OscOutboundUpdStream(OscEndPoint endPoint, IScheduler scheduler) : base(scheduler)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException("endPoint");
            }

            if (endPoint.TransportType != TransportType.Udp)
            {
                throw new InvalidOperationException("This Stream supports only udp endpoints");
            }


            m_endPoint = endPoint;
            m_socket = new Socket(m_endPoint.EndPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp);


            if (endPoint.TransmissionType == TransmissionType.Multicast)
            {
                //224.0.0.0-239.255.255.255 
                var range = IPAddressRange.MulticastRange;

                if (range.IsInRange(m_endPoint.EndPoint.Address))
                {
                    // setup multicast
                    m_socket.SetSocketOption(
                        SocketOptionLevel.IP,
                        SocketOptionName.AddMembership,
                        new MulticastOption(m_endPoint.EndPoint.Address));

                    // join multicast group
                    m_socket.SetSocketOption(
                        SocketOptionLevel.IP,
                        SocketOptionName.MulticastTimeToLive,
                        5);
                }
                else
                {
                    throw new InvalidOperationException("Address is not in multicast address range");
                }
            }
            else if (m_endPoint.TransmissionType == TransmissionType.Broadcast)
            {
                // setup broadcast 
                m_socket.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.Broadcast,
                    true);
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public override void Dispose()
        {
            m_socket.Dispose();
            m_disposed = true;
        }

        /// <summary>
        ///     Delivers the packet.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override void DeliverPacket(OscPacket value)
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException("The stream has been disposed");
            }
            var data = value.ToByteArray();
            m_socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, m_endPoint.EndPoint, null, null);
        }
    }
}