namespace eDrive.OSC;

/// <summary>
///     Osc Symbol
/// </summary>]
public class OscSymbol
{
    /// <summary>
    ///     Gets or sets the value.
    /// </summary>
    /// <value>
    ///     The value.
    /// </value>
    public string Value { get; set; }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        return obj.GetType() == GetType() && Equals((OscSymbol)obj);
    }

    public override int GetHashCode()
    {
        return (Value != null ? Value.GetHashCode() : 0);
    }

    protected bool Equals(OscSymbol other)
    {
        return string.Equals(Value, other.Value);
    }
}