using System;
using System.IO;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using eDrive.Osc;

namespace eDrive.Network.Http
{
    public class OscInboundHttpStream : OscInboundStreamBase
    {
        private readonly string m_source;
        private readonly TimeSpan m_pollingInterval;
        private volatile bool m_running;
        private IDisposable m_pending;
        private readonly HttpClient m_httpClient;

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
            m_httpClient = new HttpClient();
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

        public override void Dispose()
        {
            Stop();
            m_httpClient?.Dispose();
            base.Dispose();
        }

        private void PollAndDeliver()
        {
            m_pending = null;
            if (m_running)
            {
                var start = Scheduler.Now;
                Scheduler.Schedule(async () =>
                {
                    try
                    {
                        var response = await m_httpClient.GetAsync(m_source);
                        await ProcessResponse(response, start);
                    }
                    catch (Exception)
                    {
                        // Handle HTTP errors gracefully
                        ScheduleNextPoll(Scheduler.Now - start);
                    }
                });
            }
        }

        private async Task ProcessResponse(HttpResponseMessage response, DateTimeOffset start)
        {
            try
            {
                var contentType = response.Content.Headers.ContentType?.MediaType;
                
                if (contentType == OscPaylaodMimeType.Json.Type)
                {
                    var ret = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(ret))
                    {
                        Scheduler.Schedule(() => ParseAndDeliver(ret));
                    }
                }
                else
                {
                    var data = await response.Content.ReadAsByteArrayAsync();
                    Scheduler.Schedule(() => DeSerializerAndDeliver(data));
                }
            }
            catch (Exception)
            {
                // Handle response processing errors gracefully
            }
            finally
            {
                var elapsed = Scheduler.Now - start;
                ScheduleNextPoll(elapsed);
            }
        }

        private void ScheduleNextPoll(TimeSpan elapsed)
        {
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



    }
}