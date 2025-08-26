using System;

namespace eDrive.OSC
{
    /// <summary>
    ///     Wraps a midi message
    /// </summary>
    public class OscMidiMessage
    {
        private byte m_portId;
        private byte m_status;
        private byte m_data1;
        private byte m_data2;
        private int m_value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscMidiMessage" /> class.
        /// </summary>
        /// <param name="portId">The port id.</param>
        /// <param name="status">The status.</param>
        /// <param name="data1">The data1.</param>
        /// <param name="data2">The data2.</param>
        public OscMidiMessage(byte portId, byte status, byte data1, byte data2)
        {
            m_portId = portId;
            m_status = status;
            m_data1 = data1;
            m_data2 = data2;
            UpdateValue();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OscMidiMessage" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OscMidiMessage(int message)
        {
            m_value = message;
            UpdateParts();
        }

        /// <summary>
        ///     Gets or sets the port id.
        /// </summary>
        /// <value>
        ///     The port id.
        /// </value>
        public byte PortId
        {
            get { return m_portId; }
            set
            {
                m_portId = value;
                UpdateValue();
            }
        }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>
        ///     The status.
        /// </value>
        public byte Status
        {
            get { return m_status; }
            set
            {
                m_status = value;
                UpdateValue();
            }
        }

        /// <summary>
        ///     Gets or sets the data1.
        /// </summary>
        /// <value>
        ///     The data1.
        /// </value>
        public byte Data1
        {
            get { return m_data1; }
            set
            {
                m_data1 = value;
                UpdateValue();
            }
        }

        /// <summary>
        ///     Gets or sets the data2.
        /// </summary>
        /// <value>
        ///     The data2.
        /// </value>
        public byte Data2
        {
            get { return m_data2; }
            set
            {
                m_data2 = value;
                UpdateValue();
            }
        }

        /// <summary>
        ///     The message as int 32
        /// </summary>
        /// <returns></returns>
        public int ToInt32()
        {
            return m_value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == GetType() && Equals((OscMidiMessage)obj);
        }

        public override int GetHashCode()
        {
            return m_value;
        }

        protected bool Equals(OscMidiMessage other)
        {
            return m_value == other.m_value;
        }
        private void UpdateValue()
        {
            m_value = BitConverter.ToInt32(new[] { m_data2, m_data1, m_status, m_portId }, 0);
        }

        private void UpdateParts()
        {
            var parts = BitConverter.GetBytes(m_value);
            m_portId = parts[3];
            m_status = parts[2];
            m_data1 = parts[1];
            m_data2 = parts[0];
        }


    }
}