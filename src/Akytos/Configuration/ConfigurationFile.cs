using System.Reflection;

namespace Akytos.Configuration;

internal class ConfigurationFile : IConfiguration
{
    private readonly Dictionary<string, Dictionary<string, string>> m_configuration;

    private readonly string m_filePath;
    private readonly IniSerializer m_iniSerializer;
    
    public ConfigurationFile(string path)
    {
        m_filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, path);
        string ini = GetFileContent();
        m_iniSerializer = new IniSerializer();
        m_configuration = m_iniSerializer.Deserialize(ini);
    }

    public Dictionary<string, string> GetSection(string key)
    {
        if (!m_configuration.ContainsKey(key))
        {
            return new Dictionary<string, string>();
        }
        
        return m_configuration[key];
    }

    public int? ReadInt(string key)
    {
        string? value = ReadString(key);

        return value != null ? int.Parse(value) : null;
    }

    public float? ReadFloat(string key)
    {
        string? value = ReadString(key);

        return value != null ? float.Parse(value) : null;
    }

    public string? ReadString(string key)
    {
        (string categoryKey, key) = ParseKey(key);

        m_configuration.TryGetValue(categoryKey, out var category);

        if (category == null)
        {
            return null;
        }
        
        category.TryGetValue(key, out string? value);

        return value;
    }

    public bool Remove(string key)
    {
        (string categoryKey, key) = ParseKey(key);

        m_configuration.TryGetValue(categoryKey, out var category);

        if (category == null)
        {
            return false;
        }

        return category.Remove(key);
    }

    public void WriteString(string key, string value)
    {
        (string categoryKey, key) = ParseKey(key);

        if (!m_configuration.ContainsKey(categoryKey))
        {
            m_configuration[categoryKey] = new Dictionary<string, string>();
        }

        m_configuration[categoryKey][key] = value;
    }

    public void Save()
    {
        string fileContent = m_iniSerializer.Serialize(m_configuration);

        File.WriteAllText(m_filePath, fileContent);
    }

    private static (string, string) ParseKey(string key)
    {
        string[] segments = key.Split("/");

        if (segments.Length != 2)
        {
            throw new ArgumentException("Configuration key must have exactly two levels!");
        }

        return (segments[0], segments[1]);
    }

    private string GetFileContent()
    {
        if (!File.Exists(m_filePath))
        {
            var file = File.Create(m_filePath);
            file.Dispose();
        }
        
        return File.ReadAllText(m_filePath);
    }
}