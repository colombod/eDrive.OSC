// File :		IOscOutboundStream.cs
// Copyright :  	2012-2014 Diego Colombo.
// Created : 		04-2014

#region

using System;
using eDrive.Osc;

#endregion

namespace eDrive.Core
{
    /// <summary>
    ///     Contract for outbound stream.
    /// </summary>
    public interface IOscOutboundStream : IObserver<OscPacket>,IDisposable
    {
        /// <summary>
        ///     Creates the string stream.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IObserver<string> CreateStringStream(string method);

        /// <summary>
        ///     Creates the int stream.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IObserver<int> CreateIntStream(string method);

        /// <summary>
        ///     Creates the long stream.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IObserver<long> CreateLongStream(string method);

        /// <summary>
        ///     Creates the bool stream.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IObserver<bool> CreateBoolStream(string method);

        /// <summary>
        ///     Creates the double stream.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IObserver<double> CreateDoubleStream(string method);

        /// <summary>
        ///     Creates the float stream.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IObserver<float> CreateFloatStream(string method);

        /// <summary>
        ///     Creates the char stream.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IObserver<char> CreateCharStream(string method);

        /// <summary>
        ///     Creates the raw stream.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IObserver<byte[]> CreateRawStream(string method);

        /// <summary>
        /// Creates the midi stream.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        IObserver<OscMidiMessage> CreateMidiStream(string method);
    }
}