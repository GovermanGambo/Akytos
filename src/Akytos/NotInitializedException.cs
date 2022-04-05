namespace Akytos;

internal class NotInitializedException : Exception
{
    public NotInitializedException(string serviceName)
        : base($"{serviceName} is not initialized.")
    {
    }
}