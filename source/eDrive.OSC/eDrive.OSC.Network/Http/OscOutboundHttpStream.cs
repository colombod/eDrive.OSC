using System;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace eDrive.OSC.Network.Http;

public class OscOutboundHttpStream : OscOutboundStreamBase
{
    private readonly string m_destination;
    private readonly IObserver<OscPacket> m_responseStream;
    private readonly HttpClient m_httpClient;

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
        m_httpClient = new HttpClient();
    }

    public OscPaylaodMimeType Mimetype { get; }

    public override void Dispose()
    {
        m_httpClient?.Dispose();
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

            Scheduler.Schedule(async () =>
            {
                try
                {
                    using var content = new ByteArrayContent(postData);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Mimetype.Type);

                    var response = await m_httpClient.PostAsync(m_destination, content);

                    if (m_responseStream != null)
                    {
                        await ProcessResponse(response);
                    }
                }
                catch (Exception)
                {
                    // Handle HTTP errors gracefully
                }
            });
        }
    }

    private async Task ProcessResponse(HttpResponseMessage response)
    {
        try
        {
            var contentType = response.Content.Headers.ContentType?.MediaType;

            if (contentType == OscPaylaodMimeType.Json.Type)
            {
                var ret = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(ret))
                {
                    Scheduler.Schedule(() => ParseAndDeliver(ret, m_responseStream));
                }
            }
            else
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                Scheduler.Schedule(() => DeSerializerAndDeliver(data, m_responseStream));
            }
        }
        catch (Exception)
        {
            // Handle response processing errors gracefully
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

}