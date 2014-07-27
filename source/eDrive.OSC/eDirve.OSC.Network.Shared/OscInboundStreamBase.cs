using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using eDrive.Osc;
using eDrive.Core;


namespace eDrive.Network
{
    /// <summary>
    ///     Abstract core class for <see cref="IOscInboundStream" />
    /// </summary>
    public abstract class OscInboundStreamBase : IOscInboundStream
    {
        private readonly Subject<OscBundle> m_bundleStream;
        private readonly Subject<OscMessage> m_messageStream;
        private readonly Subject<OscPacket> m_pakectStream;
        private readonly IScheduler m_scheduler;

        protected OscInboundStreamBase(IScheduler scheduler)
        {
            m_scheduler = scheduler ?? TaskPoolScheduler.Default;
            m_pakectStream = new Subject<OscPacket>();
            m_messageStream = new Subject<OscMessage>();
            m_bundleStream = new Subject<OscBundle>();
        }

        /// <summary>
        ///     Gets the scheduler.
        /// </summary>
        /// <value>
        ///     The scheduler.
        /// </value>
        public IScheduler Scheduler
        {
            get { return m_scheduler; }
        }

        public virtual void Dispose()
        {
            m_pakectStream.OnCompleted();
            m_bundleStream.OnCompleted();
            m_messageStream.OnCompleted();
        }

        public IObservable<OscMessage> MessageStream
        {
            get { return m_messageStream; }
        }

        public IObservable<OscPacket> PacketStream
        {
            get { return m_pakectStream; }
        }

        public IObservable<OscBundle> BundleStream
        {
            get { return m_bundleStream; }
        }

        public bool SuppressParsingExceptions { get; set; }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        public abstract void Start();

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public abstract void Stop();

        
        /// <summary>
        ///     Forwards the bundle.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected void ForwardBundle(OscBundle bundle)
        {
            InternalFowrardBundle(bundle, true);
        }

        private void InternalFowrardBundle(OscBundle bundle, bool forward)
        {
            if (bundle != null)
            {
                if (forward)
                {
                    Scheduler.Schedule(bundle, (b, action) => m_bundleStream.OnNext(b));
                }

                if (bundle.Messages != null
                    && bundle.Messages.Count > 0)
                {
                    foreach (var message in bundle.Messages)
                    {
                        ForwardMessage(message);
                    }
                }

                if (bundle.Bundles != null
                    && bundle.Bundles.Count > 0)
                {
                    foreach (var b in bundle.Bundles)
                    {
                        if (b.TimeStamp.TimeTag == OscTimeTag.Immediate)
                        {
                            InternalFowrardBundle(b, false);
                        }
                        else
                        {
                            var delay = b.TimeStamp.DateTime - m_scheduler.Now;
                            var ob = b;
                            m_scheduler.Schedule(delay, () => InternalFowrardBundle(ob, false));
                        }
                    }
                }
            }
        }

        protected void ForwardPacket(OscPacket packet)
        {
            if (packet != null)
            {
                Scheduler.Schedule(packet, (b, action) => m_pakectStream.OnNext(b));

                var msg = packet as OscMessage;

                if (msg != null)
                {
                    ForwardMessage(msg);
                }
                else
                {
                    var bundle = packet as OscBundle;
                    if (bundle != null)
                    {
                        ForwardBundle(bundle);
                    }
                }
            }
        }

        /// <summary>
        ///     Forwards the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void ForwardMessage(OscMessage message)
        {
            if (message != null)
            {
                Scheduler.Schedule(message, (m, action) => m_messageStream.OnNext(m));
            }
        }
    }
}