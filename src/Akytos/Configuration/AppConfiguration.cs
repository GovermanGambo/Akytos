using System.Reflection;

namespace Akytos.Configuration;

public class AppConfiguration
{
    private readonly Dictionary<string, string> m_configuration;

    private readonly string m_filePath;

    public AppConfiguration(string path)
    {
        m_filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, path);
        m_configuration = LoadConfiguration()!;
    }

    public string? ReadString(string key)
    {
        m_configuration.TryGetValue(key, out string? value);

        return value;
    }

    public void WriteString(string key, string value)
    {
        if (m_configuration.ContainsKey(key))
        {
            m_configuration[key] = value;
        }
        else
        {
            m_configuration.Add(key, value);
        }
    }

    public void Save()
    {
        string fileContent = string.Join(PlatformConstants.NewLine, m_configuration.Select(c => c.Key + " = " + c.Value));
        
        File.WriteAllText(m_filePath, fileContent);
    }

    private Dictionary<string, string> LoadConfiguration()
    {
        string fileContent = GetFileContent();

        string[] lines = fileContent.Split(PlatformConstants.NewLine);

        if (lines.Length == 1 && lines[0] == "")
        {
            return new Dictionary<string, string>();
        }

        var dictionary = lines.ToDictionary(s => s.Split(" ")[0], s => s.Split(" ")[2]);

        return dictionary;
    }

    private string GetFileContent()
    {
        if (!File.Exists(m_filePath))
        {
            File.Create(m_filePath);
        }
        
        return File.ReadAllText(m_filePath);
    }
}