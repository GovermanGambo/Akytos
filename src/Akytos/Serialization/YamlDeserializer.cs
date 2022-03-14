namespace Akytos;

public class YamlDeserializer
{
    private readonly HashSet<ISerializationSurrogate> m_serializationSurrogates;

    public YamlDeserializer()
    {
        m_serializationSurrogates = new HashSet<ISerializationSurrogate>();
    }
    
    public YamlDeserializer(HashSet<ISerializationSurrogate> serializationSurrogates)
    {
        m_serializationSurrogates = serializationSurrogates;
    }

    public string Deserialize(object value)
    {
        throw new NotImplementedException();
    }

    public void AddSurrogate<T>(ISerializationSurrogate<T> surrogate)
    {
        m_serializationSurrogates.Add(surrogate);
    }
}