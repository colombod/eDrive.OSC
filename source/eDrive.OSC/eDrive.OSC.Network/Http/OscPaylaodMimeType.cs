namespace eDrive.OSC.Network.Http
{
    public class OscPaylaodMimeType
    {
        static OscPaylaodMimeType()
        {
            Json = new OscPaylaodMimeType("application/json; charset=utf-8");
            Osc = new OscPaylaodMimeType("application/octet-stream");
        }

        private OscPaylaodMimeType(string type)
        {
            Type = type;
        }

        public static OscPaylaodMimeType Json { get; private set; }
        public static OscPaylaodMimeType Osc { get; private set; }

        public string Type { get; private set; }
    }
}