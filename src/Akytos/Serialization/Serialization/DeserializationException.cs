namespace Akytos.Serialization;

public class DeserializationException : Exception
{
    public DeserializationException(string? message = null)
        : base(message)
    {
    }
}