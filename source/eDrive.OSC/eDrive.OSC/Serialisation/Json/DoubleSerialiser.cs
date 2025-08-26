using Newtonsoft.Json;

namespace eDrive.OSC.Serialisation.Json;

/// <summary>
///     Serializer for <see cref="double" />
/// </summary>
[CustomOscJSonSerializer('d', typeof(double))]
public class DoubleSerializer : OscTypeJsonSerializer<double>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DoubleSerializer" /> class.
    /// </summary>
    public DoubleSerializer() : base('d')
    {
    }


    public override double Decode(JsonReader reader)
    {
        try
        {
            var stringValue = reader.ReadAsString();
            
            // Handle special double values that JSON.NET produces
            if (stringValue == "Infinity")
                return double.PositiveInfinity;
            if (stringValue == "-Infinity")
                return double.NegativeInfinity;
            if (stringValue == "NaN")
                return double.NaN;
                
            return double.Parse(stringValue ?? "0");
        }
        catch
        {
            return double.NaN;
        }
    }

    public override void Encode(JsonWriter output, double value)
    {
        output.WriteValue(value);
    }
}