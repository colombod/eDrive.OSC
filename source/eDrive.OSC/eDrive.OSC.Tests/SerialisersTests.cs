#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;
using eDrive.Osc.Serialisation;
using eDrive.Osc;
using EDAssert = eDrive.Assert;
using NUAssert = NUnit.Framework.Assert;

#endregion

namespace eDrive.Osc.Tests
{
    [TestFixture]
    public class SerialisersTests
    {
        private class UnknownType
        {
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

        public void TestNilSerialiserEncode()
        {
            var serialiser = new NilSerialiser();

            // according to http://opensoundcontrol.org/spec-1_0 
            // "No bytes are allocated in the argument data."

            var stream = new MemoryStream();
            NUAssert.AreEqual(0, serialiser.Encode(stream, new NilSerialiser.Nil()));
            NUAssert.AreEqual(0, stream.ToArray().Length);

            stream = new MemoryStream();
            NUAssert.AreEqual(0, serialiser.Encode(stream, null));
            NUAssert.AreEqual(0, stream.ToArray().Length);
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
            var value = new OscSymbol {Value = "foo"}; // a dummy value.

            expectedData = Encoding.ASCII.GetBytes(value.Value);

            return value;
        }

        private static byte[] CreateExpectedDataArray()
        {
            var headerData = GetHeaderDataArray(",[ifs]");

            // NB: here we are assuming the correct behaviour of these serialisers,
            // maybe specific tests should be added as well.
            var memoryStream = new MemoryStream();
            GetArrayData(memoryStream);
            var arrayData = memoryStream.ToArray();

            return headerData.Concat(arrayData).ToArray();
        }

        private static byte[] CreateExpectedDataArray2()
        {
            var headerData = GetHeaderDataArray(",i[ifs]");

            // NB: here we are assuming the correct behaviour of these serialisers,
            // maybe specific tests should be added as well.
            var memoryStream = new MemoryStream();
            SerialiserFactory.GetSerialiser(typeof (int)).Encode(memoryStream, 1);
            GetArrayData(memoryStream);
            var arrayData = memoryStream.ToArray();

            return headerData.Concat(arrayData).ToArray();
        }

        private static byte[] CreateExpectedDataArray3()
        {
            var headerData = GetHeaderDataArray(",[ifs]i");

            // NB: here we are assuming the correct behaviour of these serialisers,
            // maybe specific tests should be added as well.
            var memoryStream = new MemoryStream();
            GetArrayData(memoryStream);
            SerialiserFactory.GetSerialiser(typeof (int)).Encode(memoryStream, 1);
            var arrayData = memoryStream.ToArray();

            return headerData.Concat(arrayData).ToArray();
        }

        private static byte[] CreateExpectedDataArray4()
        {
            var headerData = GetHeaderDataArray(",[ifs]b");

            // NB: here we are assuming the correct behaviour of these serialisers,
            // maybe specific tests should be added as well.
            var memoryStream = new MemoryStream();
            GetArrayData(memoryStream);
            SerialiserFactory.GetSerialiser(typeof (byte[])).Encode(memoryStream, CreateTestBlob());
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
                new byte[4 - (typeTagData.Length%4)];

            return addressData
                .Concat(typeTagData)
                .Concat(padding)
                .ToArray();
        }

        private static void GetArrayData(Stream memoryStream)
        {
            // NB: here we are assuming the correct behaviour of these serialisers,
            // maybe specific tests should be added as well.

            SerialiserFactory.GetSerialiser(typeof (int)).Encode(memoryStream, 1);
            SerialiserFactory.GetSerialiser(typeof (float)).Encode(memoryStream, 2f);
            SerialiserFactory.GetSerialiser(typeof (string)).Encode(memoryStream, "foo");
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
            NUAssert.IsNotNull(actual);

            NUAssert.AreEqual(1, actual[0]);
            NUAssert.AreEqual(2f, actual[1]);
            NUAssert.AreEqual("foo", actual[2]);
        }

        private static void AssertSerialiserType(IOscTypeSerialiser serialiser, Type type, char tag)
        {
            NUAssert.AreEqual(type, serialiser.Type);
            NUAssert.AreEqual(tag, serialiser.Tag);
        }

