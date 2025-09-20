using System;

namespace eDrive.OSC.Interfaces;

/// <summary>
///     Defines a contract for receiving and processing incoming OSC data from various transport layers.
///     This interface provides reactive streams for different types of OSC packets and lifecycle management.
/// </summary>
/// <remarks>
///     <para>
///     Implementations of this interface handle the reception of OSC data from network transports
///     (UDP, TCP, HTTP), named pipes, or other communication channels. The interface uses reactive
///     streams (IObservable) to provide a push-based model for handling incoming OSC data.
///     </para>
///     <para>
///     The interface provides three specialized streams to allow consumers to subscribe to specific
///     types of OSC data:
///     <list type="bullet">
///         <item><description><see cref="MessageStream"/>: Individual OSC messages only</description></item>
///         <item><description><see cref="BundleStream"/>: OSC bundles only</description></item>
///         <item><description><see cref="PacketStream"/>: All OSC packets (messages and bundles)</description></item>
///     </list>
///     </para>
///     <para>
///     Typical usage pattern:
///     <code>
///     using var inboundStream = new OscInboundUdpStream(localPort);
///     inboundStream.MessageStream.Subscribe(msg => Console.WriteLine($"Received: {msg.Address}"));
///     inboundStream.Start();
///     // Process messages...
///     inboundStream.Stop();
///     </code>
///     </para>
/// </remarks>
public interface IOscInboundStream : IDisposable
{
    /// <summary>
    ///     Gets a reactive stream that emits only OSC messages (not bundles).
    ///     Subscribe to this stream when you're only interested in individual addressable method calls.
    /// </summary>
    /// <value>
    ///     An observable sequence of <see cref="OscMessage"/> objects representing individual OSC method calls.
    /// </value>
    /// <remarks>
    ///     <para>
    ///     This stream filters out bundle packets and only emits individual messages.
    ///     Messages from bundles will also appear in this stream after being extracted from their containers.
    ///     </para>
    ///     <para>
    ///     Use this stream when your application logic only needs to process individual method calls
    ///     and doesn't need to be aware of bundle timing or grouping.
    ///     </para>
    /// </remarks>
    IObservable<OscMessage> MessageStream { get; }

    /// <summary>
    ///     Gets a reactive stream that emits all incoming OSC packets, including both messages and bundles.
    ///     Subscribe to this stream when you need to handle all OSC data with full type awareness.
    /// </summary>
    /// <value>
    ///     An observable sequence of <see cref="OscPacket"/> objects representing all incoming OSC data.
    /// </value>
    /// <remarks>
    ///     <para>
    ///     This is the most comprehensive stream, providing access to all incoming OSC data in its original form.
    ///     Subscribers will receive both individual messages and complete bundles, maintaining the structure
    ///     and timing information as sent by the OSC client.
    ///     </para>
    ///     <para>
    ///     Use this stream when you need complete control over OSC packet processing, including handling
    ///     bundle timing, maintaining message grouping, or implementing custom routing logic.
    ///     </para>
    /// </remarks>
    IObservable<OscPacket> PacketStream { get; }

    /// <summary>
    ///     Gets a reactive stream that emits only OSC bundles (not individual messages).
    ///     Subscribe to this stream when you need to handle synchronized message groups with timing information.
    /// </summary>
    /// <value>
    ///     An observable sequence of <see cref="OscBundle"/> objects representing grouped OSC operations.
    /// </value>
    /// <remarks>
    ///     <para>
    ///     This stream filters out individual message packets and only emits complete bundles.
    ///     Each bundle contains timing information and may include multiple messages and nested bundles
    ///     that should be executed together.
    ///     </para>
    ///     <para>
    ///     Use this stream when your application needs to handle synchronized operations,
    ///     implement precise timing control, or process grouped operations as atomic units.
    ///     </para>
    /// </remarks>
    IObservable<OscBundle> BundleStream { get; }

    /// <summary>
    ///     Begins listening for incoming OSC data on the configured transport.
    ///     Call this method to activate the inbound stream and start receiving OSC packets.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     This method initializes the underlying transport (network socket, named pipe, etc.)
    ///     and begins the process of receiving and parsing incoming OSC data. Once started,
    ///     the reactive streams will begin emitting OSC packets as they arrive.
    ///     </para>
    ///     <para>
    ///     The method is typically non-blocking and returns immediately. OSC data processing
    ///     occurs asynchronously in the background, with packets delivered through the
    ///     reactive stream subscriptions.
    ///     </para>
    ///     <para>
    ///     <strong>Thread Safety:</strong> This method should be safe to call from any thread,
    ///     but calling it multiple times without calling <see cref="Stop"/> first may have
    ///     undefined behavior depending on the implementation.
    ///     </para>
    /// </remarks>
    void Start();

    /// <summary>
    ///     Stops listening for incoming OSC data and deactivates the inbound stream.
    ///     Call this method to gracefully shutdown the stream and release transport resources.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     This method closes the underlying transport connection and stops processing incoming data.
    ///     Any pending data in transport buffers may be lost. The reactive streams will complete
    ///     and stop emitting new packets.
    ///     </para>
    ///     <para>
    ///     After calling this method, the stream can typically be restarted by calling <see cref="Start"/>
    ///     again, though this depends on the specific implementation and transport type.
    ///     </para>
    ///     <para>
    ///     <strong>Resource Management:</strong> This method should release transport resources
    ///     like network sockets or file handles, but may not release all resources. For complete
    ///     cleanup, call <see cref="IDisposable.Dispose"/> after stopping.
    ///     </para>
    /// </remarks>
    void Stop();
}