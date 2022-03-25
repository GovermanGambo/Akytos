namespace Akytos.Configuration;

public interface IConfiguration
{
    int? ReadInt(string key);
    float? ReadFloat(string key);
    string? ReadString(string key);

    void WriteString(string key, string value);

    public void Save();
}