// File :		OscTimeTag.cs
// Copyright :  	2012-2013 mUBreeze ltd.
// Created : 		06-2013

#region

using System;
using System.Globalization;

#endregion

namespace eDrive.Osc.Tests.ReferenceProtocol
{
    /// <summary>
    ///     Represents an Osc Time Tag.
    /// </summary>
    public class OscTimeTag
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscTimeTag" /> class.
        /// </summary>
        /// <remarks>Defaults the Osc Time Tag value to "immediate"</remarks>
        public OscTimeTag() : this(DateTime.MinValue)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscTimeTag" /> class.
        /// </summary>
        /// <param name="timeStamp">The time stamp to use to set the Osc Time Tag.</param>
        public OscTimeTag(DateTime timeStamp)
        {
            if (timeStamp == DateTime.MinValue)
            {
                Delay = TimeSpan.Zero;
                SecondsSinceEpoch = 0;
                FractionalSecond = 1;
            }
            else
            {
                timeStamp = new DateTime(timeStamp.Ticks - (timeStamp.Ticks%TimeSpan.TicksPerMillisecond),
                                         timeStamp.Kind);
                ulong sec;
                ulong fract;

                ComputeParts(timeStamp, out sec, out fract);
                SecondsSinceEpoch = (uint) sec;
                FractionalSecond = (uint) fract;

                m_timeStamp = timeStamp;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscTimeTag" /> class.
        /// </summary>
        /// <param name="data">The time stamp to use to set the Osc Time Tag.</param>
        /// <param name="offset">The offset.</param>
        public OscTimeTag(byte[] data, int offset = 0)
        {
            Assert.IsTrue((data.Length - offset) >= 8);
            m_timeStamp = GetNetworkTime(data, offset);
        }

        #endregion

        #region Public properties

        /// <summary>
        ///     Gets a value indicating whether this instance is an "immediate" time tag.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is an "immediate" time tag; otherwise, <c>false</c>.
        /// </value>
        public bool IsImmediate
        {
            get { return DateTime == DateTime.MinValue && FractionalSecond == 1; }
        }

        // TODO by GR: do we need this? Hide or compute from the DateTime property
        /// <summary>
        ///     Gets the first 32 bits of the Osc Time Tag. Specifies the number of seconds since the epoch.
        /// </summary>
        private uint SecondsSinceEpoch { get; set; }

        // TODO by GR: do we need this? Hide or compute from the DateTime property
        /// <summary>
        ///     Gets the last 32 bits of the Osc Time Tag. Specifies the fractional part of a second.
        /// </summary>
        private uint FractionalSecond { get; set; }

        // TODO: this prop should be substituted with the Delay
        /// <summary>
        ///     Gets the Osc Time Tag as a DateTime value.
        /// </summary>
        public DateTime DateTime
        {
            get { return m_timeStamp; }
        }

        /// <summary>
        ///     Gets the delay.
        /// </summary>
        /// <value>
        ///     The delay.
        /// </value>
        public TimeSpan Delay { get; private set; }

        #endregion

        /// <summary>
        ///     Osc Time Epoch (January 1, 1900 00:00:00).
        /// </summary>
        private static readonly DateTime s_epoch = new DateTime(1900, 1, 1, 0, 0, 0, 0);

        #region Public methods

        /// <summary>
        ///     Minimum Osc Time Tag.
        /// </summary>
        /// <summary>
        ///     Convert the Osc Time Tag to a byte array.
        /// </summary>
        /// <returns>A byte array containing the Osc Time Tag.</returns>
        public byte[] ToByteArray()
        {
            /*
            var timeStamp = new List<byte>(8);

            var secondsSinceEpoch = BitConverter.GetBytes(SecondsSinceEpoch);
            var fractionalSecond = BitConverter.GetBytes(FractionalSecond);

          //  if (BitConverter.IsLittleEndian != OscPacket.LittleEndianByteOrder)
            {
                Utility.SwapEndianInPlace(ref secondsSinceEpoch);
                Utility.SwapEndianInPlace(ref fractionalSecond);
            }

            timeStamp.AddRange(secondsSinceEpoch);
            timeStamp.AddRange(fractionalSecond);

            return timeStamp.ToArray();
             */

            // GR
            //return ToNetworkTime(m_timeStamp);
            return ToNetworkTime();
        }

        /// <summary>
        ///     Determines whether two specified instances of OscTimeTag are equal.
        /// </summary>
        /// <param name="lhs">An OscTimeTag.</param>
        /// <param name="rhs">An OscTimeTag.</param>
        /// <returns>true if lhs and rhs represent the same time tag; otherwise, false.</returns>
        public static bool Equals(OscTimeTag lhs, OscTimeTag rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        ///     Determines whether two specified instances of OscTimeTag are equal.
        /// </summary>
        /// <param name="lhs">An OscTimeTag.</param>
        /// <param name="rhs">An OscTimeTag.</param>
        /// <returns>true if lhs and rhs represent the same time tag; otherwise, false.</returns>
        public static bool operator ==(OscTimeTag lhs, OscTimeTag rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (((object) lhs == null)
                || ((object) rhs == null))
            {
                return false;
            }

            return lhs.DateTime == rhs.DateTime;
        }

        /// <summary>
        ///     Determines whether two specified instances of OscTimeTag are not equal.
        /// </summary>
        /// <param name="lhs">An OscTimeTag.</param>
        /// <param name="rhs">An OscTimeTag.</param>
        /// <returns>true if lhs and rhs do not represent the same time tag; otherwise, false.</returns>
        public static bool operator !=(OscTimeTag lhs, OscTimeTag rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        ///     Determines whether one specified <see cref="OscTimeTag" /> is less than another specified <see cref="OscTimeTag" />.
        /// </summary>
        /// <param name="lhs">An OscTimeTag.</param>
        /// <param name="rhs">An OscTimeTag.</param>
        /// <returns>true if lhs is less than rhs; otherwise, false.</returns>
        public static bool operator <(OscTimeTag lhs, OscTimeTag rhs)
        {
            return lhs.DateTime < rhs.DateTime;
        }

        /// <summary>
        ///     Determines whether one specified <see cref="OscTimeTag" /> is less than or equal to another specified
        ///     <see
        ///         cref="OscTimeTag" />
        ///     .
        /// </summary>
        /// <param name="lhs">An OscTimeTag.</param>
        /// <param name="rhs">An OscTimeTag.</param>
        /// <returns>true if lhs is less than or equal to rhs; otherwise, false.</returns>
        public static bool operator <=(OscTimeTag lhs, OscTimeTag rhs)
        {
            return lhs.DateTime <= rhs.DateTime;
        }

        /// <summary>
        ///     Determines whether one specified <see cref="OscTimeTag" /> is greater than another specified <see cref="OscTimeTag" />.
        /// </summary>
        /// <param name="lhs">An OscTimeTag.</param>
        /// <param name="rhs">An OscTimeTag.</param>
        /// <returns>true if lhs is greater than rhs; otherwise, false.</returns>
        public static bool operator >(OscTimeTag lhs, OscTimeTag rhs)
        {
            return lhs.DateTime > rhs.DateTime;
        }

        /// <summary>
        ///     Determines whether one specified <see cref="OscTimeTag" /> is greater than or equal to another specified
        ///     <see
        ///         cref="OscTimeTag" />
        ///     .
        /// </summary>
        /// <param name="lhs">An OscTimeTag.</param>
        /// <param name="rhs">An OscTimeTag.</param>
        /// <returns>true if lhs is greater than or equal to rhs; otherwise, false.</returns>
        public static bool operator >=(OscTimeTag lhs, OscTimeTag rhs)
        {
            return lhs.DateTime >= rhs.DateTime;
        }

        //public static bool IsValidTime(DateTime timeStamp)
        //{
        //    return (timeStamp >= s_epoch + TimeSpan.FromMilliseconds(1.0));
        //}
        /// <summary>
        ///     Validates the time stamp for use in an Osc Time Tag.
        /// </summary>
        /// <param name="timeStamp">The time stamp to validate.</param>
        /// <returns>True if the time stamp is a valid Osc Time Tag; false, otherwise.</returns>
        /// <remarks>
        ///     Time stamps must be greater-than-or-equal to <see cref="OscTimeTag.MinValue" />.
        /// </remarks>
        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns>true if value is an instance of System.DateTime and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object value)
        {
            if (value == null)
            {
                return false;
            }

            var rhs = value as OscTimeTag;
            if (rhs == null)
            {
                return false;
            }

            return m_timeStamp.Equals(rhs.m_timeStamp);
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified OscTimeTag instance.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns>true if value is an instance of System.DateTime and equals the value of this instance; otherwise, false.</returns>
        public bool Equals(OscTimeTag value)
        {
            return (object) value != null && m_timeStamp.Equals(value.m_timeStamp);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return m_timeStamp.GetHashCode();
        }

        /// <summary>
        ///     Converts the value of the current <see cref="OscTimeTag" /> object to its equivalent string representation.
        /// </summary>
        /// <returns>
        ///     A string representation of the value of the current <see cref="OscTimeTag" /> object.
        /// </returns>
        public override string ToString()
        {
            return m_timeStamp.ToString(CultureInfo.InvariantCulture);
        }

        #region Private methods

        /// <summary>
        ///     Compute the 8-byte array, given the date
        /// </summary>
        /// <returns></returns>
        private byte[] ToNetworkTime() //private static byte[] ToNetworkTime(DateTime date)
        {
            var data = new byte[8];

            // GR: la ComputePart e' ridondante. Le due parti sono gia' state calcolate e salvate in SecondsSinceEpoch & FractionalSecond
            //ulong intpart, fractpart;
            //ComputeParts(date, out intpart, out fractpart);
            ulong intpart = SecondsSinceEpoch;
            ulong fractpart = FractionalSecond;
            // <----

            // todo : se sono gia in big endia basta scrivere i bytes!
            // qui assumo siano gia' big endian
            var temp = intpart;
            for (var i = 0; i < 4; i++)
            {
                data[i] = (byte) (temp%256);
                temp = temp/256;
            }

            temp = fractpart;
            for (var i = 4; i < 8; i++)
            {
                data[i] = (byte) (temp%256);
                temp = temp/256;
            }
            /*
            // prima del cambio
            ulong temp = intpart;
            for (int i = 3; i >= 0; i--)
            {
                data[i] = (byte)(temp % 256);
                temp = temp / 256;
            }

            temp = fractpart;
            for (int i = 7; i >= 4; i--)
            {
                data[i] = (byte)(temp % 256);
                temp = temp / 256;
            }
            */
            return data;
        }

        private static void ComputeParts(DateTime date, out ulong intpart, out ulong fractpart)
        {
            if (date != DateTime.MinValue)
            {
                var milliseconds = (ulong) (date - s_epoch).TotalMilliseconds;
                intpart = milliseconds/1000;
                fractpart = ((milliseconds%1000)*0x100000000L)/1000;

                // todo : guarda questo qui potrei fare casino nell'estrarre le parti, io le parti vorrei tenerele belle e pronte big endian
                // GR: se si vuole tenerle big endian E al tempo stesso esporre le proprieta' SecondsSinceEpoch e FractionalSecond
                // e' necessario swappare la endianness in lettura delle proprieta'. 
                // Se non sono strettamente necessarie opto per nascondere queste 2 proprieta'.

                //if (BitConverter.IsLittleEndian == OscPacket.LittleEndianByteOrder) // GR: non veniva fatto lo swap
                if (BitConverter.IsLittleEndian != OscPacket.LittleEndianByteOrder) // Big endian conversion
                {
                    intpart = SwapEndianness(intpart);
                    fractpart = SwapEndianness(fractpart);
                }
            }
            else
            {
                intpart = 0;
                fractpart = 1;
            }
        }

        private static uint SwapEndianness(ulong x)
        {
            return (uint) (((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        private DateTime GetNetworkTime(byte[] ntpData, int offset = 0)
        {
            var data = (byte[]) ntpData.Clone();
            if (BitConverter.IsLittleEndian)
            {
                Utility.SwapEndianInPlace(ref data, offset, 8);
                Utility.SwapEndianInPlace(ref data, offset, 4);
                Utility.SwapEndianInPlace(ref data, offset + 4, 4);
            }

            // get the seconds part
            ulong intpart = (SecondsSinceEpoch = BitConverter.ToUInt32(data, offset + 4));

            // get the seconds fraction
            ulong fractpart = (FractionalSecond = BitConverter.ToUInt32(data, offset));

            // need to invert before if this is little endian or the test wont work.


            if (fractpart != 1)
            {
                var milliseconds = (long) (((intpart*1000) + ((fractpart*1000)/0x100000000L)));

                // **UTC** time
                var networkDateTime = s_epoch.AddMilliseconds(milliseconds);

                return networkDateTime;
            }

            return DateTime.MinValue;
        }

        #endregion

        #region Private fields

        private readonly DateTime m_timeStamp;

        #endregion

        #endregion
    }
}