using eDrive.OSC.Serialisation.Json;

using FluentAssertions;

using Newtonsoft.Json;

using System;
using System.IO;

using Xunit;

namespace eDrive.OSC.Tests;

public class JsonSerialisersTests
{
    public JsonSerialisersTests()
    {
        Serialisation.Json.JsonSerializerFactory.LoadSerializersFromAssembly(typeof(OscMessage).Assembly);
    }

    #region Helper Methods

    private static void AssertSerializerType(IOscTypeJsonSerializer serializer, Type type, char tag)
    {
        serializer.Type.Should().Be(type);
        serializer.Tag.Should().Be(tag);
    }

    private static void AssertJsonEncode<T>(OscTypeJsonSerializer<T> serializer, T value, string expectedJson)
    {
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);

        serializer.Encode(jsonWriter, value);

        var actualJson = stringWriter.ToString();
        actualJson.Should().Be(expectedJson);
    }

    private static void AssertJsonDecode<T>(IOscTypeJsonSerializer<T> serializer, string json, T expected)
    {
        var stringReader = new StringReader(json);
        var jsonReader = new JsonTextReader(stringReader);

        var actual = serializer.Decode(jsonReader);
        
        if (typeof(T) == typeof(byte[]))
        {
            actual.Should().BeEquivalentTo(expected);
        }
        else
        {
            actual.Should().Be(expected);
        }
    }

    private static void AssertJsonRoundTrip<T>(OscTypeJsonSerializer<T> serializer, T value)
    {
        // Encode to JSON
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Encode(jsonWriter, value);
        var json = stringWriter.ToString();

        // Decode from JSON
        var stringReader = new StringReader(json);
        var jsonReader = new JsonTextReader(stringReader);
        var decoded = serializer.Decode(jsonReader);

        if (typeof(T) == typeof(byte[]))
        {
            decoded.Should().BeEquivalentTo(value);
        }
        else
        {
            decoded.Should().Be(value);
        }
    }

    #endregion

    #region BooleanValueSerialiser Tests

    [Fact]
    public void TestBooleanValueSerializer_Type()
    {
        AssertSerializerType(new BooleanValueSerializer(), typeof(bool), ' ');
    }

    [Fact]
    public void TestBooleanValueSerializer_GetTag_True()
    {
        var serializer = new BooleanValueSerializer();
        serializer.GetTag(true).Should().Be('T');
    }

    [Fact]
    public void TestBooleanValueSerializer_GetTag_False()
    {
        var serializer = new BooleanValueSerializer();
        serializer.GetTag(false).Should().Be('F');
    }

    [Fact]
    public void TestBooleanValueSerializer_Encode_True()
    {
        var serializer = new BooleanValueSerializer();
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);

        serializer.Encode(jsonWriter, true);

        // Boolean serializer writes no content, only uses tag
        stringWriter.ToString().Should().BeEmpty();
    }

    [Fact]
    public void TestBooleanValueSerializer_Encode_False()
    {
        var serializer = new BooleanValueSerializer();
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);

        serializer.Encode(jsonWriter, false);

        // Boolean serializer writes no content, only uses tag
        stringWriter.ToString().Should().BeEmpty();
    }

    [Fact]
    public void TestBooleanValueSerializer_Decode_ThrowsNotSupported()
    {
        var serializer = new BooleanValueSerializer();
        var stringReader = new StringReader("true");
        var jsonReader = new JsonTextReader(stringReader);

        var action = new Action(() => serializer.Decode(jsonReader));
        action.Should().Throw<NotSupportedException>();
    }

    #endregion

    #region TrueBooleanValueDeserializer Tests

    [Fact]
    public void TestTrueBooleanValueDeserializer_Type()
    {
        AssertSerializerType(new TrueBooleanValueDeserializer(), typeof(bool), 'T');
    }

    [Fact]
    public void TestTrueBooleanValueDeserializer_Decode()
    {
        var serializer = new TrueBooleanValueDeserializer();
        var stringReader = new StringReader("null");
        var jsonReader = new JsonTextReader(stringReader);

        var result = serializer.Decode(jsonReader);
        result.Should().BeTrue();
    }

    [Fact]
    public void TestTrueBooleanValueDeserializer_Encode_ThrowsNotSupported()
    {
        var serializer = new TrueBooleanValueDeserializer();
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);

        var action = new Action(() => serializer.Encode(jsonWriter, true));
        action.Should().Throw<NotSupportedException>();
    }

    #endregion

    #region FalseBooleanValueDeserializer Tests

    [Fact]
    public void TestFalseBooleanValueDeserializer_Type()
    {
        AssertSerializerType(new FalseBooleanValueDeserializer(), typeof(bool), 'F');
    }

    [Fact]
    public void TestFalseBooleanValueDeserializer_Decode()
    {
        var serializer = new FalseBooleanValueDeserializer();
        var stringReader = new StringReader("null");
        var jsonReader = new JsonTextReader(stringReader);

        var result = serializer.Decode(jsonReader);
        result.Should().BeFalse();
    }

    [Fact]
    public void TestFalseBooleanValueDeserializer_Encode_ThrowsNotSupported()
    {
        var serializer = new FalseBooleanValueDeserializer();
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);

        var action = new Action(() => serializer.Encode(jsonWriter, false));
        action.Should().Throw<NotSupportedException>();
    }

    #endregion

    #region IntSerialiser Tests

    [Fact]
    public void TestIntSerializer_Type()
    {
        AssertSerializerType(new IntSerializer(), typeof(int), 'i');
    }

    [Fact]
    public void TestIntSerializer_Encode()
    {
        AssertJsonEncode(new IntSerializer(), 42, "42");
        AssertJsonEncode(new IntSerializer(), -123, "-123");
        AssertJsonEncode(new IntSerializer(), 0, "0");
        AssertJsonEncode(new IntSerializer(), int.MaxValue, int.MaxValue.ToString());
        AssertJsonEncode(new IntSerializer(), int.MinValue, int.MinValue.ToString());
    }

    [Fact]
    public void TestIntSerializer_Decode()
    {
        AssertJsonDecode(new IntSerializer(), "42", 42);
        AssertJsonDecode(new IntSerializer(), "-123", -123);
        AssertJsonDecode(new IntSerializer(), "0", 0);
    }

    [Fact]
    public void TestIntSerializer_RoundTrip()
    {
        var serializer = new IntSerializer();
        AssertJsonRoundTrip(serializer, 42);
        AssertJsonRoundTrip(serializer, -123);
        AssertJsonRoundTrip(serializer, 0);
        AssertJsonRoundTrip(serializer, int.MaxValue);
        AssertJsonRoundTrip(serializer, int.MinValue);
    }

    #endregion

    #region FloatSerialiser Tests

    [Fact]
    public void TestFloatSerializer_Type()
    {
        AssertSerializerType(new FloatSerializer(), typeof(float), 'f');
    }

    [Fact]
    public void TestFloatSerializer_Encode()
    {
        AssertJsonEncode(new FloatSerializer(), 3.14f, "3.14");
        AssertJsonEncode(new FloatSerializer(), -2.5f, "-2.5");
        AssertJsonEncode(new FloatSerializer(), 0.0f, "0.0");
    }

    [Fact]
    public void TestFloatSerializer_Decode()
    {
        AssertJsonDecode(new FloatSerializer(), "3.14", 3.14f);
        AssertJsonDecode(new FloatSerializer(), "-2.5", -2.5f);
        AssertJsonDecode(new FloatSerializer(), "0.0", 0.0f);
    }

    [Fact]
    public void TestFloatSerializer_RoundTrip()
    {
        var serializer = new FloatSerializer();
        AssertJsonRoundTrip(serializer, 3.14f);
        AssertJsonRoundTrip(serializer, -2.5f);
        AssertJsonRoundTrip(serializer, 0.0f);
        AssertJsonRoundTrip(serializer, 1000.5f);
        AssertJsonRoundTrip(serializer, -1000.5f);
        
        // Test special float values - now that FloatSerializer handles them properly
        AssertJsonRoundTrip(serializer, float.PositiveInfinity);
        AssertJsonRoundTrip(serializer, float.NegativeInfinity);
        
        // NaN requires special handling since NaN != NaN
        var nan = float.NaN;
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Encode(jsonWriter, nan);
        var json = stringWriter.ToString();
        
        var stringReader = new StringReader(json);
        var jsonReader = new JsonTextReader(stringReader);
        var decoded = serializer.Decode(jsonReader);
        
        float.IsNaN(decoded).Should().BeTrue();
    }

    [Fact]
    public void TestFloatSerializer_SpecialValues()
    {
        var serializer = new FloatSerializer();
        
        // Test positive infinity
        var posInfinity = float.PositiveInfinity;
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Encode(jsonWriter, posInfinity);
        var json = stringWriter.ToString();
        
        // Check what JSON is produced for infinity
        json.Should().NotBeNullOrEmpty();
        
        // Test if we can decode it back
        var stringReader = new StringReader(json);
        var jsonReader = new JsonTextReader(stringReader);
        var decoded = serializer.Decode(jsonReader);
        
        // Verify the result
        decoded.Should().Be(posInfinity);
        
        // Test negative infinity
        AssertJsonRoundTrip(serializer, float.NegativeInfinity);
        
        // Test NaN
        var nan = float.NaN;
        stringWriter = new StringWriter();
        jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Encode(jsonWriter, nan);
        var nanJson = stringWriter.ToString();
        
        stringReader = new StringReader(nanJson);
        jsonReader = new JsonTextReader(stringReader);
        var decodedNaN = serializer.Decode(jsonReader);
        
        // NaN != NaN, so we need to check IsNaN
        float.IsNaN(decodedNaN).Should().BeTrue();
    }

    #endregion

    #region DoubleSerialiser Tests

    [Fact]
    public void TestDoubleSerializer_Type()
    {
        AssertSerializerType(new DoubleSerializer(), typeof(double), 'd');
    }

    [Fact]
    public void TestDoubleSerializer_Encode()
    {
        AssertJsonEncode(new DoubleSerializer(), 3.141592653589793, "3.141592653589793");
        AssertJsonEncode(new DoubleSerializer(), -2.718281828459045, "-2.718281828459045");
        AssertJsonEncode(new DoubleSerializer(), 0.0, "0.0");
    }

    [Fact]
    public void TestDoubleSerializer_Decode()
    {
        AssertJsonDecode(new DoubleSerializer(), "3.141592653589793", 3.141592653589793);
        AssertJsonDecode(new DoubleSerializer(), "-2.718281828459045", -2.718281828459045);
        AssertJsonDecode(new DoubleSerializer(), "0.0", 0.0);
    }

    [Fact]
    public void TestDoubleSerializer_RoundTrip()
    {
        var serializer = new DoubleSerializer();
        AssertJsonRoundTrip(serializer, 3.141592653589793);
        AssertJsonRoundTrip(serializer, -2.718281828459045);
        AssertJsonRoundTrip(serializer, 0.0);
        AssertJsonRoundTrip(serializer, double.MaxValue);
        AssertJsonRoundTrip(serializer, double.MinValue);
        
        // Test special double values
        AssertJsonRoundTrip(serializer, double.PositiveInfinity);
        AssertJsonRoundTrip(serializer, double.NegativeInfinity);
        
        // NaN requires special handling since NaN != NaN
        var nan = double.NaN;
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Encode(jsonWriter, nan);
        var json = stringWriter.ToString();
        
        var stringReader = new StringReader(json);
        var jsonReader = new JsonTextReader(stringReader);
        var decoded = serializer.Decode(jsonReader);
        
        double.IsNaN(decoded).Should().BeTrue();
    }

    #endregion

    #region LongSerialiser Tests

    [Fact]
    public void TestLongSerializer_Type()
    {
        AssertSerializerType(new LongSerializer(), typeof(long), 'h');
    }

    [Fact]
    public void TestLongSerializer_Encode()
    {
        AssertJsonEncode(new LongSerializer(), 1234567890123456789L, "1234567890123456789");
        AssertJsonEncode(new LongSerializer(), -987654321098765432L, "-987654321098765432");
        AssertJsonEncode(new LongSerializer(), 0L, "0");
    }

    [Fact]
    public void TestLongSerializer_Decode()
    {
        AssertJsonDecode(new LongSerializer(), "1234567890123456789", 1234567890123456789L);
        AssertJsonDecode(new LongSerializer(), "-987654321098765432", -987654321098765432L);
        AssertJsonDecode(new LongSerializer(), "0", 0L);
    }

    [Fact]
    public void TestLongSerializer_RoundTrip()
    {
        var serializer = new LongSerializer();
        AssertJsonRoundTrip(serializer, 1234567890123456789L);
        AssertJsonRoundTrip(serializer, -987654321098765432L);
        AssertJsonRoundTrip(serializer, 0L);
        AssertJsonRoundTrip(serializer, long.MaxValue);
        AssertJsonRoundTrip(serializer, long.MinValue);
    }

    #endregion

    #region CharSerialiser Tests

    [Fact]
    public void TestCharSerializer_Type()
    {
        AssertSerializerType(new CharSerializer(), typeof(char), 'c');
    }

    [Fact]
    public void TestCharSerializer_Encode()
    {
        AssertJsonEncode(new CharSerializer(), 'A', "\"A\"");
        AssertJsonEncode(new CharSerializer(), 'z', "\"z\"");
        AssertJsonEncode(new CharSerializer(), '0', "\"0\"");
        AssertJsonEncode(new CharSerializer(), ' ', "\" \"");
    }

    [Fact]
    public void TestCharSerializer_Decode()
    {
        AssertJsonDecode(new CharSerializer(), "\"A\"", 'A');
        AssertJsonDecode(new CharSerializer(), "\"z\"", 'z');
        AssertJsonDecode(new CharSerializer(), "\"0\"", '0');
        AssertJsonDecode(new CharSerializer(), "\" \"", ' ');
    }

    [Fact]
    public void TestCharSerializer_RoundTrip()
    {
        var serializer = new CharSerializer();
        AssertJsonRoundTrip(serializer, 'A');
        AssertJsonRoundTrip(serializer, 'z');
        AssertJsonRoundTrip(serializer, '0');
        AssertJsonRoundTrip(serializer, ' ');
        AssertJsonRoundTrip(serializer, '\n');
        AssertJsonRoundTrip(serializer, '\t');
    }

    #endregion

    #region StringSerialiser Tests

    [Fact]
    public void TestStringSerializer_Type()
    {
        AssertSerializerType(new StringSerializer(), typeof(string), 's');
    }

    [Fact]
    public void TestStringSerializer_Encode()
    {
        AssertJsonEncode(new StringSerializer(), "Hello World", "\"Hello World\"");
        AssertJsonEncode(new StringSerializer(), "", "\"\"");
        AssertJsonEncode(new StringSerializer(), "Test with \"quotes\"", "\"Test with \\\"quotes\\\"\"");
        AssertJsonEncode(new StringSerializer(), "Line1\nLine2", "\"Line1\\nLine2\"");
    }

    [Fact]
    public void TestStringSerializer_Decode()
    {
        AssertJsonDecode(new StringSerializer(), "\"Hello World\"", "Hello World");
        AssertJsonDecode(new StringSerializer(), "\"\"", "");
        AssertJsonDecode(new StringSerializer(), "\"Test with \\\"quotes\\\"\"", "Test with \"quotes\"");
        AssertJsonDecode(new StringSerializer(), "\"Line1\\nLine2\"", "Line1\nLine2");
    }

    [Fact]
    public void TestStringSerializer_RoundTrip()
    {
        var serializer = new StringSerializer();
        AssertJsonRoundTrip(serializer, "Hello World");
        AssertJsonRoundTrip(serializer, "");
        AssertJsonRoundTrip(serializer, "Test with \"quotes\"");
        AssertJsonRoundTrip(serializer, "Line1\nLine2\tTab");
    }

    [Fact]
    public void TestStringSerializer_Null()
    {
        var serializer = new StringSerializer();
        AssertJsonEncode(serializer, null, "null");
        AssertJsonDecode(serializer, "null", null);
    }

    #endregion

    #region SymbolSerialiser Tests

    [Fact]
    public void TestSymbolSerializer_Type()
    {
        AssertSerializerType(new SymbolSerializer(), typeof(OscSymbol), 'S');
    }

    [Fact]
    public void TestSymbolSerializer_Encode()
    {
        var symbol = new OscSymbol { Value = "test_symbol" };
        AssertJsonEncode(new SymbolSerializer(), symbol, "\"test_symbol\"");

        var emptySymbol = new OscSymbol { Value = "" };
        AssertJsonEncode(new SymbolSerializer(), emptySymbol, "\"\"");
    }

    [Fact]
    public void TestSymbolSerializer_Decode()
    {
        var expected = new OscSymbol { Value = "test_symbol" };
        AssertJsonDecode(new SymbolSerializer(), "\"test_symbol\"", expected);

        var expectedEmpty = new OscSymbol { Value = "" };
        AssertJsonDecode(new SymbolSerializer(), "\"\"", expectedEmpty);
    }

    [Fact]
    public void TestSymbolSerializer_RoundTrip()
    {
        var serializer = new SymbolSerializer();
        var symbol1 = new OscSymbol { Value = "test_symbol" };
        var symbol2 = new OscSymbol { Value = "another.symbol" };
        var symbol3 = new OscSymbol { Value = "" };

        AssertJsonRoundTrip(serializer, symbol1);
        AssertJsonRoundTrip(serializer, symbol2);
        AssertJsonRoundTrip(serializer, symbol3);
    }

    #endregion

    #region ByteArraySerialiser Tests

    [Fact]
    public void TestByteArraySerializer_Type()
    {
        AssertSerializerType(new ByteArraySerializer(), typeof(byte[]), 'b');
    }

    [Fact]
    public void TestByteArraySerializer_Encode()
    {
        var bytes = new byte[] { 0x01, 0x02, 0x03, 0xFF };
        AssertJsonEncode(new ByteArraySerializer(), bytes, "[1,2,3,255]");

        var emptyBytes = new byte[0];
        AssertJsonEncode(new ByteArraySerializer(), emptyBytes, "[]");
    }

    [Fact]
    public void TestByteArraySerializer_Decode()
    {
        var serializer = new ByteArraySerializer();
        
        // Test decoding empty array
        AssertJsonDecode(serializer, "[]", new byte[0]);
        
        // Test decoding small array
        AssertJsonDecode(serializer, "[1,2,3,255]", new byte[] { 0x01, 0x02, 0x03, 0xFF });
        
        // Test decoding array with zero values
        AssertJsonDecode(serializer, "[0,0,0]", new byte[] { 0x00, 0x00, 0x00 });
        
        // Test decoding array with max byte values
        AssertJsonDecode(serializer, "[255,254,253]", new byte[] { 0xFF, 0xFE, 0xFD });
        
        // Test decoding single byte
        AssertJsonDecode(serializer, "[42]", new byte[] { 42 });
    }

    [Fact]
    public void TestByteArraySerializer_RoundTrip()
    {
        var serializer = new ByteArraySerializer();
        
        // Test empty byte array round trip
        AssertJsonRoundTrip(serializer, new byte[0]);
        
        // Test small byte array round trip
        AssertJsonRoundTrip(serializer, new byte[] { 0x01, 0x02, 0x03, 0xFF });
        
        // Test array with zero values
        AssertJsonRoundTrip(serializer, new byte[] { 0x00, 0x00, 0x00 });
        
        // Test array with max byte values
        AssertJsonRoundTrip(serializer, new byte[] { 0xFF, 0xFE, 0xFD });
        
        // Test single byte
        AssertJsonRoundTrip(serializer, new byte[] { 42 });
        
        // Test larger array with various values
        var largeArray = new byte[256];
        for (int i = 0; i < 256; i++)
        {
            largeArray[i] = (byte)i;
        }
        AssertJsonRoundTrip(serializer, largeArray);
        
        // Test random byte values
        var randomArray = new byte[] { 17, 89, 123, 200, 0, 255, 1, 254 };
        AssertJsonRoundTrip(serializer, randomArray);
    }

    [Fact]
    public void TestByteArraySerializer_Null()
    {
        var serializer = new ByteArraySerializer();
        AssertJsonEncode(serializer, null, "[]");
    }

    #endregion

    #region GuidSerialiser Tests

    [Fact]
    public void TestGuidSerializer_Type()
    {
        AssertSerializerType(new GuidSerializer(), typeof(Guid), 'g');
    }

    [Fact]
    public void TestGuidSerializer_Encode()
    {
        var guid = new Guid("12345678-1234-5678-9abc-123456789abc");
        AssertJsonEncode(new GuidSerializer(), guid, "\"12345678-1234-5678-9abc-123456789abc\"");

        AssertJsonEncode(new GuidSerializer(), Guid.Empty, "\"00000000-0000-0000-0000-000000000000\"");
    }

    [Fact]
    public void TestGuidSerializer_Decode()
    {
        var expected = new Guid("12345678-1234-5678-9abc-123456789abc");
        AssertJsonDecode(new GuidSerializer(), "\"12345678-1234-5678-9abc-123456789abc\"", expected);

        AssertJsonDecode(new GuidSerializer(), "\"00000000-0000-0000-0000-000000000000\"", Guid.Empty);
    }

    [Fact]
    public void TestGuidSerializer_RoundTrip()
    {
        var serializer = new GuidSerializer();
        var guid1 = new Guid("12345678-1234-5678-9abc-123456789abc");
        var guid2 = Guid.NewGuid();

        AssertJsonRoundTrip(serializer, guid1);
        AssertJsonRoundTrip(serializer, guid2);
        AssertJsonRoundTrip(serializer, Guid.Empty);
    }

    #endregion

    #region NilSerialiser Tests

    [Fact]
    public void TestNilSerializer_Type()
    {
        AssertSerializerType(new NilSerializer(), typeof(NilSerializer.Nil), 'N');
    }

    [Fact]
    public void TestNilSerializer_Encode()
    {
        var nil = new NilSerializer.Nil();
        AssertJsonEncode(new NilSerializer(), nil, "");
        AssertJsonEncode(new NilSerializer(), null, "");
    }

    [Fact]
    public void TestNilSerializer_Decode()
    {
        // NilSerializer.Decode() returns null, not a Nil instance
        AssertJsonDecode(new NilSerializer(), "null", null);
    }

    [Fact]
    public void TestNilSerializer_RoundTrip()
    {
        // Skip round trip test for Nil as Encode produces empty string and Decode expects null
        var serializer = new NilSerializer();

        // Test that encoding produces empty string
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Encode(jsonWriter, new NilSerializer.Nil());
        stringWriter.ToString().Should().BeEmpty();

        // Test that decoding null returns null
        var stringReader = new StringReader("null");
        var jsonReader = new JsonTextReader(stringReader);
        var decoded = serializer.Decode(jsonReader);
        decoded.Should().BeNull();
    }

    #endregion

    #region OscColourSerialiser Tests

    [Fact]
    public void TestOscColourSerializer_Type()
    {
        AssertSerializerType(new OscColourSerializer(), typeof(OscColour), 'r');
    }

    [Fact]
    public void TestOscColourSerializer_Encode()
    {
        var color = new OscColour(255, 128, 64, 32);
        AssertJsonEncode(new OscColourSerializer(), color, "[255,128,64,32]");

        var blackColor = new OscColour(0, 0, 0, 0);
        AssertJsonEncode(new OscColourSerializer(), blackColor, "[0,0,0,0]");
    }

    [Fact]
    public void TestOscColourSerializer_Decode()
    {
        var expected = new OscColour(255, 128, 64, 32);
        AssertJsonDecode(new OscColourSerializer(), "[255,128,64,32]", expected);

        var expectedBlack = new OscColour(0, 0, 0, 0);
        AssertJsonDecode(new OscColourSerializer(), "[0,0,0,0]", expectedBlack);
    }

    [Fact]
    public void TestOscColourSerializer_RoundTrip()
    {
        var serializer = new OscColourSerializer();
        var color1 = new OscColour(255, 128, 64, 32);
        var color2 = new OscColour(0, 0, 0, 255);
        var color3 = new OscColour(255, 255, 255, 255);

        AssertJsonRoundTrip(serializer, color1);
        AssertJsonRoundTrip(serializer, color2);
        AssertJsonRoundTrip(serializer, color3);
    }

    #endregion

    #region OscMidiSerialiser Tests

    [Fact]
    public void TestOscMidiSerializer_Type()
    {
        AssertSerializerType(new OscMidiSerializer(), typeof(OscMidiMessage), 'm');
    }

    [Fact]
    public void TestOscMidiSerializer_Encode()
    {
        var midi = new OscMidiMessage(1, 144, 60, 127);
        AssertJsonEncode(new OscMidiSerializer(), midi, "[1,144,60,127]");

        var midiZero = new OscMidiMessage(0, 0, 0, 0);
        AssertJsonEncode(new OscMidiSerializer(), midiZero, "[0,0,0,0]");
    }

    [Fact]
    public void TestOscMidiSerializer_Decode()
    {
        var expected = new OscMidiMessage(1, 144, 60, 127);
        AssertJsonDecode(new OscMidiSerializer(), "[1,144,60,127]", expected);

        var expectedZero = new OscMidiMessage(0, 0, 0, 0);
        AssertJsonDecode(new OscMidiSerializer(), "[0,0,0,0]", expectedZero);
    }

    [Fact]
    public void TestOscMidiSerializer_RoundTrip()
    {
        var serializer = new OscMidiSerializer();
        var midi1 = new OscMidiMessage(1, 144, 60, 127);
        var midi2 = new OscMidiMessage(2, 128, 60, 0);
        var midi3 = new OscMidiMessage(0, 176, 7, 100);

        AssertJsonRoundTrip(serializer, midi1);
        AssertJsonRoundTrip(serializer, midi2);
        AssertJsonRoundTrip(serializer, midi3);
    }

    #endregion

    #region OscTimeTagSerialiser Tests

    [Fact]
    public void TestOscTimeTagSerializer_Type()
    {
        AssertSerializerType(new OscTimeTagSerializer(), typeof(OscTimeTag), 't');
    }

    [Fact]
    public void TestOscTimeTagSerializer_Encode()
    {
        var timeTag = new OscTimeTag(DateTime.Parse("2023-01-01T12:00:00Z").ToUniversalTime());
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);

        new OscTimeTagSerializer().Encode(jsonWriter, timeTag);

        var json = stringWriter.ToString();
        json.Should().NotBeNullOrEmpty();
        // OscTimeTagSerializer encodes as a number (TimeTag property), not a quoted string
        ulong.TryParse(json, out _).Should().BeTrue("JSON should be a valid ulong number");
    }

    [Fact]
    public void TestOscTimeTagSerializer_RoundTrip()
    {
        var serializer = new OscTimeTagSerializer();
        var timeTag1 = new OscTimeTag(DateTime.Parse("2023-01-01T12:00:00Z").ToUniversalTime());
        var timeTag2 = new OscTimeTag(DateTime.Parse("2024-12-31T23:59:59Z").ToUniversalTime());

        // Test round trip for fixed dates (avoid precision issues with DateTime.UtcNow)
        AssertJsonRoundTrip(serializer, timeTag1);
        AssertJsonRoundTrip(serializer, timeTag2);

        // Test encoding/decoding separately for current time due to precision differences
        var timeTag3 = new OscTimeTag(DateTime.UtcNow);
        var stringWriter = new StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);
        serializer.Encode(jsonWriter, timeTag3);
        var json = stringWriter.ToString();

        var stringReader = new StringReader(json);
        var jsonReader = new JsonTextReader(stringReader);
        var decoded = serializer.Decode(jsonReader);

        // Verify the TimeTag values are the same (they should be exact)
        decoded.TimeTag.Should().Be(timeTag3.TimeTag);
    }

    #endregion

    #region VersionSerialiser Tests

    [Fact]
    public void TestVersionSerializer_Type()
    {
        AssertSerializerType(new VersionSerializer(), typeof(Version), 'v');
    }

    [Fact]
    public void TestVersionSerializer_Encode()
    {
        var version = new Version(1, 2, 3, 4);
        AssertJsonEncode(new VersionSerializer(), version, "\"1.2.3.4\"");

        var simpleVersion = new Version(2, 0);
        AssertJsonEncode(new VersionSerializer(), simpleVersion, "\"2.0\"");
    }

    [Fact]
    public void TestVersionSerializer_Decode()
    {
        var expected = new Version(1, 2, 3, 4);
        AssertJsonDecode(new VersionSerializer(), "\"1.2.3.4\"", expected);

        var expectedSimple = new Version(2, 0);
        AssertJsonDecode(new VersionSerializer(), "\"2.0\"", expectedSimple);
    }

    [Fact]
    public void TestVersionSerializer_RoundTrip()
    {
        var serializer = new VersionSerializer();
        var version1 = new Version(1, 2, 3, 4);
        var version2 = new Version(2, 0);
        var version3 = new Version(10, 5, 1);

        AssertJsonRoundTrip(serializer, version1);
        AssertJsonRoundTrip(serializer, version2);
        AssertJsonRoundTrip(serializer, version3);
    }

    #endregion

    #region Edge Cases and Error Handling Tests

    [Fact]
    public void TestJsonSerializers_NullHandling()
    {
        // Test that serializers handle null appropriately
        var stringSerializer = new StringSerializer();
        AssertJsonEncode(stringSerializer, null, "null");

        var byteArraySerializer = new ByteArraySerializer();
        AssertJsonEncode(byteArraySerializer, null, "[]");
    }

    [Fact]
    public void TestJsonSerializers_EmptyValues()
    {
        // Test empty string
        var stringSerializer = new StringSerializer();
        AssertJsonRoundTrip(stringSerializer, "");

        // Test empty byte array (encode only due to ByteArraySerializer bug)
        var byteArraySerializer = new ByteArraySerializer();
        AssertJsonEncode(byteArraySerializer, new byte[0], "[]");

        // Test empty symbol
        var symbolSerializer = new SymbolSerializer();
        var emptySymbol = new OscSymbol { Value = "" };
        AssertJsonRoundTrip(symbolSerializer, emptySymbol);
    }

    [Fact]
    public void TestJsonSerializers_BoundaryValues()
    {
        // Test numeric boundary values
        var intSerializer = new IntSerializer();
        AssertJsonRoundTrip(intSerializer, int.MaxValue);
        AssertJsonRoundTrip(intSerializer, int.MinValue);

        var longSerializer = new LongSerializer();
        AssertJsonRoundTrip(longSerializer, long.MaxValue);
        AssertJsonRoundTrip(longSerializer, long.MinValue);

        // Skip extreme float/double values due to ReadAsDecimal() limitations
        var floatSerializer = new FloatSerializer();
        AssertJsonRoundTrip(floatSerializer, 1000000.5f);
        AssertJsonRoundTrip(floatSerializer, -1000000.5f);

        var doubleSerializer = new DoubleSerializer();
        AssertJsonRoundTrip(doubleSerializer, 1000000000.123456789);
        AssertJsonRoundTrip(doubleSerializer, -1000000000.123456789);
    }

    [Fact]
    public void TestJsonSerializers_SpecialCharacters()
    {
        var stringSerializer = new StringSerializer();

        // Test JSON special characters
        AssertJsonRoundTrip(stringSerializer, "String with \"quotes\"");
        AssertJsonRoundTrip(stringSerializer, "String with \\ backslash");
        AssertJsonRoundTrip(stringSerializer, "String with \n newline");
        AssertJsonRoundTrip(stringSerializer, "String with \t tab");
        AssertJsonRoundTrip(stringSerializer, "String with / forward slash");
    }

    #endregion
}