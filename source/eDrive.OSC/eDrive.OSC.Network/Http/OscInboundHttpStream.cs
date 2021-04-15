using System;
using System.IO;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using eDrive.Osc;

namespace eDrive.Network.Http
{
    public class OscInboundHttpStream : OscInboundStreamBase
    {
        private readonly string m_source;
        private readonly TimeSpan m_pollingInterval;
        private volatile bool m_running;
        private IDisposable m_pending;

        /// <summary>
        /// Initializes a new instance of the <see cref="OscInboundHttpStream" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="scheduler">The scheduler.</param>
        public OscInboundHttpStream(string source, IScheduler scheduler = null)
            : this(source, OscPaylaodMimeType.Json, TimeSpan.FromSeconds(0.2), scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OscInboundHttpStream" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mimetype">The mimetype.</param>
        /// <param name="pollingInterval">The polling interval.</param>
        /// <param name="scheduler">The scheduler.</param>
        /// <exception cref="System.ArgumentNullException">source</exception>
        public OscInboundHttpStream(string source, OscPaylaodMimeType mimetype, TimeSpan pollingInterval, IScheduler scheduler = null)
            : base(scheduler)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException("source");
            }
            m_source = source;
            m_pollingInterval = pollingInterval;
            Mimetype = mimetype ?? OscPaylaodMimeType.Json;
        }

        public TimeSpan PollingInterval
        {
            get { return m_pollingInterval; }
        }

        public OscPaylaodMimeType Mimetype { get; private set; }

        public override void Start()
        {
            m_running = true;
            Scheduler.Schedule(PollAndDeliver);
            
        }

        public override void Stop()
        {
            m_running = false;
            if (m_pending != null)
            {
                m_pending.Dispose();
                m_pending = null;
            }
        }

        private void PollAndDeliver()
        {
            m_pending = null;
            if (m_running)
            {
                // post method to destination.
                var request = WebRequest.Create(m_source);
                request.Method = "GET";
                request.ContentType = Mimetype.Type;
                request.ContentLength = 0;
                var state = new RequestState
                {
                    Request = request,
                    Stream = request.GetRequestStream(),
                    Start= Scheduler.Now
                };
                state.Request.BeginGetResponse(ResponseReceived, state);
            }
        }

        private void ResponseReceived(IAsyncResult ar)
        {
            var elapsed = TimeSpan.Zero;

            var state = ar.AsyncState as RequestState;
            if (state != null)
            {
                var response = state.Request.EndGetResponse(ar);
                state.Reponse = response;
                using (var stream = state.Reponse.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            if (response.ContentType == OscPaylaodMimeType.Json.Type)
                            {
                                var ret = reader.ReadToEnd();

                                if (!string.IsNullOrWhiteSpace(ret))
                                {
                                    Scheduler.Schedule(() => ParseAndDeliver(ret));
                                }
                            }
                            else
                            {
                                using (var dst = new MemoryStream())
                                {
                                    stream.CopyTo(dst);
                                    dst.Flush();
                                    var data = dst.ToArray();
                                    Scheduler.Schedule(() => DeSerializerAndDeliver(data));
                                }
                            }
                        }
                    }
                }
                state.Reponse.Close();


                elapsed = Scheduler.Now - state.Start;
            }

            if (m_running)
            {
                if (elapsed > PollingInterval)
                {
                    // too long elapsed, go for another poll asap
                    Scheduler.Schedule(PollAndDeliver);
                }
                else
                {
                    // get ready for another sample.
                    var next = PollingInterval - elapsed;
                    m_pending = Observable.Timer(next, Scheduler).Subscribe(_ => PollAndDeliver());
                }
            }
        }

        private void DeSerializerAndDeliver(byte[] data)
        {
            OscPacket response;
            try
            {
                response = OscPacket.FromByteArray(data);
            }
            catch (Exception e)
            {
                response = null;
            }
            if (response != null)
            {
                ForwardPacket(response);
            }
        }

        private void ParseAndDeliver(string ret)
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
                ForwardPacket(response);
            }
        }



        #region Nested type: RequestState

        private class RequestState
        {
            public Stream Stream { get; set; }
            public WebRequest Request { get; set; }
            public WebResponse Reponse { get; set; }

            public byte[] Data { get; set; }

            public DateTimeOffset Start { get; set; }
        }

        #endregion
    }
}