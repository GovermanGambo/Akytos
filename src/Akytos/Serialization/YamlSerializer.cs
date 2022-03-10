using YamlDotNet.Serialization;

namespace Akytos;

public class YamlSerializer
{
    public string Serialize(object value)
    {
        var serializer = new Serializer();
        var writer = new StringWriter();

        var serializedObject = SerializedObject.Create(value);

        serializer.Serialize(writer, serializedObject);

        return writer.ToString();
    }
}