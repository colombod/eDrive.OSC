using eDrive.OSC.Interfaces;

using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace eDrive.OSC.Network
{
    /// <summary>
    ///     Abstract core class for <see cref="IOscInboundStream" />
    /// </summary>
    public abstract class OscInboundStreamBase : IOscInboundStream
    {
        private readonly Subject<OscBundle> _mBundleStream;
        private readonly Subject<OscMessage> _mMessageStream;
        private readonly Subject<OscPacket> _mPakectStream;

        protected OscInboundStreamBase(IScheduler scheduler)
        {
            Scheduler = scheduler ?? TaskPoolScheduler.Default;
            _mPakectStream = new Subject<OscPacket>();
            _mMessageStream = new Subject<OscMessage>();
            _mBundleStream = new Subject<OscBundle>();
        }

        /// <summary>
        ///     Gets the scheduler.
        /// </summary>
        /// <value>
        ///     The scheduler.
        /// </value>
        public IScheduler Scheduler { get; }

        #region IOscInboundStream Members

        public virtual void Dispose()
        {
            _mPakectStream.OnCompleted();
            _mBundleStream.OnCompleted();
            _mMessageStream.OnCompleted();
        }

        public IObservable<OscMessage> MessageStream => _mMessageStream;

        public IObservable<OscPacket> PacketStream => _mPakectStream;

        public IObservable<OscBundle> BundleStream => _mBundleStream;

        public bool SuppressParsingExceptions { get; set; }

        /// <summary>
        ///     Starts this instance.
        /// </summary>
        public abstract void Start();

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public abstract void Stop();

        #endregion

        /// <summary>
        ///     Forwards the bundle.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected void ForwardBundle(OscBundle bundle)
        {
            InternalForwardBundle(bundle, true);
        }

        private void InternalForwardBundle(OscBundle bundle, bool forward)
        {
            if (bundle != null)
            {
                if (forward)
                {
                    Scheduler.Schedule(bundle, (b, action) => _mBundleStream.OnNext(b));
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
                            InternalForwardBundle(b, false);
                        }
                        else
                        {
                            var delay = b.TimeStamp.DateTime - Scheduler.Now;
                            var ob = b;
                            Scheduler.Schedule(delay, () => InternalForwardBundle(ob, false));
                        }
                    }
                }
            }
        }

        protected void ForwardPacket(OscPacket packet)
        {
            if (packet != null)
            {
                Scheduler.Schedule(packet, (b, action) => _mPakectStream.OnNext(b));

                if (packet is OscMessage msg)
                {
                    ForwardMessage(msg);
                }
                else
                {
                    if (packet is OscBundle bundle)
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
                Scheduler.Schedule(message, (m, action) => _mMessageStream.OnNext(m));
            }
        }
    }

}