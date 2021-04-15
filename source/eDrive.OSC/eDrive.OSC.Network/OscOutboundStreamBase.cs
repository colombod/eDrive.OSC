using System;
using System.Reactive.Concurrency;
using eDrive.Osc;
using eDrive.Core;

namespace eDrive.Network
{
    /// <summary>
    ///     Abstract core class for <see cref="IOscOutboundStream" />
    /// </summary>
    public abstract class OscOutboundStreamBase : IOscOutboundStream
    {
        protected OscOutboundStreamBase(IScheduler scheduler)
        {
            Scheduler = scheduler ?? TaskPoolScheduler.Default;
        }

        /// <summary>
        ///     Gets the scheduler.
        /// </summary>
        /// <value>
        ///     The scheduler.
        /// </value>
        public IScheduler Scheduler { get; private set; }

        public void OnNext(OscPacket value)
        {
            DeliverPacket(value);
        }

        public void OnError(Exception error)
        {
            DeliverError(error);
        }

        public void OnCompleted()
        {
            DeliverCompletion();
        }

        public abstract void Dispose();

        protected abstract void DeliverPacket(OscPacket value);

        protected virtual void DeliverError(Exception error)
        {
        }

        protected virtual void DeliverCompletion()
        {
        }
    }
}