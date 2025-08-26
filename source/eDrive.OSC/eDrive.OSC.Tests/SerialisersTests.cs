using eDrive.OSC.Serialisation;

using FluentAssertions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xunit;

namespace eDrive.OSC.Tests
{

    public class SerializersTests
    {
        private class UnknownType
        {
        }

        public SerializersTests()
        {
            Serialisation.SerializerFactory.LoadSerializersFromAssembly(typeof(OscMessage).Assembly);
        }


        private static int GetIntExpectedData(out byte[] expectedData)
        {
            const int value = 7456321; // a dummy value.
            var data = BitConverter.GetBytes(value);

            expectedData = new byte[4];
            expectedData[0] = data[3];
            expectedData[1] = data[2];
            expectedData[2] = data[1];
            expectedData[3] = data[0];

            return value;
        }

        private static float GetFloatExpectedData(out byte[] expectedData)
        {
            const float value = 7456321.345f; // a dummy value.
            var data = BitConverter.GetBytes(value);

            expectedData = new byte[4];
            expectedData[0] = data[3];
            expectedData[1] = data[2];
            expectedData[2] = data[1];
            expectedData[3] = data[0];

            return value;
        }

        private static double GetDoubleExpectedData(out byte[] expectedData)
        {
            const double value = 7456321.345; // a dummy value.
            var data = BitConverter.GetBytes(value);

            expectedData = new byte[8];
            expectedData[0] = data[7];
            expectedData[1] = data[6];
            expectedData[2] = data[5];
            expectedData[3] = data[4];
            expectedData[4] = data[3];
            expectedData[5] = data[2];
            expectedData[6] = data[1];
            expectedData[7] = data[0];

            return value;
        }

        private static long GetLongExpectedData(out byte[] expectedData)
        {
            const long value = 7456321; // a dummy value.
            var data = BitConverter.GetBytes(value);

            expectedData = new byte[8];
            expectedData[0] = data[7];
            expectedData[1] = data[6];
            expectedData[2] = data[5];
            expectedData[3] = data[4];
            expectedData[4] = data[3];
            expectedData[5] = data[2];
            expectedData[6] = data[1];
            expectedData[7] = data[0];

            return value;
        }

        [Fact]
        public void TestNilSerializerEncode()
        {
            var serializer = new NilSerializer();

            // according to http://opensoundcontrol.org/spec-1_0 
            // "No bytes are allocated in the argument data."

            var stream = new MemoryStream();
            serializer.Encode(stream, new NilSerializer.Nil()).Should().Be(0);
            stream.ToArray().Length.Should().Be(0);

            stream = new MemoryStream();
            serializer.Encode(stream, null).Should().Be(0);
            stream.ToArray().Length.Should().Be(0);
        }

        private static OscMidiMessage GetMidiExpectedData(out byte[] expectedData)
        {
            var value = new OscMidiMessage(0, 1, 2, 3); // a dummy value.
            expectedData = BitConverter.GetBytes(value.ToInt32());
            return value;
        }

        private static OscColour GetColourExpectedData(out byte[] expectedData)
        {
            var value = new OscColour(0, 1, 2, 3); // a dummy value.

            expectedData = new byte[4];
            expectedData[0] = 3;
            expectedData[1] = 2;
            expectedData[2] = 1;
            expectedData[3] = 0;

            return value;
        }

        private static OscSymbol GetSymbolExpectedData(out byte[] expectedData)
        {
            var value = new OscSymbol { Value = "foo" }; // a dummy value.

            expectedData = Encoding.ASCII.GetBytes(value.Value);

            return value;
        }

        private static byte[] CreateExpectedDataArray()
        {
            var headerData = GetHeaderDataArray(",[ifs]");

            // NB: here we are assuming the correct behaviour of these serializers,
            // maybe specific tests should be added as well.
            var memoryStream = new MemoryStream();
            GetArrayData(memoryStream);
            var arrayData = memoryStream.ToArray();

            return headerData.Concat(arrayData).ToArray();
        }

