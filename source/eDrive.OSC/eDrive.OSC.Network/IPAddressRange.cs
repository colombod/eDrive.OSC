using System.Net;
using System.Net.Sockets;

namespace eDrive.OSC.Network
{
    public class IPAddressRange
    {
        private readonly AddressFamily m_addressFamily;
        private readonly byte[] m_lowerBytes;
        private readonly byte[] m_upperBytes;

        static IPAddressRange()
        {
            MulticastRange = new IPAddressRange(new byte[] { 224, 0, 0, 0 },
                                                new byte[] { 239, 255, 255, 255 },
                                                AddressFamily.InterNetwork);
        }

        public IPAddressRange(IPAddress lower, IPAddress upper)
            : this(lower.GetAddressBytes(), upper.GetAddressBytes(), lower.AddressFamily)
        {
        }

        public IPAddressRange(byte[] lower, byte[] upper, AddressFamily addressFamily)
        {
            m_addressFamily = addressFamily;
            m_lowerBytes = lower;
            m_upperBytes = upper;
        }

        public static IPAddressRange MulticastRange { get; private set; }

        public bool IsInRange(IPAddress address)
        {
            if (address.AddressFamily != m_addressFamily)
            {
                return false;
            }

            var addressBytes = address.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;

            for (var i = 0;
                 i < m_lowerBytes.Length &&
                 (lowerBoundary || upperBoundary);
                 i++)
            {
                if ((lowerBoundary && addressBytes[i] < m_lowerBytes[i])
                    ||
                    (upperBoundary && addressBytes[i] > m_upperBytes[i]))
                {
                    return false;
                }

                lowerBoundary &= (addressBytes[i] == m_lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == m_upperBytes[i]);
            }

            return true;
        }
    }
}