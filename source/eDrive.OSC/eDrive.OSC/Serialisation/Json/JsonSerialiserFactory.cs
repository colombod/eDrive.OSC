using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using eDrive.Osc.Serialisation;

namespace eDrive.Osc.Serialisation.Json
{
    /// <summary>
    ///     Osc Serialiser Factory;
    /// </summary>
    public static class JsonSerialiserFactory
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

        private static readonly IOscTypeJsonSerialiser[] s_tag2Serialiser;
        private static readonly Dictionary<Type, IOscTypeJsonSerialiser> s_type2Serialiser;
        private static string s_supported;

        /// <summary>
        ///     Initializes the <see cref="SerialiserFactory" /> class.
        /// </summary>
        static JsonSerialiserFactory()
        {
            s_type2Serialiser = new Dictionary<Type, IOscTypeJsonSerialiser>();
            s_tag2Serialiser = new IOscTypeJsonSerialiser[256];
           
            NilSerialiser = new NilSerialiser();
            StringSerialiser = new StringSerialiser();
            TimeTagSerialiser = new OscTimeTagSerialiser();
            IntSerialiser = new IntSerialiser();
            ByteArraySerialiser = new ByteArraySerialiser();

			var src = typeof(JsonSerialiserFactory).GetTypeInfo ().Assembly;
			LoadSerialisersFromAssembly (src);
        }

		public static void LoadSerialiser(TypeInfo source){
			if (source == null) {
				throw new ArgumentNullException ("source");
			}
			s_supported = null;
			Scan(source);
		}


		public static void LoadSerialiser(Type source){
			if (source == null) {
				throw new ArgumentNullException ("source");
			}

			LoadSerialiser (source.GetTypeInfo ());
		}


		public static void LoadSerialisers(IEnumerable<TypeInfo> sources){
			if (sources == null) {
				throw new ArgumentNullException ("sources");
			}
			s_supported = null;
			foreach (var source in sources) {
				Scan(source);
			}
		}

		public static void LoadSerialisers(IEnumerable<Type> sources){
			if (sources == null) {
				throw new ArgumentNullException ("sources");
			}
			s_supported = null;
			foreach (var source in sources.Select(s => s.GetTypeInfo())) {
				Scan(source);
			}
		}

		public static void LoadSerialisers(params TypeInfo[] sources){
			if (sources == null) {
				throw new ArgumentNullException ("sources");
			}
			LoadSerialisers ((IEnumerable<TypeInfo>)sources);
		}

		public static void LoadSerialisersFromAssembly(Assembly source){
			if (source == null) {
				throw new ArgumentNullException ("source");
			}

			s_supported = null;
			Scan(source);
		}

		public static void LoadSerialisersFromAssemblies(IEnumerable<Assembly> sources)
		{
			if (sources == null) {
				throw new ArgumentNullException ("sources");
			}

			s_supported = null;
			foreach (var source in sources) {
				Scan(source);
			}
		}

		public static void LoadSerialisersFromAssemblies(params Assembly[] sources)
		{

			if (sources == null) {
				throw new ArgumentNullException ("sources");
			}
			LoadSerialisersFromAssemblies((IEnumerable<Assembly>)sources);
		}

        /// <summary>
        ///     Gets the byte array serialiser.
        /// </summary>
        /// <value>
        ///     The byte array serialiser.
        /// </value>
        public static ByteArraySerialiser ByteArraySerialiser { get; private set; }

        /// <summary>
        ///     Gets the int serialiser.
        /// </summary>
        /// <value>
        ///     The int serialiser.
        /// </value>
        public static IntSerialiser IntSerialiser { get; private set; }

        /// <summary>
        ///     Gets the time tag serialiser.
        /// </summary>
        /// <value>
        ///     The time tag serialiser.
        /// </value>
        public static OscTimeTagSerialiser TimeTagSerialiser { get; private set; }

