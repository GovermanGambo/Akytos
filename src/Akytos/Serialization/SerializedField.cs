using YamlDotNet.Serialization;

namespace Akytos;

public class SerializedField
{
    public SerializedField()
    {
    }
    
    public SerializedField(string key, string type, object? value)
    {
        Key = key;
        Type = type;
        Value = value;
    }

    [YamlMember]
    public string Key { get; set; }
    [YamlMember]
    public string Type { get; set; }
    [YamlMember]
    public object? Value { get; set; }
}