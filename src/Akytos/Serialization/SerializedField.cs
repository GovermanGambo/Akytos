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

    public bool IsPrimitive
    {
        get
        {
            var type = System.Type.GetType(Type);

            if (type == null)
            {
                return false;
            }
            
            return type.IsPrimitive || type.IsValueType || type == typeof(string);
        }
    }
}