        private static void AssertEncode<T>(OscTypeSerialiser<T> serialiser, T value, byte[] expectedData)
        {
            var stream1 = new MemoryStream();
            serialiser.Encode(stream1, value);

            var actualData = stream1.ToArray();

            for (var i = 0; i < expectedData.Length; i++)
            {
                NUAssert.AreEqual(expectedData[i], actualData[i]);
            }


            var stream2 = new MemoryStream();
            serialiser.Encode(stream2, (object) value);

            actualData = stream2.ToArray();

            for (var i = 0; i < expectedData.Length; i++)
            {
                NUAssert.AreEqual(expectedData[i], actualData[i]);
            }
        }

        private static void AssertDecode<T>
            (
            IOscTypeSerialiser<T> serialiser, byte[] data, T expected, int size)
        {
            int pos;
            var actual = serialiser.Decode(data, 0, out pos);

            NUAssert.AreEqual(expected, actual);
            NUAssert.AreEqual(size, pos);
        }

		[TestFixtureSetUp]
		public void Setup(){
			Serialisation.SerialiserFactory.LoadSerialisersFromAssembly(typeof(OscMessage).Assembly);
		}

        [Test]
        public void TestArrayDeserialisation()
        {
            var msg = OscPacket.FromByteArray(CreateExpectedDataArray());

            const string typeTag = ",[ifs]";

            NUAssert.IsTrue(msg is OscMessage);
            NUAssert.AreEqual(typeTag, ((OscMessage) msg).TypeTag);

            var actual = msg.Data[0] as object[]; // the heterogeneous array.

            AssertCorrectArray(actual);
        }

        [Test]
        public void TestArrayDeserialisation2()
        {
            var msg = OscPacket.FromByteArray(CreateExpectedDataArray2());

            const string typeTag = ",i[ifs]";

            NUAssert.IsTrue(msg is OscMessage);
            NUAssert.AreEqual(typeTag, ((OscMessage) msg).TypeTag);

            NUAssert.AreEqual(msg.Data[0], 1);

            var actual = msg.Data[1] as object[]; // the heterogeneous array.

            AssertCorrectArray(actual);
        }

        [Test]
        public void TestArrayDeserialisation3()
        {
            var msg = OscPacket.FromByteArray(CreateExpectedDataArray3());

            const string typeTag = ",[ifs]i";

            NUAssert.IsTrue(msg is OscMessage);
            NUAssert.AreEqual(typeTag, ((OscMessage) msg).TypeTag);

            var actual = msg.Data[0] as object[]; // the heterogeneous array.

            AssertCorrectArray(actual);

            NUAssert.AreEqual(msg.Data[1], 1);
        }

        [Test]
        public void TestArrayDeserialisation4()
        {
            var msg = OscPacket.FromByteArray(CreateExpectedDataArray4());

            const string typeTag = ",[ifs]b";

            NUAssert.IsTrue(msg is OscMessage);
            NUAssert.AreEqual(typeTag, ((OscMessage) msg).TypeTag);

            var actual = msg.Data[0] as object[]; // the heterogeneous array.

            AssertCorrectArray(actual);

            var expectedBlob = CreateTestBlob();
            var actualBlob = (byte[]) msg.Data[1];
            for (var i = 0; i < expectedBlob.Length; i++)
            {
                NUAssert.AreEqual(expectedBlob[i], actualBlob[i]);
            }
        }

        [Test]
        public void TestArraySerialisation()
        {
            var data = new object[] {1, 2f, "foo"};
            var msg = new OscMessage("/test", data);

            var expected = CreateExpectedDataArray();

            NUAssert.AreEqual(",[ifs]", msg.TypeTag);
            OscValidation.AssertSamePacket(expected, msg.ToByteArray());
        }

        [Test]
        public void TestArraySerialisation2()
        {
            var msg = new OscMessage("/test", 1); // the first argument is an integer 
            msg.Append(new object[] {1, 2f, "foo"});

            var expected = CreateExpectedDataArray2();

            NUAssert.AreEqual(",i[ifs]", msg.TypeTag);
            OscValidation.AssertSamePacket(expected, msg.ToByteArray());
        }

        [Test]
        public void TestArraySerialisation3()
        {
            var msg = new OscMessage("/test", new object[] {1, 2f, "foo"});
            msg.Append(1); // the last argument is an integer.

            var expected = CreateExpectedDataArray3();

            NUAssert.AreEqual(",[ifs]i", msg.TypeTag);
            OscValidation.AssertSamePacket(expected, msg.ToByteArray());
        }

        [Test]
        public void TestArraySerialisation4()
        {
            var msg = new OscMessage("/test", new object[] {1, 2f, "foo"});

            var blob = CreateTestBlob();
            msg.Append(blob); // the last argument is a blob.

            var expected = CreateExpectedDataArray4();

            NUAssert.AreEqual(",[ifs]b", msg.TypeTag);
            OscValidation.AssertSamePacket(expected, msg.ToByteArray());
        }