        private static byte[] CreateExpectedDataArray2()
        {
            var headerData = GetHeaderDataArray(",i[ifs]");

            // NB: here we are assuming the correct behaviour of these serializers,
            // maybe specific tests should be added as well.
            var memoryStream = new MemoryStream();
            SerializerFactory.GetSerializer(typeof(int)).Encode(memoryStream, 1);
            GetArrayData(memoryStream);
            var arrayData = memoryStream.ToArray();

            return headerData.Concat(arrayData).ToArray();
        }

        private static byte[] CreateExpectedDataArray3()
        {
            var headerData = GetHeaderDataArray(",[ifs]i");

            // NB: here we are assuming the correct behaviour of these serializers,
            // maybe specific tests should be added as well.
            var memoryStream = new MemoryStream();
            GetArrayData(memoryStream);
            SerializerFactory.GetSerializer(typeof(int)).Encode(memoryStream, 1);
            var arrayData = memoryStream.ToArray();

            return headerData.Concat(arrayData).ToArray();
        }

        private static byte[] CreateExpectedDataArray4()
        {
            var headerData = GetHeaderDataArray(",[ifs]b");

            // NB: here we are assuming the correct behaviour of these serializers,
            // maybe specific tests should be added as well.
            var memoryStream = new MemoryStream();
            GetArrayData(memoryStream);
            SerializerFactory.GetSerializer(typeof(byte[])).Encode(memoryStream, CreateTestBlob());
            var arrayData = memoryStream.ToArray();

            return headerData.Concat(arrayData).ToArray();
        }

        private static IEnumerable<byte> GetHeaderDataArray(string typeTag)
        {
            var addressData = Encoding.ASCII
                                      .GetBytes("/test")
                                      .Concat(new byte[3]); // padding

            var typeTagData = Encoding.ASCII
                                      .GetBytes(typeTag);

            var padding =
                new byte[4 - (typeTagData.Length % 4)];

            return addressData
                .Concat(typeTagData)
                .Concat(padding)
                .ToArray();
        }

        private static void GetArrayData(Stream memoryStream)
        {
            // NB: here we are assuming the correct behaviour of these serializers,
            // maybe specific tests should be added as well.

            SerializerFactory.GetSerializer(typeof(int)).Encode(memoryStream, 1);
            SerializerFactory.GetSerializer(typeof(float)).Encode(memoryStream, 2f);
            SerializerFactory.GetSerializer(typeof(string)).Encode(memoryStream, "foo");
        }

        private static byte[] CreateTestBlob()
        {
            var blob = new byte[4];
            blob[0] = 0xff;
            blob[1] = 0xab;
            blob[2] = 0x1e;
            blob[3] = 0;
            return blob;
        }

        private static void AssertCorrectArray(IList<object> actual)
        {
            actual.Should().NotBeNull();
            actual[0].Should().Be(1);
            actual[1].Should().Be(2);
            actual[2].Should().Be("foo");
        }


        private static void AssertSerializerType(IOscTypeSerializer serializer, Type type, char tag)
        {
            serializer.Type.Should().Be(type);
            serializer.Tag.Should().Be(tag);
        }

        private static void AssertEncode<T>(OscTypeSerializer<T> serializer, T value, byte[] expectedData)
        {
            var stream1 = new MemoryStream();
            serializer.Encode(stream1, value);

            var actualData = stream1.ToArray().Take(expectedData.Length);
            actualData.Should().BeEquivalentTo(expectedData);



            var stream2 = new MemoryStream();
            serializer.Encode(stream2, (object)value);

            actualData = stream2.ToArray().Take(expectedData.Length);
            actualData.Should().BeEquivalentTo(expectedData);
        }

        private static void AssertDecode<T>
            (
            IOscTypeSerializer<T> serializer, byte[] data, T expected, int size)
        {
            int pos;
            var actual = serializer.Decode(data, 0, out pos);
            actual.Should().Be(expected);
            pos.Should().Be(size);
        }



