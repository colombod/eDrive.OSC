using eDrive.OSC.Interfaces;

using System;
using System.Reactive;

namespace eDrive.OSC.Network
{
    /// <summary>
    ///     Extension for easy creation of  osc method bound streams
    /// </summary>
    public static class OscOutboundStreamExtensions
    {
        /// <summary>
        ///     Delivers the value to method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <param name="value">The value.</param>
        private static void DeliverValueToMethod<T>(this IObserver<OscPacket> stream, string method, T value)
        {
            var msg = new OscMessage(method);
            msg.Append(value);
            stream.OnNext(msg);
        }

        /// <summary>
        ///     Creates the string stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IObserver<string> CreateStringStream(this IOscOutboundStream stream, string method)
        {
            return Observer.Create<string>(payload => stream.DeliverValueToMethod(method, payload));
        }

        /// <summary>
        ///     Creates the int stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IObserver<int> CreateIntStream(this IOscOutboundStream stream, string method)
        {
            return Observer.Create<int>(payload => stream.DeliverValueToMethod(method, payload));
        }

        /// <summary>
        ///     Creates the long stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IObserver<long> CreateLongStream(this IOscOutboundStream stream, string method)
        {
            return Observer.Create<long>(payload => stream.DeliverValueToMethod(method, payload));
        }

        /// <summary>
        ///     Creates the bool stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IObserver<bool> CreateBoolStream(this IOscOutboundStream stream, string method)
        {
            return Observer.Create<bool>(payload => stream.DeliverValueToMethod(method, payload));
        }

        /// <summary>
        ///     Creates the double stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IObserver<double> CreateDoubleStream(this IOscOutboundStream stream, string method)
        {
            return Observer.Create<double>(payload => stream.DeliverValueToMethod(method, payload));
        }

        /// <summary>
        ///     Creates the float stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IObserver<float> CreateFloatStream(this IOscOutboundStream stream, string method)
        {
            return Observer.Create<float>(payload => stream.DeliverValueToMethod(method, payload));
        }

        /// <summary>
        ///     Creates the char stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IObserver<char> CreateCharStream(this IOscOutboundStream stream, string method)
        {
            return Observer.Create<char>(payload => stream.DeliverValueToMethod(method, payload));
        }

        /// <summary>
        ///     Creates the raw stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IObserver<byte[]> CreateRawStream(this IOscOutboundStream stream, string method)
        {
            return Observer.Create<byte[]>(payload => stream.DeliverValueToMethod(method, payload));
        }

        /// <summary>
        ///     Creates the midi stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static IObserver<OscMidiMessage> CreateMidiStream(this IOscOutboundStream stream, string method)
        {
            return Observer.Create<OscMidiMessage>(payload => stream.DeliverValueToMethod(method, payload));
        }
    }
}