        [Test]
        public void TestBooleanSerialiser()
        {
            AssertSerialiserType(new BooleanValueSerialiser(), typeof (bool), ' ');
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TestBooleanSerialiserDecode()
        {
            var serialiser = new BooleanValueSerialiser();

            int pos;
            serialiser.Decode(new byte[1], 0, out pos);
        }

        [Test]
        public void TestBooleanSerialiserEncode()
        {
            var serialiser = new BooleanValueSerialiser();

            // according to http://opensoundcontrol.org/spec-1_0 
            // "No bytes are allocated in the argument data."

            var stream = new MemoryStream();
            NUAssert.AreEqual(0, serialiser.Encode(stream, true));
            NUAssert.AreEqual(0, stream.ToArray().Length);

            stream = new MemoryStream();
            NUAssert.AreEqual(0, serialiser.Encode(stream, false));
            NUAssert.AreEqual(0, stream.ToArray().Length);
        }

        [Test]
        public void TestCharSerialiser()
        {
            AssertSerialiserType(new CharSerialiser(), typeof (char), 'c');
        }

        [Test]
        public void TestCharSerialiserDecode()
        {
            // according to http://opensoundcontrol.org/spec-1_0, chars are encoded as 32 bits ascii characters.
            const char expected = 'a';

            var data = new byte[4];
            Array.Copy(BitConverter.GetBytes(expected), data, 2);
            Array.Reverse(data);

            AssertDecode(new CharSerialiser(), data, expected, 4);
        }

        [Test]
        public void TestCharSerialiserEncode()
        {
            // according to http://opensoundcontrol.org/spec-1_0, chars are encoded as 32 bits ascii characters.
            const char value = 'a';
            var expectedData = new byte[4];
            Array.Copy(BitConverter.GetBytes(value), expectedData, 2);
            Array.Reverse(expectedData);

            AssertEncode(new CharSerialiser(), value, expectedData);
        }

        [Test]
        public void TestDoubleSerialiser()
        {
            AssertSerialiserType(new DoubleSerialiser(), typeof (double), 'd');
        }

        [Test]
        public void TestDoubleSerialiserDecode()
        {
            byte[] data;
            var expected = GetDoubleExpectedData(out data);

            AssertDecode(new DoubleSerialiser(), data, expected, 8);
        }

        [Test]
        public void TestDoubleSerialiserEncode()
        {
            byte[] expectedData;
            var value = GetDoubleExpectedData(out expectedData);

            AssertEncode(new DoubleSerialiser(), value, expectedData);
        }

		/*
        [Test]
        public void TestEndPointSerialisation()
        {
            var ep = new OscEndPoint(IPAddress.Parse("122.168.2.2"), 9900, TransportType.Udp, TransmissionType.Unicast);

            var serialiser = new EndPointSerialiser();
            using (var s = new MemoryStream())
            {
                var l = serialiser.Encode(s, ep);
                NUAssert.AreNotEqual(0, l);

                var bytes = s.ToArray();

                var ep2 = serialiser.Decode(bytes, 0, out l);

                NUAssert.AreEqual(ep.EndPoint, ep2.EndPoint);
                NUAssert.AreEqual(ep.TransportType, ep2.TransportType);
                NUAssert.AreEqual(ep.TransmissionType, ep2.TransmissionType);
            }
        }
*/
        [Test]
        public void TestFalseSerialiser()
        {
            AssertSerialiserType(new FalseBooleanValueDeserialiser(), typeof (bool), 'F');
        }

        [Test]
        public void TestFalseSerialiserDecode()
        {
            var serialiser = new FalseBooleanValueDeserialiser();

            int pos;
            NUAssert.IsFalse(serialiser.Decode(new byte[1], 0, out pos));
            NUAssert.AreEqual(0, pos);
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TestFalseSerialiserEncode()
        {
            var serialiser = new FalseBooleanValueDeserialiser();
            serialiser.Encode(new MemoryStream(), true);
        }

        [Test]
        public void TestFloatSerialiser()
        {
            AssertSerialiserType(new FloatSerialiser(), typeof (float), 'f');
        }

        [Test]
        public void TestFloatSerialiserDecode()
        {
            byte[] data;
            var expected = GetFloatExpectedData(out data);

            AssertDecode(new FloatSerialiser(), data, expected, 4);
        }

        [Test]
        public void TestFloatSerialiserEncode()
        {
            byte[] expectedData;
            var value = GetFloatExpectedData(out expectedData);

            AssertEncode(new FloatSerialiser(), value, expectedData);
        }

        [Test]
        public void TestIntSerialiser()
        {
            AssertSerialiserType(new IntSerialiser(), typeof (int), 'i');
        }

        [Test]
        public void TestIntSerialiserDecode()
        {
            byte[] data;
            var expected = GetIntExpectedData(out data);

            AssertDecode(new IntSerialiser(), data, expected, 4);
        }

        [Test]
        public void TestIntSerialiserEncode()
        {
            byte[] expectedData;
            var value = GetIntExpectedData(out expectedData);

            AssertEncode(new IntSerialiser(), value, expectedData);
        }

        [Test]
        public void TestLongSerialiser()
        {
            AssertSerialiserType(new LongSerialiser(), typeof (long), 'h');
        }

        [Test]
        public void TestLongSerialiserDecode()
        {
            byte[] data;
            var expected = GetLongExpectedData(out data);

            AssertDecode(new LongSerialiser(), data, expected, 8);
        }

        [Test]
        public void TestLongSerialiserEncode()
        {
            byte[] expectedData;
            var value = GetLongExpectedData(out expectedData);

            AssertEncode(new LongSerialiser(), value, expectedData);
        }

        [Test]
        public void TestNestedArrayNotSupported()
        {
            var value = new object[2];
            value[0] = "foo";
            value[1] = new object[] {3, "boo"};

            var success = false;
			OscMessage msg = null;
            try
            {
                msg = new OscMessage("/test", value);
            }
            catch (OscSerialiserException e)
            {
                NUAssert.AreEqual("Nested arrays are not supported.", e.Message);
                success = true;
				msg = null;
            }

            NUAssert.IsTrue(success);
			NUAssert.IsNull (msg);
        }

        [Test]
        public void TestNilSerialiser()
        {
            AssertSerialiserType(new NilSerialiser(), typeof (NilSerialiser.Nil), 'N');
        }

        [Test]
        public void TestNilSerialiserDecode()
        {
            var serialiser = new NilSerialiser();

            int pos;
            NUAssert.IsNull(serialiser.Decode(new byte[1], 0, out pos));
            NUAssert.AreEqual(0, pos);
        }

        /// <summary>
        ///     Checks that the ToInt32 yields the int32 corresponding to the little endian
        ///     internal representation of the 4 bytes.
        ///     http://opensoundcontrol.org/spec-1_0
        /// </summary>
        [Test]
        public void TestOscColourMessageToInt32()
        {
            var value = new OscColour(1, 0, 0, 0);
            var actual = value.ToInt32();

            NUAssert.AreEqual(1, actual);
        }

        [Test]
        public void TestOscColourSerialiser()
        {
            AssertSerialiserType(new OscColourSerialiser(), typeof (OscColour), 'r');
        }

        [Test]
        public void TestOscColourSerialiserDecode()
        {
            byte[] data;
            var expected = GetColourExpectedData(out data);

            AssertDecode(new OscColourSerialiser(), data, expected, 4);
        }

        [Test]
        public void TestOscColourSerialiserEncode()
        {
            byte[] expectedData;
            var value = GetColourExpectedData(out expectedData);

            AssertEncode(new OscColourSerialiser(), value, expectedData);
        }

        /// <summary>
        ///     Checks that the ToInt32 yields the int32 corresponding to the big endian
        ///     internal representation of the 4 bytes.
        /// </summary>
        [Test]
        public void TestOscMidiMessageToInt32()
        {
            var value = new OscMidiMessage(1, 0, 0, 0);
            var actual = value.ToInt32();

            NUAssert.AreEqual(16777216, actual);
        }

        [Test]
        public void TestOscMidiSerialiser()
        {
            AssertSerialiserType(new OscMidiSerialiser(), typeof (OscMidiMessage), 'm');
        }

        [Test]
        public void TestOscMidiSerialiserDecode()
        {
            byte[] data;
            var expected = GetMidiExpectedData(out data);

            AssertDecode(new OscMidiSerialiser(), data, expected, 4);
        }

        [Test]
        public void TestOscMidiSerialiserEncode()
        {
            byte[] expectedData;
            var value = GetMidiExpectedData(out expectedData);

            AssertEncode(new OscMidiSerialiser(), value, expectedData);
        }

        /// <summary>
        ///     Checks that an unsupported type raise an error upon OscMessage creation.
        /// </summary>
        [Test]
        public void TestOscSerialiserExceptionWhenUnsupportedType()
        {
			OscMessage msg = null;
            try
            {
                msg = new OscMessage("/test", new UnknownType());
            }
            catch (OscSerialiserException e)
            {
				msg = null;
                NUAssert.AreEqual(typeof (KeyNotFoundException), e.InnerException.GetType());
                NUAssert.AreEqual(
                    string.Format("Unsupported type: {0}", typeof (UnknownType)), e.Message);

            }

            NUAssert.IsNull(msg);
        }

        [Test]
        public void TestStringSerialiser()
        {
            AssertSerialiserType(new StringSerialiser(), typeof (string), 's');
        }

        [Test]
        public void TestStringSerialiserDecode()
        {
            const string expected = "data";
            var data = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(expected), data, 4);

            AssertDecode(new StringSerialiser(), data, expected, 8);
        }

        [Test]
        public void TestStringSerialiserEncode()
        {
            const string value = "data";
            var expectedData = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(value), expectedData, 4);

            AssertEncode(new StringSerialiser(), value, expectedData);
        }