        [Fact]
        public void TestArrayDeserialisation()
        {
            var msg = OscPacket.FromByteArray(CreateExpectedDataArray());

            const string typeTag = ",[ifs]";
            msg.Should().BeOfType<OscMessage>()
                .Which
                .TypeTag.Should().Be(typeTag);

            var actual = msg.Data[0] as object[]; // the heterogeneous array.

            AssertCorrectArray(actual);
        }

        [Fact]
        public void TestArrayDeserialisation2()
        {
            var msg = OscPacket.FromByteArray(CreateExpectedDataArray2());

            const string typeTag = ",i[ifs]";

            msg.Should().BeOfType<OscMessage>()
                .Which
                .TypeTag.Should().Be(typeTag);

            msg.Data[0].Should().Be(1);

            var actual = msg.Data[1] as object[]; // the heterogeneous array.

            AssertCorrectArray(actual);
        }

        [Fact]
        public void TestArrayDeserialisation3()
        {
            var msg = OscPacket.FromByteArray(CreateExpectedDataArray3());

            const string typeTag = ",[ifs]i";

            msg.Should().BeOfType<OscMessage>()
                .Which
                .TypeTag.Should().Be(typeTag);

            var actual = msg.Data[0] as object[]; // the heterogeneous array.

            AssertCorrectArray(actual);
            msg.Data[1].Should().Be(1);
        }

        [Fact]
        public void TestArrayDeserialisation4()
        {
            var msg = OscPacket.FromByteArray(CreateExpectedDataArray4());

            const string typeTag = ",[ifs]b";

            msg.Should().BeOfType<OscMessage>()
                .Which
                .TypeTag.Should().Be(typeTag);



            var actual = msg.Data[0] as object[]; // the heterogeneous array.

            AssertCorrectArray(actual);

            var expectedBlob = CreateTestBlob();
            var actualBlob = (byte[])msg.Data[1];
            actualBlob.Should().BeEquivalentTo(expectedBlob);
        }

        [Fact]
        public void TestArraySerialisation()
        {
            var data = new object[] { 1, 2f, "foo" };
            var msg = new OscMessage("/test", data);

            var expected = CreateExpectedDataArray();
            msg.TypeTag.Should().Be(",[ifs]");
            OscValidation.AssertSamePacket(expected, msg.ToByteArray());
        }

        [Fact]
        public void TestArraySerialisation2()
        {
            var msg = new OscMessage("/test", 1); // the first argument is an integer 
            msg.Append(new object[] { 1, 2f, "foo" });

            var expected = CreateExpectedDataArray2();
            msg.TypeTag.Should().Be(",i[ifs]");
            OscValidation.AssertSamePacket(expected, msg.ToByteArray());
        }

        [Fact]
        public void TestArraySerialisation3()
        {
            var msg = new OscMessage("/test", new object[] { 1, 2f, "foo" });
            msg.Append(1); // the last argument is an integer.

            var expected = CreateExpectedDataArray3();
            msg.TypeTag.Should().Be(",[ifs]i");
            OscValidation.AssertSamePacket(expected, msg.ToByteArray());
        }

        [Fact]
        public void TestArraySerialisation4()
        {
            var msg = new OscMessage("/test", new object[] { 1, 2f, "foo" });

            var blob = CreateTestBlob();
            msg.Append(blob); // the last argument is a blob.

            var expected = CreateExpectedDataArray4();
            msg.TypeTag.Should().Be(",[ifs]b");
            OscValidation.AssertSamePacket(expected, msg.ToByteArray());
        }

        [Fact]
        public void TestBooleanSerializer()
        {
            AssertSerializerType(new BooleanValueSerializer(), typeof(bool), ' ');
        }

