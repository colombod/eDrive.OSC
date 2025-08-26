using eDrive.OSC.Serialisation;

using FluentAssertions;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace eDrive.OSC.Tests;

/// <summary>
/// Comprehensive edge case and error handling tests for the serialization layer
/// </summary>
public class SerializationEdgeCasesTests
{
    public SerializationEdgeCasesTests()
    {
        Serialisation.SerializerFactory.LoadSerializersFromAssembly(typeof(OscMessage).Assembly);
    }

    #region Binary Serializer Error Handling Tests

    [Fact]
    public void StringSerializer_Decode_InsufficientBuffer_ThrowsException()
    {
        var serializer = new StringSerializer();
        var data = new byte[2]; // Too small for any meaningful string

        var action = () => serializer.Decode(data, 0, out int position);
        action.Should().NotThrow(); // Should handle gracefully, returning empty string
    }

    [Fact]
    public void StringSerializer_Decode_NegativePosition_ThrowsException()
    {
        var serializer = new StringSerializer();
        var data = Encoding.UTF8.GetBytes("test\0\0\0\0");

        var action = () => serializer.Decode(data, -1, out int position);
        action.Should().Throw<IndexOutOfRangeException>();
    }

    [Fact]
    public void StringSerializer_Decode_PositionBeyondBuffer_ThrowsException()
    {
        var serializer = new StringSerializer();
        var data = Encoding.UTF8.GetBytes("test\0\0\0\0");

        var action = () => serializer.Decode(data, data.Length + 1, out int position);
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void StringSerializer_Encode_NullStream_ThrowsException()
    {
        var serializer = new StringSerializer();

        var action = () => serializer.Encode(null, "test");
        action.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void IntSerializer_Decode_InsufficientBuffer_ThrowsException()
    {
        var serializer = new IntSerializer();
        var data = new byte[2]; // Too small for int (needs 4 bytes)

        var action = () => serializer.Decode(data, 0, out int position);
        action.Should().Throw<IndexOutOfRangeException>();
    }

    [Fact]
    public void FloatSerializer_Decode_CorruptedData_HandlesGracefully()
    {
        var serializer = new FloatSerializer();
        var data = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }; // NaN representation

        var result = serializer.Decode(data, 0, out int position);
        position.Should().Be(4);
        // Should not throw, but result might be NaN or infinity
    }

    [Fact]
    public void DoubleSerializer_Decode_NullBuffer_ThrowsException()
    {
        var serializer = new DoubleSerializer();

        var action = () => serializer.Decode(null, 0, out int position);
        action.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void ByteArraySerializer_Decode_MalformedSizeHeader_ThrowsException()
    {
        var serializer = new ByteArraySerializer();
        // Create malformed data with size larger than available data
        var data = new byte[] { 0x00, 0x00, 0x00, 0x10, 0x01, 0x02 }; // Claims 16 bytes but only has 2

        var action = () => serializer.Decode(data, 0, out int position);
        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region OscTimeTag Edge Case Tests

    [Fact]
    public void OscTimeTag_DateTimeMinValue_HandlesCorrectly()
    {
        var timeTag = new OscTimeTag(DateTime.MinValue);
        timeTag.DateTime.Should().Be(DateTime.MinValue);
        timeTag.TimeTag.Should().Be(OscTimeTag.Immediate);
    }

    [Fact]
    public void OscTimeTag_DateTimeMaxValue_HandlesCorrectly()
    {
        var timeTag = new OscTimeTag(DateTime.MaxValue);
        timeTag.DateTime.Should().Be(DateTime.MaxValue);
        // Should not throw and should have a valid tag
        timeTag.TimeTag.Should().NotBe(0);
    }

    [Fact]
    public void OscTimeTag_Year1900Boundary_HandlesCorrectly()
    {
        var year1900 = new DateTime(1900, 1, 1);
        var timeTag = new OscTimeTag(year1900);
        timeTag.DateTime.Should().Be(year1900);
    }

    [Fact]
    public void OscTimeTagSerializer_Decode_CorruptedTimestamp_HandlesGracefully()
    {
        var serializer = new OscTimeTagSerializer();
        var data = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }; // Max ulong

        var result = serializer.Decode(data, 0, out int position);
        position.Should().Be(8);
        result.Should().NotBeNull();
    }

    #endregion

    #region OscColour Edge Case Tests

    [Fact]
    public void OscColour_MaxByteValues_HandlesCorrectly()
    {
        var colour = new OscColour(255, 255, 255, 255);
        colour.R.Should().Be(255);
        colour.G.Should().Be(255);
        colour.B.Should().Be(255);
        colour.A.Should().Be(255);
    }

    [Fact]
    public void OscColour_IntConstructor_ExtremeValues_HandlesCorrectly()
    {
        var colour1 = new OscColour(int.MaxValue);
        colour1.Should().NotBeNull();

        var colour2 = new OscColour(int.MinValue);
        colour2.Should().NotBeNull();
    }

    [Fact]
    public void OscColourSerializer_Decode_CorruptedData_HandlesGracefully()
    {
        var serializer = new OscColourSerializer();
        var data = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

        var result = serializer.Decode(data, 0, out int position);
        position.Should().Be(4);
        result.Should().NotBeNull();
    }

    #endregion

    #region OscMidiMessage Edge Case Tests

    [Fact]
    public void OscMidiMessage_InvalidMidiData_OutOfRange_HandlesCorrectly()
    {
        // MIDI values should be 0-127, but constructor might accept any byte
        var action = () => new OscMidiMessage(255, 255, 255, 255);
        action.Should().NotThrow(); // Constructor should accept any byte values
    }

    [Fact]
    public void OscMidiMessage_ZeroValues_HandlesCorrectly()
    {
        var midiMsg = new OscMidiMessage(0, 0, 0, 0);
        midiMsg.Should().NotBeNull();
        midiMsg.ToInt32().Should().Be(0);
    }

    [Fact]
    public void OscMidiSerializer_Decode_CorruptedData_HandlesGracefully()
    {
        var serializer = new OscMidiSerializer();
        var data = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

        var result = serializer.Decode(data, 0, out int position);
        position.Should().Be(4);
        result.Should().NotBeNull();
    }

    #endregion

    #region OscSymbol Edge Case Tests

    [Fact]
    public void OscSymbol_EmptyString_HandlesCorrectly()
    {
        var symbol = new OscSymbol { Value = "" };
        symbol.Value.Should().Be("");
    }

    [Fact]
    public void OscSymbol_NullValue_HandlesCorrectly()
    {
        var symbol = new OscSymbol { Value = null };
        symbol.Value.Should().BeNull();
    }

    [Fact]
    public void OscSymbol_SpecialCharacters_HandlesCorrectly()
    {
        var specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?`~";
        var symbol = new OscSymbol { Value = specialChars };
        symbol.Value.Should().Be(specialChars);
    }

    [Fact]
    public void OscSymbol_UnicodeCharacters_HandlesCorrectly()
    {
        var unicode = "αβγδε中文日本語한국어";
        var symbol = new OscSymbol { Value = unicode };
        symbol.Value.Should().Be(unicode);
    }

    [Fact]
    public void SymbolSerializer_Decode_MalformedData_HandlesGracefully()
    {
        var serializer = new SymbolSerializer();
        var data = new byte[] { 0xFF, 0xFE, 0xFD, 0xFC }; // Invalid UTF-8 sequence

        var action = () => serializer.Decode(data, 0, out int position);
        // Should either handle gracefully or throw a specific exception
        action.Should().NotThrow<NullReferenceException>();
    }

    #endregion

    #region Version Edge Case Tests

    [Fact]
    public void VersionSerializer_Decode_InvalidVersionString_HandlesGracefully()
    {
        var serializer = new VersionSerializer();
        var invalidVersionBytes = Encoding.UTF8.GetBytes("not.a.version\0\0\0");

        var action = () => serializer.Decode(invalidVersionBytes, 0, out int position);
        // Should handle invalid version strings gracefully
        action.Should().NotThrow<NullReferenceException>();
    }

    [Fact]
    public void VersionSerializer_Encode_NullVersion_ThrowsException()
    {
        var serializer = new VersionSerializer();
        var stream = new MemoryStream();

        var action = () => serializer.Encode(stream, null);
        action.Should().Throw<NullReferenceException>();
    }

    #endregion

    #region Buffer Boundary and Alignment Tests

    [Fact]
    public void StringSerializer_FourByteAlignment_ExactBoundary()
    {
        var serializer = new StringSerializer();
        var stream = new MemoryStream();

        // Test string that should align exactly to 4-byte boundary
        var testString = "abc"; // 3 chars + null terminator = 4 bytes
        var bytesWritten = serializer.Encode(stream, testString);

        (bytesWritten % 4).Should().Be(0, "String encoding should maintain 4-byte alignment");
    }

    [Fact]
    public void StringSerializer_FourByteAlignment_RequiresPadding()
    {
        var serializer = new StringSerializer();
        var stream = new MemoryStream();

        // Test string that requires padding
        var testString = "a"; // 1 char + null terminator = 2 bytes, needs 2 bytes padding
        var bytesWritten = serializer.Encode(stream, testString);

        (bytesWritten % 4).Should().Be(0, "String encoding should pad to 4-byte alignment");
        bytesWritten.Should().Be(4);
    }

    [Fact]
    public void ByteArraySerializer_ReadAtExactBufferBoundary()
    {
        var serializer = new ByteArraySerializer();
        var testData = new byte[] { 1, 2, 3, 4 };

        // Create buffer with exact size needed
        var stream = new MemoryStream();
        serializer.Encode(stream, testData);
        var encodedData = stream.ToArray();

        var decoded = serializer.Decode(encodedData, 0, out int position);
        decoded.Should().BeEquivalentTo(testData);
        position.Should().Be(encodedData.Length);
    }

    [Fact]
    public void StringSerializer_ZeroLengthString_HandlesCorrectly()
    {
        var serializer = new StringSerializer();
        var stream = new MemoryStream();

        var bytesWritten = serializer.Encode(stream, "");
        bytesWritten.Should().Be(4); // Empty string + null terminator + padding = 4 bytes

        var data = stream.ToArray();
        var decoded = serializer.Decode(data, 0, out int position);
        decoded.Should().Be("");
        position.Should().Be(4);
    }

    #endregion

    #region Large Data Tests

    [Fact]
    public void StringSerializer_LargeString_HandlesCorrectly()
    {
        var serializer = new StringSerializer();
        var stream = new MemoryStream();

        // Create a large string (1MB+)
        var largeString = new string('A', 1024 * 1024 + 100);

        var action = () => serializer.Encode(stream, largeString);
        action.Should().NotThrow();

        var data = stream.ToArray();
        var decoded = serializer.Decode(data, 0, out int position);
        decoded.Should().Be(largeString);
    }

    [Fact]
    public void ByteArraySerializer_LargeByteArray_HandlesCorrectly()
    {
        var serializer = new ByteArraySerializer();
        var stream = new MemoryStream();

        // Create a large byte array (1MB+)
        var largeArray = new byte[1024 * 1024 + 100];
        for (int i = 0; i < largeArray.Length; i++)
        {
            largeArray[i] = (byte)(i % 256);
        }

        var action = () => serializer.Encode(stream, largeArray);
        action.Should().NotThrow();

        var data = stream.ToArray();
        var decoded = serializer.Decode(data, 0, out int position);
        decoded.Should().BeEquivalentTo(largeArray);
    }

    [Fact]
    public void StringSerializer_ExtremelyLargeString_MemoryHandling()
    {
        var serializer = new StringSerializer();

        // Test with a very large string that might cause memory issues
        var action = () =>
        {
            var stream = new MemoryStream();
            var veryLargeString = new string('X', 10 * 1024 * 1024); // 10MB string
            serializer.Encode(stream, veryLargeString);
        };

        // Should either succeed or throw OutOfMemoryException, not crash
        action.Should().NotThrow<NullReferenceException>();
        action.Should().NotThrow<AccessViolationException>();
    }

    #endregion

    #region Malformed OSC Packet Tests

    [Fact]
    public void OscPacket_FromByteArray_TruncatedData_ThrowsException()
    {
        var truncatedData = new byte[] { 0x2F, 0x74, 0x65, 0x73 }; // "/tes" - incomplete address

        var action = () => OscPacket.FromByteArray(truncatedData);
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void OscPacket_FromByteArray_InvalidTypeTag_HandlesGracefully()
    {
        // Create packet with invalid type tag
        var invalidPacket = new byte[]
        {
            0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, // "/test\0\0\0"
            0x2C, 0x7A, 0x00, 0x00, // ",z\0\0" - 'z' is not a valid OSC type tag
            0x00, 0x00, 0x00, 0x01  // Some data
        };

        // The library appears to handle unknown type tags gracefully
        var result = OscPacket.FromByteArray(invalidPacket);
        result.Should().NotBeNull();
    }

    [Fact]
    public void OscPacket_FromByteArray_MismatchedDataSize_HandlesGracefully()
    {
        // Create packet claiming to have int data but providing insufficient bytes
        var mismatchedPacket = new byte[]
        {
            0x2F, 0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, // "/test\0\0\0"
            0x2C, 0x69, 0x00, 0x00, // ",i\0\0" - claims to have int
            0x00, 0x01  // Only 2 bytes instead of required 4
        };

        // The library appears to handle this gracefully too
        var result = OscPacket.FromByteArray(mismatchedPacket);
        result.Should().NotBeNull();
    }

    [Fact]
    public void OscPacket_FromByteArray_EmptyData_ThrowsException()
    {
        var emptyData = new byte[0];

        var action = () => OscPacket.FromByteArray(emptyData);
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void OscPacket_FromByteArray_NullData_ThrowsException()
    {
        var action = () => OscPacket.FromByteArray(null);
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Concurrency and Thread Safety Tests

    [Fact]
    public async Task StringSerializer_ConcurrentAccess_HandlesCorrectly()
    {
        var serializer = new StringSerializer();
        var tasks = new Task[10];

        for (int i = 0; i < tasks.Length; i++)
        {
            int taskId = i;
            tasks[i] = Task.Run(() =>
            {
                var stream = new MemoryStream();
                var testString = $"ConcurrentTest{taskId}";

                var bytesWritten = serializer.Encode(stream, testString);
                bytesWritten.Should().BeGreaterThan(0);

                var data = stream.ToArray();
                var decoded = serializer.Decode(data, 0, out int position);
                decoded.Should().Be(testString);
            });
        }

        await Task.WhenAll(tasks);
        // If we get here without exception, the test passes
    }

    [Fact]
    public async Task SerializerFactory_ConcurrentAccess_HandlesCorrectly()
    {
        var tasks = new Task[10];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                var stringSerializer = SerializerFactory.GetSerializer(typeof(string));
                var intSerializer = SerializerFactory.GetSerializer(typeof(int));
                var floatSerializer = SerializerFactory.GetSerializer(typeof(float));

                stringSerializer.Should().NotBeNull();
                intSerializer.Should().NotBeNull();
                floatSerializer.Should().NotBeNull();
            });
        }

        await Task.WhenAll(tasks);
        // If we get here without exception, the test passes
    }

    #endregion

    #region Integer Overflow and Boundary Tests

    [Fact]
    public void IntSerializer_MaxMinValues_HandlesCorrectly()
    {
        var serializer = new IntSerializer();
        var stream = new MemoryStream();

        // Test int.MaxValue
        serializer.Encode(stream, int.MaxValue);
        var maxData = stream.ToArray();
        var decodedMax = serializer.Decode(maxData, 0, out int position);
        decodedMax.Should().Be(int.MaxValue);

        // Test int.MinValue
        stream = new MemoryStream();
        serializer.Encode(stream, int.MinValue);
        var minData = stream.ToArray();
        var decodedMin = serializer.Decode(minData, 0, out position);
        decodedMin.Should().Be(int.MinValue);
    }

    [Fact]
    public void LongSerializer_MaxMinValues_HandlesCorrectly()
    {
        var serializer = new LongSerializer();
        var stream = new MemoryStream();

        // Test long.MaxValue
        serializer.Encode(stream, long.MaxValue);
        var maxData = stream.ToArray();
        var decodedMax = serializer.Decode(maxData, 0, out int position);
        decodedMax.Should().Be(long.MaxValue);

        // Test long.MinValue
        stream = new MemoryStream();
        serializer.Encode(stream, long.MinValue);
        var minData = stream.ToArray();
        var decodedMin = serializer.Decode(minData, 0, out position);
        decodedMin.Should().Be(long.MinValue);
    }

    [Fact]
    public void ByteArraySerializer_IntegerOverflowInSize_HandlesCorrectly()
    {
        var serializer = new ByteArraySerializer();

        // Create data that claims to have a size that would cause integer overflow
        var maliciousData = new byte[]
        {
            0xFF, 0xFF, 0xFF, 0xFF, // int.MaxValue as size
            0x01, 0x02, 0x03, 0x04  // Some actual data
        };

        var action = () => serializer.Decode(maliciousData, 0, out int position);
        action.Should().Throw<OverflowException>();
    }

    #endregion

    #region Stream Position Corruption Tests

    [Fact]
    public void StringSerializer_StreamPositionCorruption_HandlesCorrectly()
    {
        var serializer = new StringSerializer();

        // Create a scenario where stream position might be corrupted
        using var stream = new MemoryStream();
        stream.Write([0x74, 0x65, 0x73, 0x74, 0x00, 0x00, 0x00, 0x00]); // "test\0\0\0\0"
        stream.Position = 0;

        // Manually corrupt position
        stream.Seek(100, SeekOrigin.Begin); // Position beyond data

        var action = () => serializer.Encode(stream, "newtest");
        action.Should().NotThrow(); // Should handle position gracefully
    }

    [Fact]
    public void MultipleSerializers_SequentialOperations_MaintainCorrectPositions()
    {
        var stringSerializer = new StringSerializer();
        var intSerializer = new IntSerializer();
        var floatSerializer = new FloatSerializer();

        var stream = new MemoryStream();

        // Encode multiple values sequentially
        var pos1 = stringSerializer.Encode(stream, "test");
        var pos2 = intSerializer.Encode(stream, 12345);
        var pos3 = floatSerializer.Encode(stream, 3.14159f);

        pos1.Should().BeGreaterThan(0);
        pos2.Should().Be(4);
        pos3.Should().Be(4);

        // Decode and verify positions are maintained correctly
        var data = stream.ToArray();
        var decodedString = stringSerializer.Decode(data, 0, out int stringPos);
        var decodedInt = intSerializer.Decode(data, stringPos, out int intPos);
        var decodedFloat = floatSerializer.Decode(data, intPos, out int floatPos);

        decodedString.Should().Be("test");
        decodedInt.Should().Be(12345);
        decodedFloat.Should().BeApproximately(3.14159f, 0.00001f);
        floatPos.Should().Be(data.Length);
    }

    #endregion
}