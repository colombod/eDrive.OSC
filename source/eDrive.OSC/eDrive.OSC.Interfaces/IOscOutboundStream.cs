using System;

namespace eDrive.OSC.Interfaces;

/// <summary>
///     Contract for outbound stream.
/// </summary>
public interface IOscOutboundStream : IObserver<OscPacket>, IDisposable
{
}