        [Fact]
        public void TestBooleanSerializerDecode()
        {
            var serializer = new BooleanValueSerializer();

            int pos;
            var action = new Action(() => serializer.Decode(new byte[1], 0, out pos));
            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void TestBooleanSerializerEncode()
        {
            var serializer = new BooleanValueSerializer();

            // according to http://opensoundcontrol.org/spec-1_0 
            // "No bytes are allocated in the argument data."

            var stream = new MemoryStream();
            serializer.Encode(stream, true).Should().Be(0);
            stream.ToArray().Length.Should().Be(0);

            stream = new MemoryStream();
            serializer.Encode(stream, false).Should().Be(0);
            stream.ToArray().Length.Should().Be(0);
        }

        [Fact]
        public void TestCharSerializer()
        {
            AssertSerializerType(new CharSerializer(), typeof(char), 'c');
        }

        [Fact]
        public void TestCharSerializerDecode()
        {
            // according to http://opensoundcontrol.org/spec-1_0, chars are encoded as 32 bits ascii characters.
            const char expected = 'a';

            var data = new byte[4];
            Array.Copy(BitConverter.GetBytes(expected), data, 2);
            Array.Reverse(data);

            AssertDecode(new CharSerializer(), data, expected, 4);
        }

        [Fact]
        public void TestCharSerializerEncode()
        {
            // according to http://opensoundcontrol.org/spec-1_0, chars are encoded as 32 bits ascii characters.
            const char value = 'a';
            var expectedData = new byte[4];
            Array.Copy(BitConverter.GetBytes(value), expectedData, 2);
            Array.Reverse(expectedData);

            AssertEncode(new CharSerializer(), value, expectedData);
        }

        [Fact]
        public void TestDoubleSerializer()
        {
            AssertSerializerType(new DoubleSerializer(), typeof(double), 'd');
        }

        [Fact]
        public void TestDoubleSerializerDecode()
        {
            var expected = GetDoubleExpectedData(out var data);

            AssertDecode(new DoubleSerializer(), data, expected, 8);
        }

        [Fact]
        public void TestDoubleSerializerEncode()
        {
            var value = GetDoubleExpectedData(out var expectedData);

            AssertEncode(new DoubleSerializer(), value, expectedData);
        }

        /*
        [Fact]
        public void TestEndPointSerialisation()
        {
            var ep = new OscEndPoint(IPAddress.Parse("122.168.2.2"), 9900, TransportType.Udp, TransmissionType.Unicast);

            var serializer = new EndPointSerializer();
            using (var s = new MemoryStream())
            {
                var l = serializer.Encode(s, ep);
                NUAssert.AreNotEqual(0, l);

                var bytes = s.ToArray();

                var ep2 = serializer.Decode(bytes, 0, out l);

                NUAssert.AreEqual(ep.EndPoint, ep2.EndPoint);
                NUAssert.AreEqual(ep.TransportType, ep2.TransportType);
                NUAssert.AreEqual(ep.TransmissionType, ep2.TransmissionType);
            }
        }
*/
        [Fact]
        public void TestFalseSerializer()
        {
            AssertSerializerType(new FalseBooleanValueDeserializer(), typeof(bool), 'F');
        }

        [Fact]
        public void TestFalseSerializerDecode()
        {
            var serializer = new FalseBooleanValueDeserializer();

            serializer.Decode(new byte[1], 0, out var pos).Should().BeFalse();
            pos.Should().Be(0);
        }

        [Fact]
        public void TestFalseSerializerEncode()
        {
            var serializer = new FalseBooleanValueDeserializer();
            var action = new Action(() => serializer.Encode(new MemoryStream(), true));
            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void TestFloatSerializer()
        {
            AssertSerializerType(new FloatSerializer(), typeof(float), 'f');
        }

        [Fact]
        public void TestFloatSerializerDecode()
        {
            byte[] data;
            var expected = GetFloatExpectedData(out data);

            AssertDecode(new FloatSerializer(), data, expected, 4);
        }

        [Fact]
        public void TestFloatSerializerEncode()
        {
            byte[] expectedData;
            var value = GetFloatExpectedData(out expectedData);

            AssertEncode(new FloatSerializer(), value, expectedData);
        }

        [Fact]
        public void TestIntSerializer()
        {
            AssertSerializerType(new IntSerializer(), typeof(int), 'i');
        }

        [Fact]
        public void TestIntSerializerDecode()
        {
            byte[] data;
            var expected = GetIntExpectedData(out data);

            AssertDecode(new IntSerializer(), data, expected, 4);
        }

        [Fact]
        public void TestIntSerializerEncode()
        {
            byte[] expectedData;
            var value = GetIntExpectedData(out expectedData);

            AssertEncode(new IntSerializer(), value, expectedData);
        }

        [Fact]
        public void TestLongSerializer()
        {
            AssertSerializerType(new LongSerializer(), typeof(long), 'h');
        }

        [Fact]
        public void TestLongSerializerDecode()
        {
            byte[] data;
            var expected = GetLongExpectedData(out data);

            AssertDecode(new LongSerializer(), data, expected, 8);
        }

        [Fact]
        public void TestLongSerializerEncode()
        {
            byte[] expectedData;
            var value = GetLongExpectedData(out expectedData);

            AssertEncode(new LongSerializer(), value, expectedData);
        }

        [Fact]
        public void TestNestedArrayNotSupported()
        {
            var value = new object[2];
            value[0] = "foo";
            value[1] = new object[] { 3, "boo" };

            var success = false;
            OscMessage msg;
            try
            {
                msg = new OscMessage("/test", value);
            }
            catch (OscSerializerException e)
            {
                e.Message.Should().Be("Nested arrays are not supported.");
                success = true;
                msg = null;
            }

            success.Should().BeTrue();
            msg.Should().BeNull();
        }

        [Fact]
        public void TestNilSerializer()
        {
            AssertSerializerType(new NilSerializer(), typeof(NilSerializer.Nil), 'N');
        }

        [Fact]
        public void TestNilSerializerDecode()
        {
            var serializer = new NilSerializer();

            int pos;
            serializer.Decode(new byte[1], 0, out pos).Should().BeNull();
            pos.Should().Be(0);
        }

        /// <summary>
        ///     Checks that the ToInt32 yields the int32 corresponding to the little endian
        ///     internal representation of the 4 bytes.
        ///     http://opensoundcontrol.org/spec-1_0
        /// </summary>
        [Fact]
        public void TestOscColourMessageToInt32()
        {
            var value = new OscColour(1, 0, 0, 0);
            var actual = value.ToInt32();
            actual.Should().Be(1);
        }

        [Fact]
        public void TestOscColourSerializer()
        {
            AssertSerializerType(new OscColourSerializer(), typeof(OscColour), 'r');
        }

        [Fact]
        public void TestOscColourSerializerDecode()
        {
            byte[] data;
            var expected = GetColourExpectedData(out data);

            AssertDecode(new OscColourSerializer(), data, expected, 4);
        }

        [Fact]
        public void TestOscColourSerializerEncode()
        {
            byte[] expectedData;
            var value = GetColourExpectedData(out expectedData);

            AssertEncode(new OscColourSerializer(), value, expectedData);
        }

        /// <summary>
        ///     Checks that the ToInt32 yields the int32 corresponding to the big endian
        ///     internal representation of the 4 bytes.
        /// </summary>
        [Fact]
        public void TestOscMidiMessageToInt32()
        {
            var value = new OscMidiMessage(1, 0, 0, 0);
            var actual = value.ToInt32();
            actual.Should().Be(16777216);
        }

        [Fact]
        public void TestOscMidiSerializer()
        {
            AssertSerializerType(new OscMidiSerializer(), typeof(OscMidiMessage), 'm');
        }

        [Fact]
        public void TestOscMidiSerializerDecode()
        {
            byte[] data;
            var expected = GetMidiExpectedData(out data);

            AssertDecode(new OscMidiSerializer(), data, expected, 4);
        }

        [Fact]
        public void TestOscMidiSerializerEncode()
        {
            byte[] expectedData;
            var value = GetMidiExpectedData(out expectedData);

            AssertEncode(new OscMidiSerializer(), value, expectedData);
        }

        /// <summary>
        ///     Checks that an unsupported type raise an error upon OscMessage creation.
        /// </summary>
        [Fact]
        public void TestOscSerializerExceptionWhenUnsupportedType()
        {
            OscMessage msg = null;
            try
            {
                msg = new OscMessage("/test", new UnknownType());
            }
            catch (OscSerializerException e)
            {
                msg = null;
                e.InnerException.Should().BeOfType<KeyNotFoundException>();
                e.Message.Should().Be($"Unsupported type: {typeof(UnknownType)}");

            }

            msg.Should().BeNull();
        }

        [Fact]
        public void TestStringSerializer()
        {
            AssertSerializerType(new StringSerializer(), typeof(string), 's');
        }

        [Fact]
        public void TestStringSerializerDecode()
        {
            const string expected = "data";
            var data = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(expected), data, 4);

            AssertDecode(new StringSerializer(), data, expected, 8);
        }

        [Fact]
        public void TestStringSerializerEncode()
        {
            const string value = "data";
            var expectedData = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(value), expectedData, 4);

            AssertEncode(new StringSerializer(), value, expectedData);
        }

