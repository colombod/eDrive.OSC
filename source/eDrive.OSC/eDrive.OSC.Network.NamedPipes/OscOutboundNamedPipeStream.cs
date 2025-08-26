using System.Diagnostics;
using System.IO.Pipes;
using System.Reactive.Concurrency;
using System.Runtime.Versioning;

namespace eDrive.OSC.Network.NamedPipes;

/// <summary>
///     Named pipe based stream
/// </summary>
[SupportedOSPlatform("windows")]
public class OscOutboundNamedPipeStream : OscOutboundStreamBase
{
    private readonly string m_destinationPipeName;
    private readonly NamedPipeClientStream m_pipe;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OscOutboundNamedPipeStream" /> class.
    /// </summary>
    /// <param name="destinationPipeName">Name of the pipe.</param>
    /// <param name="scheduler">The scheduler.</param>
    public OscOutboundNamedPipeStream(string destinationPipeName, IScheduler scheduler = null)
        : base(scheduler)
    {
        m_destinationPipeName = destinationPipeName;
        m_pipe = new NamedPipeClientStream(".", m_destinationPipeName, PipeDirection.Out, PipeOptions.Asynchronous);
        var pid = Process.GetCurrentProcess().Id;
        m_pipe.Connect(2000);
        m_pipe.ReadMode = PipeTransmissionMode.Message;


        m_pipe.WaitForPipeDrain();
        lock (m_pipe)
        {
            m_pipe.Write(BitConverter.GetBytes(-1), 0, 4);
            m_pipe.Write(BitConverter.GetBytes(pid), 0, 4);
        }
    }

    protected override void DeliverPacket(OscPacket packet)
    {
        if (m_pipe.IsConnected
            && packet != null)
        {
            Scheduler.Schedule(packet,
                (oscPacket, action) =>
                {
                    byte[] data;
                    try
                    {
                        var packetData = oscPacket.ToByteArray();
                        using (var ms = new MemoryStream())
                        {
                            ms.Write(BitConverter.GetBytes(packetData.Length), 0, 4);
                            ms.Write(packetData, 0, packetData.Length);
                            data = ms.ToArray();
                        }
                    }
                    catch (Exception)
                    {
                        // Handle serialization errors gracefully
                        data = new byte[0];
                    }
                    if (data.Length > 0)
                    {
                        m_pipe.WaitForPipeDrain();

                        m_pipe.BeginWrite(data, 0, data.Length, DataWriteFinished, null);
                    }
                });
        }
    }

    private void DataWriteFinished(IAsyncResult ar)
    {
        m_pipe.EndWrite(ar);
    }


    public override void Dispose()
    {
        m_pipe.Dispose();
    }
}