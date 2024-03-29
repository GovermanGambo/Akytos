using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Akytos.Configuration;

namespace Windmill;

internal class EditorConfiguration : IConfiguration
{
    private const string ConfigurationFileName = "Akytos.ini";

    private readonly IConfiguration m_configurationFile;

    public EditorConfiguration()
    {
        string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, ConfigurationFileName);
        m_configurationFile = new ConfigurationFile(filePath);
    }

    public Dictionary<string, string> GetSection(string key)
    {
        return m_configurationFile.GetSection(key);
    }

    public int? ReadInt(string key)
    {
        return m_configurationFile.ReadInt(key);
    }

    public float? ReadFloat(string key)
    {
        return m_configurationFile.ReadFloat(key);
    }

    public string? ReadString(string key)
    {
        return m_configurationFile.ReadString(key);
    }

    public bool Remove(string key)
    {
        return m_configurationFile.Remove(key);
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