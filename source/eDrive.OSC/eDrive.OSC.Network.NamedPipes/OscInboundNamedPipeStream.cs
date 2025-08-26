using System.Diagnostics;
using System.IO.Pipes;
using System.Reactive.Concurrency;

namespace eDrive.OSC.Network.NamedPipes;

/// <summary>
/// </summary>
public class OscInboundNamedPipeStream : OscInboundStreamBase
{
    private readonly List<AsynchReader> m_clients;
    private readonly string m_pipeName;
    private NamedPipeServerStream m_pipeServer;
    private bool m_running;

    /// <summary>
    ///     Initializes a new instance of the <see cref="OscInboundNamedPipeStream" /> class.
    /// </summary>
    /// <param name="pipeName">Name of the pipe.</param>
    /// <param name="scheduler">The scheduler.</param>
    public OscInboundNamedPipeStream(string pipeName, IScheduler scheduler = null)
        : base(scheduler)
    {
        m_pipeName = pipeName;
        m_clients = new List<AsynchReader>();
    }

    public override void Start()
    {
        m_running = true;
        Scheduler.Schedule(WaitIncoming);
        Scheduler.Schedule(TimeSpan.FromSeconds(2),
            action =>
            {
                var count = 0;
                lock (m_clients)
                {
                    if (m_clients.Count > 0)
                    {
                        var todelete = m_clients.Where(c => !c.IsConnected).ToList();
                        foreach (var client in todelete)
                        {
                            RemoveReader(client);
                        }

                        count = m_clients.Count;
                    }
                }

                if (m_running)
                {
                    action(TimeSpan.FromSeconds(count > 0 ? 2 : 20));
                }
            }
        );
    }

    private void WaitIncoming()
    {
        m_pipeServer = new NamedPipeServerStream(m_pipeName,
            PipeDirection.InOut,
            NamedPipeServerStream.MaxAllowedServerInstances,
            PipeTransmissionMode.Message,
            PipeOptions.Asynchronous);
        m_pipeServer.BeginWaitForConnection(Connected, m_pipeServer);
    }

    private void Connected(IAsyncResult ar)
    {
        m_pipeServer.EndWaitForConnection(ar);
        var srv = ar.AsyncState as NamedPipeServerStream;
        if (srv != null)
        {
            Scheduler.Schedule(() => StartRead(srv));
        }

        if (m_running)
        {
            Scheduler.Schedule(WaitIncoming);
        }
    }

    private void StartRead(NamedPipeServerStream srv)
    {
        var ar = new AsynchReader(srv, m => Scheduler.Schedule(() => ForwardPacket(m)), RemoveReader);
        m_clients.Add(ar);
        ar.Start();
    }

    private void RemoveReader(AsynchReader reader)
    {
        lock (m_clients)
        {
            m_clients.Remove(reader);
        }

        reader.Dispose();
    }

    public override void Stop()
    {
        m_running = false;
    }

    public override void Dispose()
    {
        // dispose local

        Stop();
        m_pipeServer.Dispose();

        base.Dispose();
    }

    #region Nested type: AsynchReader

    private class AsynchReader : IDisposable
    {
        private readonly Action<OscPacket> m_deliveryAction;
        private readonly Action<AsynchReader> m_droppedConnection;
        private readonly NamedPipeServerStream m_srv;
        private Process m_process;

        public AsynchReader
            (NamedPipeServerStream srv, Action<OscPacket> deliveryAction, Action<AsynchReader> droppedConnection)
        {
            m_srv = srv;

            m_deliveryAction = deliveryAction;
            m_droppedConnection = droppedConnection;
        }

        public bool IsConnected => m_srv.IsConnected;

        #region IDisposable Members

        public void Dispose()
        {
            m_srv.Dispose();
        }

        #endregion

        public void Start()
        {
            ReadHeader();
        }

        private void ReadHeader()
        {
            if (!m_srv.IsConnected)
            {
                m_droppedConnection(this);
                return;
            }

            var buffer = new byte[4];
            try
            {
                m_srv.BeginRead(buffer, 0, 4, SizeReadCompleted, buffer);
            }
            catch
            {
                m_droppedConnection(this);
            }
        }

        private void SizeReadCompleted(IAsyncResult ar)
        {
            var read = m_srv.EndRead(ar);
            if (read < 4)
            {
                if (m_srv.IsConnected)
                {
                    ReadHeader();
                }
                m_droppedConnection(this);
                return;
            }

            var size = ar.AsyncState is byte[] buffer ? BitConverter.ToInt32(buffer, 0) : 0;


            if (size != 0)
            {
                if (size == -1)
                {
                    buffer = new byte[4];
                    // registering process id
                    m_srv.BeginRead(buffer, 0, 4, ProcessIdReadCompleted, buffer);
                }
                else
                {
                    try
                    {
                        buffer = new byte[size];
                        if (!m_srv.IsConnected)
                        {
                            m_droppedConnection(this);
                            return;
                        }

                        var state = new BodyReadState
                        {
                            Data = buffer,
                            Offset = 0
                        };
                        m_srv.BeginRead(state.Data, state.Offset, state.Data.Length, BodyReadCompleted, state);
                    }

                    catch
                    {
                        m_droppedConnection(this);
                    }
                }
            }
        }

        private void ProcessIdReadCompleted(IAsyncResult ar)
        {
            m_srv.EndRead(ar);
            var buffer = ar.AsyncState as byte[];
            var pid = buffer != null ? BitConverter.ToInt32(buffer, 0) : 0;

            if (pid != 0)
            {
                m_process = Process.GetProcessById(pid);
                m_process.EnableRaisingEvents = true;
                m_process.Exited += ProcessExited;

                ReadHeader();
            }
            else
            {
                m_droppedConnection(this);
            }
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            m_droppedConnection(this);
        }

        private void BodyReadCompleted(IAsyncResult ar)
        {
            var read = m_srv.EndRead(ar);
            var state = ar.AsyncState as BodyReadState;


            if (state != null)
            {
                state.Offset += read;
                if (state.Offset == state.Data.Length)
                {
                    OscPacket p = null;
                    try
                    {
                        p = OscPacket.FromByteArray(state.Data);
                    }
                    catch (Exception)
                    {
                        // Handle deserialization errors gracefully
                        p = null;
                    }

                    if (p != null)
                    {
                        m_deliveryAction(p);
                    }
                }
                else
                {
                    m_srv.BeginRead(state.Data,
                        state.Offset,
                        state.Data.Length - state.Offset,
                        BodyReadCompleted,
                        state);
                }
            }

            ReadHeader();
        }

        #region Nested type: BodyReadState

        private class BodyReadState
        {
            public byte[] Data { get; set; }
            public int Offset { get; set; }
        }

        #endregion
    }

    #endregion
}