using System;
using System.Net;
using Newtonsoft.Json;
using eDrive.Osc;
using eDrive.Network;
using System.IO;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serialiser for <see cref="OscEndPoint" />.
    /// </summary>
    [CustomOscJSonSerialiser('E', typeof (OscEndPoint))]
    public class EndPointJsonSerialiser : OscTypeJsonSerialiser<OscEndPoint>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EndPointSerialiser" /> class.
        /// </summary>
		public EndPointJsonSerialiser() : base('E')
        {
        }

        public override OscEndPoint Decode(JsonReader reader)
        {
            reader.Read();
            var ip = IPAddress.Parse(reader.ReadAsString());
            var port = reader.ReadAsInt32() ?? 0;

            var transmissionType = (TransmissionType) Enum.Parse(typeof (TransmissionType), reader.ReadAsString());
            var transportType = (TransportType) Enum.Parse(typeof (TransportType), reader.ReadAsString());
            reader.Read();
            var ret = new OscEndPoint(ip, port, transportType, transmissionType);
            return ret;
        }

        public override void Encode(JsonWriter output, OscEndPoint value)
        {
            output.WriteStartArray();
            output.WriteValue(value.EndPoint.Address.ToString());
            output.WriteValue(value.EndPoint.Port);
            output.WriteValue(value.TransmissionType.ToString());
            output.WriteValue(value.TransportType.ToString());
            output.WriteEndArray();
        }
    }

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