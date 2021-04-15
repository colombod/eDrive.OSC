
using System;
using System.Net;
using eDrive.Network;
using eDrive.Osc.Serialisation;
using eDrive.Osc.Serialisation.Json;
namespace eDrive.Osc
{
    /// <summary>
    ///     Osc Network endpoint
    /// </summary>
    public class OscEndPoint
    {
		static OscEndPoint()
		{
			SerializerFactory.LoadSerializer(typeof(EndPointSerializer));
			JsonSerializerFactory.LoadSerializer (typeof(EndPointJsonSerializer));
		}

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscEndPoint" /> class.
        /// </summary>
        /// <param name="ep">The ep.</param>
        /// <param name="transportType">Type of the transport.</param>
        /// <param name="transmissionType">Type of the transmission.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">transmissionType</exception>
        public OscEndPoint(IPEndPoint ep, TransportType transportType, TransmissionType transmissionType)
        {
            EndPoint = ep;
            TransportType = transportType;
            TransmissionType = transmissionType;
            switch (transmissionType)
            {
                case TransmissionType.Unicast:
                    break;
                case TransmissionType.Multicast:
                    break;
                case TransmissionType.Broadcast:
                    break;
                case TransmissionType.LocalBroadcast:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("transmissionType");
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscEndPoint" /> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        /// <param name="transportType">Type of the transport.</param>
        /// <param name="transmissionType">Type of the transmission.</param>
        public OscEndPoint(IPAddress address, int port, TransportType transportType, TransmissionType transmissionType)
            : this(new IPEndPoint(address, port), transportType, transmissionType)
        {
        }

        /// <summary>
        ///     Gets the end point.
        /// </summary>
        /// <value>
        ///     The end point.
        /// </value>
        public IPEndPoint EndPoint { get; internal set; }

        /// <summary>
        ///     Gets the type of the transport.
        /// </summary>
        /// <value>
        ///     The type of the transport.
        /// </value>
        public TransportType TransportType { get; private set; }

        /// <summary>
        ///     Gets the type of the transmission.
        /// </summary>
        /// <value>
        ///     The type of the transmission.
        /// </value>
        public TransmissionType TransmissionType { get; internal set; }
    }
}