        [Fact]
        public void TestSymbolDeserializer()
        {
            AssertSerializerType(new SymbolSerializer(), typeof(OscSymbol), 'S');
        }

        [Fact]
        public void TestSymbolDeserializerDecode()
        {
            var expected = GetSymbolExpectedData(out var data);

            AssertDecode(new SymbolSerializer(), data, expected, 4);
        }

        [Fact]
        public void TestSymbolDeserializerEncode()
        {
            var value = GetSymbolExpectedData(out var expectedData);

            AssertEncode(new SymbolSerializer(), value, expectedData);
        }

        /// <summary>
        ///     Tests that Encode/Decode produce two equals OscTimeTag.
        /// </summary>
        [Fact]
        public void TestTimeTagSerializer()
        {
            var stream = new MemoryStream();
            var expected = new OscTimeTag();

            SerializerFactory.TimeTagSerializer.Encode(stream, expected);
            var data = stream.ToArray();

            var actual = SerializerFactory.TimeTagSerializer.Decode(data, 0, out _);
            actual.Should().Be(expected);
        }

        /// <summary>
        ///     Tests the deserialisation for an immediate value.
        /// </summary>
        [Fact]
        public void TestTimeTagSerializerDecodeImmediateValue()
        {
            var data = new byte[8];
            data[7] = 1;

            var actual = SerializerFactory.TimeTagSerializer.Decode(data, 0, out _);
            actual.IsImmediate.Should().BeTrue();
            actual.TimeTag.Should().Be(1UL);
            actual.DateTime.Should().Be(DateTime.MinValue);
        }

