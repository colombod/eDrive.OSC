using eDrive.OSC.Serialisation.Json;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace eDrive.OSC.Network.Http;

public static class JsonExtensions
{
    public static OscPacket CreateFromJson(this string json)
    {
        OscPacket ret = null;
        if (!string.IsNullOrWhiteSpace(json))
        {
            var reader = new JsonTextReader(new StringReader(json));
            ret = CreateFromJson(reader);
        }
        return ret;
    }

    private static OscPacket CreateFromJson(JsonReader reader)
    {
        OscPacket ret = null;

        {
            while (reader.TokenType != JsonToken.StartObject)
            {
                var done = reader.Read(); // read obejct start
                if (!done)
                {
                    break;
                }
            }

            if (reader.TokenType != JsonToken.None)
            {
                reader.Read(); // read property name for address

                var address = reader.ReadAsString(); // read address;

                if (address.StartsWith("#"))
                {
                    ret = DeserialiseBundle(reader);
                }
                else
                {
                    ret = DeserialiseMessage(reader, address);
                }

                reader.Read(); // read end object
            }
        }
        return ret;
    }

    private static OscPacket DeserialiseMessage(JsonReader reader, string address)
    {
        reader.Read(); // read property tags
        var tags = reader.ReadAsString();
        var msg = new OscMessage(address);
        reader.Read(); // data property name
        reader.Read(); // array start
        for (var index = 0; index < tags.Length; index++)
        {
            var tag = tags[index];

            if (tag == JsonSerializerFactory.ArrayOpen)
            {
                // skip the '[' character.
                index++;

                // deserialise array of object
                var ret = new List<object>();
                while (tags[index] != JsonSerializerFactory.ArrayClose
                       && index < tags.Length)
                {
                    var des = JsonSerializerFactory.GetSerializer(tags[index]);
                    ret.Add(des.Decode(reader));

                    index++;
                }

                msg.Append(ret.ToArray());
            }
            else if (tag != JsonSerializerFactory.DefaultTag)
            {
                if (tag != JsonSerializerFactory.EventTag)
                {
                    var des = JsonSerializerFactory.GetSerializer(tag);
                    msg.Append(des.Decode(reader));
                }
                else
                {
                    msg.IsEvent = true;
                }
            }
        }
        reader.Read(); // array end

        return msg;
    }

    private static OscPacket DeserialiseBundle(JsonReader reader)
    {
        reader.Read(); // property timetag
        reader.Read();
        var tag = (ulong)Convert.ChangeType(reader.Value, typeof(ulong));

        var ret = new OscBundle(new OscTimeTag(tag));

        while (reader.TokenType != JsonToken.EndObject) // empty bundle
        {
            if (reader.TokenType == JsonToken.PropertyName)
            {
                if (reader.ValueType == typeof(string) && StringComparer.OrdinalIgnoreCase.Compare(reader.Value, "messages") == 0) // mesages found
                {
                    // messages
                    while (reader.TokenType != JsonToken.StartArray)
                    {
                        reader.Read(); // start array
                    }

                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            var msg = CreateFromJson(reader) as OscMessage;
                            if (msg != null)
                            {
                                ret.Append(msg);
                            }
                        }
                        else
                        {
                            reader.Read();
                        }
                    }
                }
                else if (reader.ValueType == typeof(string) && StringComparer.OrdinalIgnoreCase.Compare(reader.Value, "bundles") == 0) // nested bundles
                {
                    // messages
                    while (reader.TokenType != JsonToken.StartArray)
                    {
                        reader.Read(); // start array
                    }

                    while (reader.TokenType != JsonToken.EndArray)
                    {
                        if (reader.TokenType == JsonToken.StartObject)
                        {
                            var bundle = CreateFromJson(reader) as OscBundle;
                            if (bundle != null)
                            {
                                ret.Append(bundle);
                            }
                        }
                        else
                        {
                            reader.Read();
                        }
                    }
                }

            }
            else
            {
                reader.Read();
            }

        }

        return ret;
    }

    public static string CreateJson(this OscPacket value)
    {
        var ret = string.Empty;
        if (value != null)
        {
            var json = new StringBuilder();
            var sw = new StringWriter(json);
            using (var writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();

                var msg = value as OscMessage;
                var bundle = value as OscBundle;

                if (msg != null)
                {
                    Serialise(msg, writer);
                }
                else if (bundle != null)
                {
                    Serialise(bundle, writer);
                }

                writer.WriteEndObject();
            }
            ret = json.ToString();
        }
        return ret;
    }

    private static void Serialise(OscMessage msg, JsonWriter writer)
    {
        writer.WritePropertyName("address");
        writer.WriteValue(msg.Address);
        writer.WritePropertyName("typeTag");
        writer.WriteValue(msg.TypeTag);
        writer.WritePropertyName("data");
        writer.WriteStartArray();
        for (var i = 0; i < msg.Data.Count; i++)
        {
            var part = msg.Data[i];

            if (part is Array
                && !(part is byte[])) // NB: blobs are handled with a specific serialisator.)
            {
                var collection = part as Array;
                foreach (var component in collection)
                {
                    var ser = JsonSerializerFactory.GetSerializer(component);
                    ser.Encode(writer, component);
                }
            }
            else
            {
                var ser = JsonSerializerFactory.GetSerializer(part);
                ser.Encode(writer, part);
            }
        }
        writer.WriteEndArray();
    }

    private static void Serialise(OscBundle bundle, JsonTextWriter writer)
    {
        if (writer == null)
        {
            throw new ArgumentNullException("writer");
        }
        writer.WritePropertyName("address");
        writer.WriteValue(bundle.Address);
        writer.WritePropertyName("timeTag");
        writer.WriteValue(bundle.TimeStamp.TimeTag);
        var messages = bundle.Messages;
        if (messages != null
            && messages.Count > 0)
        {
            writer.WritePropertyName("messages");
            writer.WriteStartArray();
            for (var i = 0; i < messages.Count; i++)
            {
                writer.WriteStartObject();
                Serialise(messages[i], writer);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }

        var bundles = bundle.Bundles;
        if (bundles != null
            && bundles.Count > 0)
        {
            writer.WritePropertyName("bundles");
            writer.WriteStartArray();
            for (var i = 0; i < bundles.Count; i++)
            {
                writer.WriteStartObject();
                Serialise(bundles[i], writer);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}