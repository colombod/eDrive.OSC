using System;
using eDrive.Osc;

namespace eDrive.Core
{
	/// <summary>
	///     Contract for outbound stream.
	/// </summary>
	public interface IOscOutboundStream : IObserver<OscPacket>, IDisposable
	{
	}
}