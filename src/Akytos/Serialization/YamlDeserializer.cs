using YamlDotNet.Serialization;

namespace Akytos;

public class YamlDeserializer
{
    public T Deserialize<T>(string value)
    {
        return (T)Deserialize(value);
    }
    
    public object Deserialize(string value)
    {
        var deserializer = new Deserializer();

        var serializedObject = deserializer.Deserialize<SerializedObject>(value);

        object deserializedObject = SerializedObject.Deserialize(serializedObject);

        return deserializedObject;
    }
}