// File :		EndPointSerialiser.cs
// Copyright :  	2012-2013 mUBreeze ltd.
// Created : 		05-2013

#region

using System.IO;
using System.Net;
using mUBreeze.eDrive.Network;

#endregion

namespace mUBreeze.eDrive.Osc.Serialisation
{
    /// <summary>
    ///     Serialiser for <see cref="OscEndPoint" />.
    /// </summary>
    [CustomOscSerialiser('E', typeof (OscEndPoint))]
    public class EndPointSerialiser : OscTypeSerialiser<OscEndPoint>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EndPointSerialiser" /> class.
        /// </summary>
        public EndPointSerialiser() : base('E')
        {
        }

        /// <summary>
        ///     Decodes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override OscEndPoint Decode(byte[] data, int start, out int position)
        {
            var ip = new IPAddress(SerialiserFactory.ByteArraySerialiser.Decode(data, start, out position));
            var port = SerialiserFactory.IntSerialiser.Decode(data, position, out position);
            var transmissionType =
                (TransmissionType) SerialiserFactory.IntSerialiser.Decode(data, position, out position);
            var transportType = (TransportType) SerialiserFactory.IntSerialiser.Decode(data, position, out position);

            return new OscEndPoint(ip, port, transportType, transmissionType);
        }

        /// <summary>
        ///     Encodes the specified output.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override int Encode(Stream output, OscEndPoint value)
        {
            var ip = value.EndPoint.Address.GetAddressBytes();

            var ret = SerialiserFactory.ByteArraySerialiser.Encode(output, ip);
            ret += SerialiserFactory.IntSerialiser.Encode(output, value.EndPoint.Port);
            ret += SerialiserFactory.IntSerialiser.Encode(output, value.TransmissionType);
            ret += SerialiserFactory.IntSerialiser.Encode(output, value.TransportType);

            return ret;
        }
    }
}