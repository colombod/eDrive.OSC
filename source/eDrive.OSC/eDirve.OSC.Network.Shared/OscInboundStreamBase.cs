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
		private readonly Subject<OscBundle> _mBundleStream;
		private readonly Subject<OscMessage> _mMessageStream;
		private readonly Subject<OscPacket> _mPakectStream;
		private readonly IScheduler _mScheduler;

		protected OscInboundStreamBase(IScheduler scheduler)
		{
			_mScheduler = scheduler ?? TaskPoolScheduler.Default;
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
		public IScheduler Scheduler
		{
			get { return _mScheduler; }
		}

		#region IOscInboundStream Members

		public virtual void Dispose()
		{
			_mPakectStream.OnCompleted();
			_mBundleStream.OnCompleted();
			_mMessageStream.OnCompleted();
		}

		public IObservable<OscMessage> MessageStream
		{
			get { return _mMessageStream; }
		}

		public IObservable<OscPacket> PacketStream
		{
			get { return _mPakectStream; }
		}

		public IObservable<OscBundle> BundleStream
		{
			get { return _mBundleStream; }
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

		#endregion

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
							InternalFowrardBundle(b, false);
						}
						else
						{
							var delay = b.TimeStamp.DateTime - _mScheduler.Now;
							var ob = b;
							_mScheduler.Schedule(delay, () => InternalFowrardBundle(ob, false));
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
				Scheduler.Schedule(message, (m, action) => _mMessageStream.OnNext(m));
			}
		}
	}

}