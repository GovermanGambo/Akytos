namespace Akytos.Serialization;

public class DeserializationException : Exception
{
    public DeserializationException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}