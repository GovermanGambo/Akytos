namespace Akytos.Configuration;

public interface IConfiguration
{
    Dictionary<string, string>? GetSection(string key);
    int? ReadInt(string key);
    float? ReadFloat(string key);
    string? ReadString(string key);
    bool Remove(string key);
    void WriteString(string key, string value);

    void Save();
}