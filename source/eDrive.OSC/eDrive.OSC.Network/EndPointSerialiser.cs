using System;
using System.Net;
using Newtonsoft.Json;
using eDrive.Network;
using System.IO;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Serializer for <see cref="OscEndPoint" />.
    /// </summary>
    [CustomOscJSonSerializer('E', typeof (OscEndPoint))]
    public class EndPointJsonSerializer : OscTypeJsonSerializer<OscEndPoint>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EndPointSerializer" /> class.
        /// </summary>
		public EndPointJsonSerializer() : base('E')
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
	///     Serializer for <see cref="OscEndPoint" />.
	/// </summary>
	[CustomOscSerializer('E', typeof (OscEndPoint))]
	public class EndPointSerializer : OscTypeSerializer<OscEndPoint>
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="EndPointSerializer" /> class.
		/// </summary>
		public EndPointSerializer() : base('E')
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
			var ip = new IPAddress(SerializerFactory.ByteArraySerializer.Decode(data, start, out position));
			var port = SerializerFactory.IntSerializer.Decode(data, position, out position);
			var transmissionType =
				(TransmissionType) SerializerFactory.IntSerializer.Decode(data, position, out position);
			var transportType = (TransportType) SerializerFactory.IntSerializer.Decode(data, position, out position);

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

			var ret = SerializerFactory.ByteArraySerializer.Encode(output, ip);
			ret += SerializerFactory.IntSerializer.Encode(output, value.EndPoint.Port);
			ret += SerializerFactory.IntSerializer.Encode(output, value.TransmissionType);
			ret += SerializerFactory.IntSerializer.Encode(output, value.TransportType);

			return ret;
		}
	}
}