        [Test]
        public void TestSymbolDeserialiser()
        {
            AssertSerialiserType(new SymbolSerialiser(), typeof (OscSymbol), 'S');
        }

        [Test]
        public void TestSymbolDeserialiserDecode()
        {
            byte[] data;
            var expected = GetSymbolExpectedData(out data);

            AssertDecode(new SymbolSerialiser(), data, expected, 4);
        }

        [Test]
        public void TestSymbolDeserialiserEncode()
        {
            byte[] expectedData;
            var value = GetSymbolExpectedData(out expectedData);

            AssertEncode(new SymbolSerialiser(), value, expectedData);
        }

        /// <summary>
        ///     Tests that Encode/Decode produce two equals OscTimeTag.
        /// </summary>
        [Test]
        public void TestTimeTagSerialiser()
        {
            var stream = new MemoryStream();
            var expected = new OscTimeTag();

            SerialiserFactory.TimeTagSerialiser.Encode(stream, expected);
            var data = stream.ToArray();

            int pos;
            var actual = SerialiserFactory.TimeTagSerialiser.Decode(data, 0, out pos);

            NUAssert.AreEqual(expected, actual);
        }

        /// <summary>
        ///     Tests the deserialisation for an immediate value.
        /// </summary>
        [Test]
        public void TestTimeTagSerialiserDecodeImmediateValue()
        {
            var data = new byte[8];
            data[7] = 1;

            int pos;
            var actual = SerialiserFactory.TimeTagSerialiser.Decode(data, 0, out pos);

            NUAssert.IsTrue(actual.IsImmediate);
            NUAssert.AreEqual(1UL, actual.TimeTag);
            NUAssert.AreEqual(DateTime.MinValue, actual.DateTime);
        }

