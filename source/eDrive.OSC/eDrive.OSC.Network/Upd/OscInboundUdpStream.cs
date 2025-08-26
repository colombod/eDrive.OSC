using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;

namespace eDrive.OSC.Network.Upd;

public class OscInboundUdpStream : OscInboundStreamBase
{
    private const int MaxUDPSize = 65536;
    private readonly OscEndPoint m_endPoint;
    private readonly Socket m_socket;

    private bool m_running;

    public OscInboundUdpStream(int port, TransmissionType transmissionType, IScheduler scheduler = null)
        : this(new OscEndPoint(IPAddress.Any, port, TransportType.Udp, transmissionType), scheduler)
    {
    }

    public OscInboundUdpStream(OscEndPoint endPoint, IScheduler scheduler = null)
        : base(scheduler)
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

        m_socket.Bind(m_endPoint.EndPoint);

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
    }

    public override void Start()
    {
        m_running = true;
        ReceivePacket();
    }

    private void ReceivePacket()
    {
        try
        {
            var buffer = new byte[MaxUDPSize];
            m_socket.BeginReceive(buffer, 0, MaxUDPSize, SocketFlags.None, CompleteRead, buffer);
        }
        catch (SocketException s)
        {
            m_running = false;
        }
        finally
        {
            if (m_running)
            {
                ReceivePacket();
            }
        }
    }

    private void CompleteRead(IAsyncResult ar)
    {
        var size = m_socket.EndReceive(ar);

        if (size > 0
            && ar.AsyncState is byte[] data)
        {
            Scheduler.Schedule(() =>
            {
                var start = 0;
                OscPacket packet = null;
                try
                {
                    packet = OscPacket.FromByteArray(data, ref start, size);
                }
                catch (Exception e)
                {
                    if (!SuppressParsingExceptions)
                    {
                        throw new Exception("Error deserialising packet", e);
                    }

                    packet = null;
                }

                if (packet != null)
                {
                    ForwardPacket(packet);
                }
            });
        }
    }

    public override void Stop()
    {
        m_running = false;
        m_socket.Dispose();
    }
}