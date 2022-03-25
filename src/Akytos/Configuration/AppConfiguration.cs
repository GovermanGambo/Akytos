using System.Reflection;

namespace Akytos.Configuration;

public class AppConfiguration : IConfiguration
{
    private const string ConfigurationFileName = "Akytos.ini";

    private readonly ConfigurationFile m_configurationFile;

    public AppConfiguration()
    {
        string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, ConfigurationFileName);
        m_configurationFile = new ConfigurationFile(filePath);
    }

    public string? ReadString(string key)
    {
        return m_configurationFile.ReadString(key);
    }

    public void WriteString(string key, string value)
    {
        m_configurationFile.WriteString(key, value);
    }

    public void Save()
    {
        m_configurationFile.Save();
    }
}