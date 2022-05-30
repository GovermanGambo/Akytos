namespace Akytos.Diagnostics;

public class AssertionException : Exception
{
    public AssertionException(string? message = null)
        : base(message)
    {
    }
}