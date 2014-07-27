using eDrive.Osc.Tests.ReferenceProtocol;
using NUAssert = NUnit.Framework.Assert;
using eDrive.Osc;

namespace eDrive.Osc.Tests
{
    public static class OscValidation
    {
        /// <summary>
        ///     Checks that the two messages are the same.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="dst">The DST.</param>
        public static void AssertSameMessage(OscMessageReference src, OscMessage dst)
        {
            NUAssert.AreEqual(src.TypeTag, dst.TypeTag);
            NUAssert.AreEqual(src.Address, dst.Address);

            for (var i = 0; i < src.Data.Count; i++)
            {
                NUAssert.AreEqual(src.Data[i], dst.Data[i]);
            }
        }

        /// <summary>
        ///     Checks that the two serializations are equivalent.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="dst">The DST.</param>
        public static void AssertSamePacket(byte[] src, byte[] dst)
        {
            // padded by 4
            NUAssert.AreEqual(0, src.Length%4);
            NUAssert.AreEqual(0, dst.Length%4);

            // same lenght
            NUAssert.AreEqual(src.Length, dst.Length);
            for (var i = 0; i < src.Length; i++)
            {
                if (src[i] != dst[i])
                {
                    NUAssert.Fail();
                }
            }
        }
    }
}