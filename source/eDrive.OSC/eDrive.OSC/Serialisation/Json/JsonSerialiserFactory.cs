using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace eDrive.OSC.Serialisation.Json
{
    /// <summary>
    ///     Osc Serializer Factory;
    /// </summary>
    public static class JsonSerializerFactory
    {
        /// <summary>
        ///     The beginning character in an Osc message type tag.
        /// </summary>
        public const char DefaultTag = ',';

        /// <summary>
        ///     The type tag for Nil. No bytes are allocated in the argument data.
        /// </summary>
        public const char NilTag = 'N';

        /// <summary>
        ///     The type tag for inifinitum. No bytes are allocated in the argument data.
        /// </summary>
        public const char InfinitumTag = 'I';

        /// <summary>
        ///     The type tag for event. No bytes are allocated in the argument data.
        /// </summary>
        public const char EventTag = 'I';

        /// <summary>
        ///     The type tag for event. No bytes are allocated in the argument data.
        /// </summary>
        public const string EventTagString = "I";

        /// <summary>
        ///     The array open
        /// </summary>
        public const char ArrayOpen = '[';

        /// <summary>
        ///     The array close
        /// </summary>
        public const char ArrayClose = ']';

        private static readonly IOscTypeJsonSerializer[] s_tag2Serializer;
        private static readonly Dictionary<Type, IOscTypeJsonSerializer> s_type2Serializer;
        private static string s_supported;

        /// <summary>
        ///     Initializes the <see cref="SerializerFactory" /> class.
        /// </summary>
        static JsonSerializerFactory()
        {
            s_type2Serializer = new Dictionary<Type, IOscTypeJsonSerializer>();
            s_tag2Serializer = new IOscTypeJsonSerializer[256];

            NilSerializer = new NilSerializer();
            StringSerializer = new StringSerializer();
            TimeTagSerializer = new OscTimeTagSerializer();
            IntSerializer = new IntSerializer();
            ByteArraySerializer = new ByteArraySerializer();

            var src = typeof(JsonSerializerFactory).GetTypeInfo().Assembly;
            LoadSerializersFromAssembly(src);
        }

        public static void LoadSerializer(TypeInfo source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            s_supported = null;
            Scan(source);
        }


        public static void LoadSerializer(Type source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            LoadSerializer(source.GetTypeInfo());
        }


        public static void LoadSerializers(IEnumerable<TypeInfo> sources)
        {
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            s_supported = null;
            foreach (var source in sources)
            {
                Scan(source);
            }
        }

        public static void LoadSerializers(IEnumerable<Type> sources)
        {
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            s_supported = null;
            foreach (var source in sources.Select(s => s.GetTypeInfo()))
            {
                Scan(source);
            }
        }

        public static void LoadSerializers(params TypeInfo[] sources)
        {
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            LoadSerializers((IEnumerable<TypeInfo>)sources);
        }

        public static void LoadSerializersFromAssembly(Assembly source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            s_supported = null;
            Scan(source);
        }

        public static void LoadSerializersFromAssemblies(IEnumerable<Assembly> sources)
        {
            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }

            s_supported = null;
            foreach (var source in sources)
            {
                Scan(source);
            }
        }

        public static void LoadSerializersFromAssemblies(params Assembly[] sources)
        {

            if (sources == null)
            {
                throw new ArgumentNullException("sources");
            }
            LoadSerializersFromAssemblies((IEnumerable<Assembly>)sources);
        }

        /// <summary>
        ///     Gets the byte array serializer.
        /// </summary>
        /// <value>
        ///     The byte array serializer.
        /// </value>
        public static ByteArraySerializer ByteArraySerializer { get; private set; }

        /// <summary>
        ///     Gets the int serializer.
        /// </summary>
        /// <value>
        ///     The int serializer.
        /// </value>
        public static IntSerializer IntSerializer { get; private set; }

        /// <summary>
        ///     Gets the time tag serializer.
        /// </summary>
        /// <value>
        ///     The time tag serializer.
        /// </value>
        public static OscTimeTagSerializer TimeTagSerializer { get; private set; }

        /// <summary>
        ///     Gets the string serializer.
        /// </summary>
        /// <value>
        ///     The string serializer.
        /// </value>
        public static StringSerializer StringSerializer { get; private set; }

        /// <summary>
        ///     Gets the nil serializer.
        /// </summary>
        /// <value>
        ///     The nil serializer.
        /// </value>
        public static NilSerializer NilSerializer { get; private set; }

        /// <summary>
        ///     Gets the tag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetTag<T>(T value)
        {
            var t = typeof(T);
            var tInfo = t.GetTypeInfo();
            var typeTag = string.Empty;

            if (value is Array
                && !(value is byte[])) // NB: blobs are handled with a specific serializer.
            {
                typeTag += ArrayOpen;
                foreach (var component in (value as Array))
                {
                    if (component is Array)
                    {
                        throw new OscSerializerException("Nested arrays are not supported.");
                    }

                    typeTag += GetTag(component);
                }
                typeTag += ArrayClose;
            }
            else
            {
                if (!tInfo.IsValueType
                    && Equals(value, default(T)))
                {
                    typeTag += NilSerializer.Tag;
                }

                else if (t == value.GetType())
                {
                    if (tInfo.IsValueType
                        || !Equals(value, default(T)))
                    {
                        var sed = GetSerializer<T>();
                        typeTag += sed.GetTag(value);
                    }
                    else
                    {
                        typeTag += NilSerializer.Tag;
                    }
                }
                else
                {
                    var sed = GetSerializer((object)value);
                    typeTag += sed.GetTag(value);
                }
            }

            return typeTag;
        }

        /// <summary>
        ///     Determines whether this instance can serialise the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        ///     <c>true</c> if this instance can serialise the specified t; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanSerialise(Type t)
        {
            return s_type2Serializer.ContainsKey(t);
        }

        /// <summary>
        ///     Supporteds the types.
        /// </summary>
        /// <returns></returns>
        public static string SupportedTypes()
        {
            if (s_supported == null)
            {
                var chars =
                    s_tag2Serializer.Where(s => s != null)
                                    .Select((s, i) => (char)i)
                                    .Concat(new[] { NilTag, EventTag, ArrayOpen, ArrayClose })
                                    .Distinct()
                                    .ToArray();

                s_supported = new string(chars);
            }

            return s_supported;
        }


        /// <summary>
        ///     Gets the serializer.
        /// </summary>
        /// <param name="typeTag">The type tag.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerializer GetSerializer(char typeTag)
        {
            return s_tag2Serializer[typeTag];
        }

        /// <summary>
        ///     Gets the ses serializer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerializer GetSerializer(Type type)
        {
            return s_type2Serializer[type];
        }

        /// <summary>
        ///     Gets the serializer.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerializer GetSerializer(object source)
        {
            return (source == null ? NilSerializer : GetSerializer(source.GetType()));
        }

        /// <summary>
        ///     Gets the serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerializer<T> GetSerializer<T>(T source)
        {
            return GetSerializer<T>();
        }

        /// <summary>
        ///     Gets the serializer.
        /// </summary>
        /// <param name="typeTag">The type tag.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerializer<T> GetSerializer<T>(char typeTag)
        {
            return GetSerializer(typeTag) as IOscTypeJsonSerializer<T>;
        }

        /// <summary>
        ///     Gets the ses serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IOscTypeJsonSerializer<T> GetSerializer<T>()
        {
            var type = typeof(T);
            return GetSerializer(type) as IOscTypeJsonSerializer<T>;
        }

        private static void Scan(TypeInfo tInfo)
        {
            var attr = tInfo.GetCustomAttribute<CustomOscJSonSerializerAttribute>();
            if (attr != null)
            {
                try
                {
                    var instance = Activator.CreateInstance(tInfo.AsType()) as IOscTypeJsonSerializer;
                    if (instance != null)
                    {
                        if (attr.TypeTag != ' ')
                        {
                            s_tag2Serializer[attr.TypeTag] = instance;
                        }

                        if (attr.Type != null)
                        {
                            s_type2Serializer[attr.Type] = instance;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private static void Scan(Assembly loadedAssembly)
        {
            var marked = loadedAssembly.GetCustomAttribute<ContainsOscJsonSerializersAttribute>();
            if (marked != null)
            {
                var types = loadedAssembly.ExportedTypes.Select(et => et.GetTypeInfo());

                foreach (var source in types)
                {
                    Scan(source);
                }
            }
        }
    }
}