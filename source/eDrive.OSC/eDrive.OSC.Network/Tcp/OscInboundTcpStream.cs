using eDrive.OSC.Serialisation;

using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;

namespace eDrive.OSC.Network.Tcp
{
    public class OscInboundTcpStream : OscInboundStreamBase
    {
        private readonly OscEndPoint m_endPoint;
        private TcpListener m_listener;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscInboundTcpStream" /> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="scheduler">The scheduler.</param>
        public OscInboundTcpStream(int port, IScheduler scheduler = null) :
            this(new OscEndPoint(IPAddress.Any, port, TransportType.Tcp, TransmissionType.Unicast), scheduler)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscInboundTcpStream" /> class.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <param name="scheduler">The scheduler.</param>
        public OscInboundTcpStream(OscEndPoint endPoint, IScheduler scheduler = null) : base(scheduler)
        {
            m_endPoint = endPoint;
        }

        public override void Start()
        {
            m_listener = new TcpListener(m_endPoint.EndPoint);
            m_listener.Start();

            StartAccept();
        }

        private void StartAccept()
        {
            m_listener.BeginAcceptSocket(EndAccept, null);
        }

        private void EndAccept(IAsyncResult ar)
        {
            var client = m_listener.EndAcceptSocket(ar);
            var state = new TcpConnectionState
            {
                Client = client,
                Scheduler = Scheduler,
                Stream = this,
                Buffer = new byte[4],
                ToRead = 4,
                Offset = 0
            };
            // start the io completion listening

            BeginReadHeader(state);
        }

        private static void BeginReadHeader(TcpConnectionState state)
        {
            state.Client.BeginReceive(state.Buffer, state.Offset, state.ToRead, SocketFlags.None, EndReadHeader, state);
        }

        private static void EndReadHeader(IAsyncResult ar)
        {
            var state = ar.AsyncState as TcpConnectionState;
            if (state == null)
            {
                throw new InvalidOperationException("Connection state is not set");
            }

            var read = state.Client.EndReceive(ar);

            state.ToRead -= read;
            state.Offset += read;
            if (state.ToRead > 0)
            {
                state.Client.BeginReceive(state.Buffer,
                                          state.Offset,
                                          state.ToRead,
                                          SocketFlags.None,
                                          EndReadHeader,
                                          state);
            }
            else
            {
                // header read, now proceed with message
                var cursor = 0;
                var messageSize = SerializerFactory.IntSerializer.Decode(state.Buffer, cursor, out cursor);
                state.ToRead = messageSize;
                state.Offset = 0;
                state.Buffer = new byte[messageSize];

                state.Client.BeginReceive(state.Buffer, state.Offset, state.ToRead, SocketFlags.None, EndReadBody, state);
            }
        }

        private static void EndReadBody(IAsyncResult ar)
        {
            var state = ar.AsyncState as TcpConnectionState;
            if (state == null)
            {
                throw new InvalidOperationException("Connection state is not set");
            }

            var read = state.Client.EndReceive(ar);

            state.ToRead -= read;
            state.Offset += read;
            if (state.ToRead > 0)
            {
                state.Client.BeginReceive(state.Buffer, state.Offset, state.ToRead, SocketFlags.None, EndReadBody, state);
            }
            else
            {
                var data = state.Buffer;
                var stream = state.Stream;

                // schedule delivery
                state.Scheduler.Schedule(() => DeserialiseAndDeliver(data, stream));

                // deserialise and push, start again
                state.Offset = 0;
                state.ToRead = 4;
                state.Buffer = new byte[4];
                BeginReadHeader(state);
            }
        }

        private static void DeserialiseAndDeliver(byte[] data, OscInboundTcpStream stream)
        {
            OscPacket packet;
            try
            {
                packet = OscPacket.FromByteArray(data);
            }
            catch (Exception e)
            {
                packet = null;
            }

            if (packet != null)
            {
                stream.ForwardPacket(packet);
            }
        }

        public override void Stop()
        {
            m_listener.Stop();
            m_listener = null;
        }

        #region Nested type: TcpConnectionState

        private class TcpConnectionState
        {
            public Socket Client { get; set; }
            public IScheduler Scheduler { get; set; }
            public OscInboundTcpStream Stream { get; set; }

            public byte[] Buffer { get; set; }

            public int ToRead { get; set; }

            public int Offset { get; set; }
        }

        #endregion
    }
}