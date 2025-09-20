using System;

namespace eDrive.OSC.Interfaces;

/// <summary>
///     Defines a contract for sending OSC packets to remote destinations through various transport layers.
///     This interface extends <see cref="IObserver{T}"/> to provide a reactive pattern for outbound OSC communication.
/// </summary>
/// <remarks>
///     <para>
///     Implementations of this interface handle the transmission of OSC data through different transport
///     mechanisms such as UDP, TCP, HTTP, named pipes, or other communication channels. The interface
///     follows the Observer pattern, allowing OSC packets to be pushed to the stream for delivery.
///     </para>
///     <para>
///     As an <see cref="IObserver{OscPacket}"/>, this interface can be used with reactive streams,
///     making it easy to connect inbound and outbound streams or to integrate with Rx.NET-based
///     data processing pipelines.
///     </para>
///     <para>
///     Typical usage patterns:
///     <code>
///     // Direct packet sending
///     using var outboundStream = new OscOutboundUdpStream(remoteEndpoint);
///     outboundStream.OnNext(new OscMessage("/test", 42));
///     
///     // Reactive pipeline
///     inboundStream.MessageStream
///         .Where(msg => msg.Address.StartsWith("/forward"))
///         .Subscribe(outboundStream);
///     </code>
///     </para>
///     <para>
///     <strong>Thread Safety:</strong> Implementations should be thread-safe to allow packet
///     transmission from multiple threads concurrently.
///     </para>
/// </remarks>
public interface IOscOutboundStream : IObserver<OscPacket>, IDisposable
{
}