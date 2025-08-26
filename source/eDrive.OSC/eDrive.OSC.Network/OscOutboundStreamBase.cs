using eDrive.OSC.Interfaces;

using System;
using System.Reactive.Concurrency;

namespace eDrive.OSC.Network
{
    /// <summary>
    ///     Abstract core class for <see cref="IOscOutboundStream" />
    /// </summary>
    public abstract class OscOutboundStreamBase(IScheduler scheduler) : IOscOutboundStream
    {
        /// <summary>
        ///     Gets the scheduler.
        /// </summary>
        /// <value>
        ///     The scheduler.
        /// </value>
        public IScheduler Scheduler { get; private set; } = scheduler ?? TaskPoolScheduler.Default;

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