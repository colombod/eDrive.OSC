using System;
using eDrive.Osc;

namespace eDrive.Core
{
    /// <summary>
    /// </summary>
    public interface IOscInboundStream : IDisposable
    {
        /// <summary>
        /// Gets the message stream.
        /// Only <see cref="OscMessage"/> are streamed here.
        /// </summary>
        /// <value>
        /// The message stream.
        /// </value>
        IObservable<OscMessage> MessageStream { get; }

        /// <summary>
        /// Gets the packet stream.
        /// All <see cref="OscPacket"/> are streamed here.
        /// </summary>
        /// <value>
        /// The packet stream.
        /// </value>
        IObservable<OscPacket> PacketStream { get; }

        /// <summary>
        /// Gets the bundle stream.
        /// Only <see cref="OscBundle"/> are streamed here.
        /// </summary>
        /// <value>
        /// The bundle stream.
        /// </value>
        IObservable<OscBundle> BundleStream { get; }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        void Stop();
    }
}