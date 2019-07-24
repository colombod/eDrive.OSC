using eDrive.Osc.Tests.ReferenceProtocol;
using FluentAssertions;

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
            src.TypeTag.Should().Be(dst.TypeTag);
            src.Address.Should().Be(dst.Address);
            src.Data.Should().BeEquivalentTo(dst.Data);
        }

        /// <summary>
        ///     Checks that the two serializations are equivalent.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="dst">The DST.</param>
        public static void AssertSamePacket(byte[] src, byte[] dst)
        {
            // padded by 4
            (src.Length % 4).Should().Be(0);
            (dst.Length % 4).Should().Be(0);
            

            // same length
            src.Length.Should().Be(dst.Length);
            src.Should().BeEquivalentTo(dst);
        }
    }
}