        /// <summary>
        ///     Tests the serialisation for an immediate value.
        /// </summary>
        [Test]
        public void TestTimeTagSerialiserEncodeImmediateValue()
        {
            var stream = new MemoryStream();
            var timetag = new OscTimeTag();

            var actual = SerialiserFactory.TimeTagSerialiser.Encode(stream, timetag);
            var data = stream.ToArray();

            NUAssert.AreEqual(8, actual);

            for (var i = 0; i < 7; i++)
            {
                NUAssert.AreEqual(data[i], 0);
            }

            NUAssert.AreEqual(data[7], 1);
        }

        [Test]
        public void TestTrueSerialiser()
        {
            AssertSerialiserType(new TrueBooleanValueDeserialiser(), typeof (bool), 'T');
        }

        [Test]
        public void TestTrueSerialiserDecode()
        {
            var serialiser = new TrueBooleanValueDeserialiser();

            int pos;
            NUAssert.IsTrue(serialiser.Decode(new byte[1], 0, out pos));
            NUAssert.AreEqual(0, pos);
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TestTrueSerialiserEncode()
        {
            var serialiser = new TrueBooleanValueDeserialiser();
            serialiser.Encode(new MemoryStream(), true);
        }

        [Test]
        public void TestVersionSerialisation()
        {
            var version = new Version(1, 0, 2, 3);

            var serialiser = new VersionSerialiser();
            using (var s = new MemoryStream())
            {
                var l = serialiser.Encode(s, version);
                NUAssert.AreNotEqual(0, l);

                var bytes = s.ToArray();

                var v = serialiser.Decode(bytes, 0, out l);

                NUAssert.AreEqual(version, v);
            }
        }
    }
}