        /// <summary>
        ///     Tests the serialisation for an immediate value.
        /// </summary>
        [Fact]
        public void TestTimeTagSerializerEncodeImmediateValue()
        {
            var stream = new MemoryStream();
            var timetag = new OscTimeTag();

            var actual = SerializerFactory.TimeTagSerializer.Encode(stream, timetag);
            var data = stream.ToArray();
            actual.Should().Be(8);

            data.Take(7).Should().BeEquivalentTo(Enumerable.Repeat(0, 7));
            data.Last().Should().Be(1);
        }

        [Fact]
        public void TestTrueSerializer()
        {
            AssertSerializerType(new TrueBooleanValueDeserializer(), typeof(bool), 'T');
        }

        [Fact]
        public void TestTrueSerializerDecode()
        {
            var serializer = new TrueBooleanValueDeserializer();

            serializer.Decode(new byte[1], 0, out var pos).Should().BeTrue();
            pos.Should().Be(0);
        }

        [Fact]
        public void TestTrueSerializerEncode()
        {
            var serializer = new TrueBooleanValueDeserializer();
            var action = new Action(() => serializer.Encode(new MemoryStream(), true));
            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void TestVersionSerialisation()
        {
            var version = new Version(1, 0, 2, 3);

            var serializer = new VersionSerializer();
            using (var s = new MemoryStream())
            {
                var l = serializer.Encode(s, version);
                l.Should().NotBe(0);

                var bytes = s.ToArray();

                var v = serializer.Decode(bytes, 0, out l);
                v.Should().Be(version);
            }
        }
    }
}