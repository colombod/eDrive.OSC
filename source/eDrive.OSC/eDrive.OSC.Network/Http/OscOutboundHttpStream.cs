using System;
using System.IO;
using System.Net;
using System.Reactive.Concurrency;
using System.Text;
using eDrive.Osc;

namespace eDrive.Network.Http
{
    public class OscOutboundHttpStream : OscOutboundStreamBase
    {
        private readonly string m_destination;
        private readonly IObserver<OscPacket> m_responseStream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscOutboundHttpStream" /> class.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="scheduler">The scheduler.</param>
        public OscOutboundHttpStream(string destination, IScheduler scheduler = null)
            : this(destination, null, OscPaylaodMimeType.Json, scheduler)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscOutboundHttpStream" /> class.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="responseStream">The response stream.</param>
        /// <param name="mimetype">The mimetype.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <exception cref="System.ArgumentNullException">destination</exception>
        public OscOutboundHttpStream
            (string destination,
             IObserver<OscPacket> responseStream,
             OscPaylaodMimeType mimetype,
             IScheduler scheduler = null)
            : base(scheduler)
        {
            if (string.IsNullOrWhiteSpace(destination))
            {
                throw new ArgumentNullException(nameof(destination));
            }
            m_destination = destination;
            m_responseStream = responseStream;
            Mimetype = mimetype ?? OscPaylaodMimeType.Json;
        }

        public OscPaylaodMimeType Mimetype { get; }

        public override void Dispose()
        {
        }

        /// <summary>
        ///     Delivers the packet.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void DeliverPacket(OscPacket value)
        {
            if (value != null)
            {
                var postData = Mimetype == OscPaylaodMimeType.Json ? Encoding.UTF8.GetBytes(value.CreateJson()) : value.ToByteArray();

                // post method to destination.
                var request = WebRequest.Create(m_destination);
                request.Method = "POST";
                request.ContentType = Mimetype.Type;
                request.ContentLength = postData.Length;
                var state = new RequestState
                                {
                                    Request = request,
                                    Data = postData,
                                    Stream = request.GetRequestStream()
                                };
                state.Request.BeginGetRequestStream(RequestStreamReceived, state);
            }
        }

        private void RequestStreamReceived(IAsyncResult ar)
        {
            if (ar.AsyncState is RequestState state)
            {
                state.Stream = state.Request.EndGetRequestStream(ar);
                state.Stream.BeginWrite(state.Data, 0, state.Data.Length, SendCompleted, state);
            }
        }

        private void SendCompleted(IAsyncResult ar)
        {
            if (ar.AsyncState is RequestState state)
            {
                state.Data = null;
                state.Stream.EndWrite(ar);
                state.Stream.Dispose();

                state.Stream = null;
                state.Request.BeginGetResponse(ResponseReceived, state);
            }
        }

        private void ResponseReceived(IAsyncResult ar)
        {
            if (ar.AsyncState is RequestState state)
            {
                var response = state.Request.EndGetResponse(ar);
                state.Response = response;
                using (var stream = state.Response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream);
                        if (response.ContentType == OscPaylaodMimeType.Json.Type)
                        {
                            var ret = reader.ReadToEnd();

                            if (!string.IsNullOrWhiteSpace(ret)
                                && m_responseStream != null)
                            {
                                Scheduler.Schedule(() => ParseAndDeliver(ret, m_responseStream));
                            }
                        }
                        else
                        {
                            using var dst = new MemoryStream();
                            stream.CopyTo(dst);
                            dst.Flush();
                            var data = dst.ToArray();
                            Scheduler.Schedule(() => DeSerializerAndDeliver(data, m_responseStream));
                        }
                    }
                }
                state.Response.Close();
            }
        }

        private void DeSerializerAndDeliver(byte[] data, IObserver<OscPacket> responseStream)
        {
            OscPacket response;
            try
            {
                response = OscPacket.FromByteArray(data);
            }
            catch (Exception)
            {
                response = null;
            }
            if (response != null)
            {
                responseStream.OnNext(response);
            }
        }

        private void ParseAndDeliver(string ret, IObserver<OscPacket> responseStream)
        {
            OscPacket response;
            try
            {
                response = ret.CreateFromJson();
            }
            catch
            {
                response = null;
            }

            if (response != null)
            {
                responseStream.OnNext(response);
            }
        }

        #region Nested type: RequestState

        private class RequestState
        {
            public Stream Stream { get; set; }
            public WebRequest Request { get; set; }
            public WebResponse Response { get; set; }

            public byte[] Data { get; set; }
        }

        #endregion
    }
}