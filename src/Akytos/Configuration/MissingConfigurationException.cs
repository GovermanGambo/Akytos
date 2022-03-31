namespace Akytos.Configuration;

public class MissingConfigurationException : Exception
{
    public MissingConfigurationException(string configurationKey) : base(
        $"Configuration key {configurationKey} is missing.")
    {
        
    }
}