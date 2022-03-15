using System.Collections;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Akytos.Serialization;

/// <summary>
///     A class used for serializing objects to Yaml. Note that objects must either be marked with <see cref="SerializableAttribute"/>,
///     have fields with the <see cref="SerializeFieldAttribute"/> or be a primitive or list.
/// </summary>
public class YamlSerializer
{
    private readonly HashSet<ISerializationSurrogate> m_serializationSurrogates;

    /// <summary>
    ///     Creates a new <see cref="YamlSerializer"/>
    /// </summary>
    public YamlSerializer()
    {
        m_serializationSurrogates = new HashSet<ISerializationSurrogate>();
    }
    
    /// <summary>
    ///     Creates a new <see cref="YamlSerializer"/>
    /// </summary>
    /// <param name="serializationSurrogates">A collection of serialization surrogates to be used by the serializer.</param>
    public YamlSerializer(IEnumerable<ISerializationSurrogate> serializationSurrogates)
    {
        m_serializationSurrogates = new HashSet<ISerializationSurrogate>(serializationSurrogates);
    }

    /// <summary>
    ///     Serializes the supplied object to a Yaml string.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <returns>Object serialized as yaml string</returns>
    public string Serialize(object value)
    {
        var writer = new StringWriter();
        var emitter = new Emitter(writer);
        
        emitter.Emit(new StreamStart());
        emitter.Emit(new DocumentStart());

        SerializeObject(emitter, value);
        
        emitter.Emit(new DocumentEnd(true));
        emitter.Emit(new StreamEnd());

        string result = writer.ToString();

        return result;
    }

    /// <summary>
    ///     Adds a surrogate to the serializer. Only one of each surrogate type can be registered.
    /// </summary>
    /// <param name="surrogate">The surrogate instance to add.</param>
    /// <typeparam name="T">The object type that this surrogate applies to.</typeparam>
    public void AddSurrogate<T>(ISerializationSurrogate<T> surrogate)
    {
        m_serializationSurrogates.Add(surrogate);
    }
    
    private void SerializeFields(IEmitter emitter, object? obj)
    {
        if (obj == null)
        {
            emitter.Emit(new Scalar(string.Empty));
            return;
        }
        
        var fields = NodeUtils.GetSerializedFields(obj.GetType());
        
        emitter.Emit(new MappingStart());
        
        foreach (var serializedField in fields)
        {
            emitter.Emit(new Scalar(serializedField.Name));
            object? fieldObject = serializedField.GetValue(obj);
            SerializeObject(emitter, fieldObject);
        }
        
        emitter.Emit(new MappingEnd());
    }

    private void SerializeObject(IEmitter emitter, object? obj)
    {
        if (obj == null)
        {
            emitter.Emit(new Scalar(string.Empty));
            return;
        }

        var objectType = obj.GetType();
        
        emitter.Emit(new MappingStart());
        
        emitter.Emit(new Scalar("type"));
        emitter.Emit(new Scalar(objectType.FullName));
        
        emitter.Emit(new Scalar("value"));

        var surrogate = ResolveSurrogate(objectType);
        if (surrogate != null)
        {
            surrogate.Serialize(emitter, obj);
        }
        else
        {
            SerializeObjectData(emitter, obj);
        }

        emitter.Emit(new MappingEnd());
    }

    private void SerializeObjectData(IEmitter emitter, object obj)
    {
        var objectType = obj.GetType();
        
        if (objectType.IsPrimitive || typeof(string).IsAssignableFrom(objectType) || objectType.IsValueType)
        {
            emitter.Emit(new Scalar(obj.ToString() ?? string.Empty));
        }
        else if (typeof(ICollection).IsAssignableFrom(objectType))
        {
            var collection = obj as ICollection;
            emitter.Emit(new MappingStart());
            
            emitter.Emit(new Scalar("length"));
            emitter.Emit(new Scalar(collection.Count.ToString()));
            
            emitter.Emit(new Scalar("elements"));
            emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Block));
            foreach (object element in collection!)
            {
                SerializeObject(emitter, element);
            }
            
            emitter.Emit(new SequenceEnd());
            emitter.Emit(new MappingEnd());
        }
        else
        {
            SerializeFields(emitter, obj);
        }
    }

    private ISerializationSurrogate? ResolveSurrogate(Type type)
    {
        return m_serializationSurrogates.FirstOrDefault(s => s.GetType().GetInterfaces().First(i => i.IsGenericType).GetGenericArguments().FirstOrDefault() == type);
    }
}