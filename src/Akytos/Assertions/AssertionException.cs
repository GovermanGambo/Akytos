namespace Akytos.Assertions;

public class AssertionException : Exception
{
    public AssertionException(string? message = null)
        : base(message)
    {
    }
}