        /// <summary>
        ///     Gets the string serialiser.
        /// </summary>
        /// <value>
        ///     The string serialiser.
        /// </value>
        public static StringSerialiser StringSerialiser { get; private set; }

        /// <summary>
        ///     Gets the nil serialiser.
        /// </summary>
        /// <value>
        ///     The nil serialiser.
        /// </value>
        public static NilSerialiser NilSerialiser { get; private set; }

        /// <summary>
        ///     Gets the tag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetTag<T>(T value)
        {
            var t = typeof (T);
			var tInfo = t.GetTypeInfo ();
            var typeTag = string.Empty;

            if (value is Array
                && !(value is byte[])) // NB: blobs are handled with a specific serialiser.
            {
                typeTag += ArrayOpen;
                foreach (var component in (value as Array))
                {
                    if (component is Array)
                    {
                        throw new OscSerialiserException("Nested arrays are not supported.");
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
                    typeTag += NilSerialiser.Tag;
                }

                else if (t == value.GetType())
                {
                    if (tInfo.IsValueType
                        || !Equals(value, default(T)))
                    {
                        var sed = GetSerialiser<T>();
                        typeTag += sed.GetTag(value);
                    }
                    else
                    {
                        typeTag += NilSerialiser.Tag;
                    }
                }
                else
                {
                    var sed = GetSerialiser((object) value);
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
            return s_type2Serialiser.ContainsKey(t);
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
                    s_tag2Serialiser.Where(s => s != null)
                                    .Select((s, i) => (char) i)
                                    .Concat(new[] {NilTag, EventTag, ArrayOpen, ArrayClose})
                                    .Distinct()
                                    .ToArray();

                s_supported = new string(chars);
            }

            return s_supported;
        }


        /// <summary>
        ///     Gets the serialiser.
        /// </summary>
        /// <param name="typeTag">The type tag.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerialiser GetSerialiser(char typeTag)
        {
            return s_tag2Serialiser[typeTag];
        }

        /// <summary>
        ///     Gets the ses serialiser.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerialiser GetSerialiser(Type type)
        {
            return s_type2Serialiser[type];
        }

        /// <summary>
        ///     Gets the serialiser.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerialiser GetSerialiser(object source)
        {
            return (source == null ? NilSerialiser : GetSerialiser(source.GetType()));
        }

        /// <summary>
        ///     Gets the serialiser.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerialiser<T> GetSerialiser<T>(T source)
        {
            return GetSerialiser<T>();
        }

        /// <summary>
        ///     Gets the serialiser.
        /// </summary>
        /// <param name="typeTag">The type tag.</param>
        /// <returns></returns>
        public static IOscTypeJsonSerialiser<T> GetSerialiser<T>(char typeTag)
        {
            return GetSerialiser(typeTag) as IOscTypeJsonSerialiser<T>;
        }

        /// <summary>
        ///     Gets the ses serialiser.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IOscTypeJsonSerialiser<T> GetSerialiser<T>()
		{
			var type = typeof(T);
			return GetSerialiser (type) as IOscTypeJsonSerialiser<T>;
		}

		private static void Scan(TypeInfo tInfo)
		{
			var attr = tInfo.GetCustomAttribute<CustomOscJSonSerialiserAttribute> ();
			if (attr != null) 
			{
				try
				{
					var instance = Activator.CreateInstance(tInfo.AsType()) as IOscTypeJsonSerialiser;
					if (instance != null)
					{
						if (attr.TypeTag != ' ')
						{
							s_tag2Serialiser[attr.TypeTag] = instance;
						}

						if (attr.Type != null)
						{
							s_type2Serialiser[attr.Type] = instance;
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
			var marked = loadedAssembly.GetCustomAttribute<ContainsOscJsonSerialisersAttribute>();
			if (marked != null)
			{
				var types = loadedAssembly.ExportedTypes.Select(et => et.GetTypeInfo());

				foreach (var source in types)
				{
					Scan (source);
				}
			}
        }
    }
}