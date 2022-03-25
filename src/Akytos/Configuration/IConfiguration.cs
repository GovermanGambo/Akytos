namespace Akytos.Configuration;

public interface IConfiguration
{
    string? ReadString(string key);

    void WriteString(string key, string value);

    public void Save();
}