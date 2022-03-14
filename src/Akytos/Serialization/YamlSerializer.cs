using System.Collections;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace Akytos;

public class YamlSerializer
{
    private readonly HashSet<ISerializationSurrogate> m_serializationSurrogates;

    public YamlSerializer()
    {
        m_serializationSurrogates = new HashSet<ISerializationSurrogate>();
    }
    
    public YamlSerializer(HashSet<ISerializationSurrogate> serializationSurrogates)
    {
        m_serializationSurrogates = serializationSurrogates;
    }

    public string Serialize(object value)
    {
        var writer = new StringWriter();
        var emitter = new Emitter(writer);
        
        emitter.Emit(new StreamStart());
        emitter.Emit(new DocumentStart());

        SerializeObjectNew(emitter, value);
        
        emitter.Emit(new DocumentEnd(true));
        emitter.Emit(new StreamEnd());

        string result = writer.ToString();

        return result;
    }

    public void AddSurrogate<T>(ISerializationSurrogate<T> surrogate)
    {
        m_serializationSurrogates.Add(surrogate);
    }

    private void SerializeObject(IEmitter emitter, object obj)
    {
        var type = obj.GetType();
        var serializedFields = NodeUtils.GetSerializedFields(type).ToList();

        if (!serializedFields.Any())
        {
            return;
        }
        
        emitter.Emit(new MappingStart());
        emitter.Emit(new Scalar("type"));
        emitter.Emit(new Scalar(type.FullName));
        emitter.Emit(new Scalar("value"));

        SerializeFields(emitter, obj, serializedFields);
        
        emitter.Emit(new MappingEnd());
    }

    private void SerializeFields(IEmitter emitter, object obj, IEnumerable<FieldInfo> fields)
    {
        emitter.Emit(new MappingStart());
        
        foreach (var serializedField in fields)
        {
            var fieldType = serializedField.FieldType;
            object? fieldValue = serializedField.GetValue(obj);

            emitter.Emit(new Scalar(serializedField.Name));

            if (fieldValue == null)
            {
                emitter.Emit(new Scalar(string.Empty));
            }
            
            // Case A: Primitive
            else if (fieldType.IsPrimitive || typeof(string).IsAssignableFrom(fieldType) || fieldType.IsValueType)
            {
                emitter.Emit(new Scalar(fieldValue?.ToString() ?? string.Empty));
            }
            
            // Case B: Enumerable
            else if (typeof(IEnumerable).IsAssignableFrom(fieldType))
            {
                var enumerable = fieldValue as IEnumerable;
                emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Block));
                foreach (object element in enumerable!)
                {
                    var elementType = element.GetType();
                    if (elementType.IsPrimitive || typeof(string).IsAssignableFrom(elementType) || elementType.IsValueType)
                    {
                        emitter.Emit(new Scalar(element.ToString() ?? string.Empty));
                        continue;
                    }

                    var serializedFields = NodeUtils.GetSerializedFields(elementType);
                    SerializeFields(emitter, element, serializedFields);
                }
                
                emitter.Emit(new SequenceEnd());
            }
            else
            {
                // Case C: Serialized Object
                SerializeObject(emitter, fieldValue);
            }
        }
        
        emitter.Emit(new MappingEnd());
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
            SerializeObjectNew(emitter, fieldObject);
        }
        
        emitter.Emit(new MappingEnd());
    }

    private void SerializeObjectNew(IEmitter emitter, object? obj)
    {
        if (obj == null)
        {
            emitter.Emit(new Scalar(string.Empty));
            return;
        }

        var objectType = obj.GetType();

        var surrogate = ResolveSurrogate(objectType);
        if (surrogate != null)
        {
            surrogate.Serialize(emitter, obj);
            return;
        }
        
        emitter.Emit(new MappingStart());
        
        emitter.Emit(new Scalar("type"));
        emitter.Emit(new Scalar(objectType.FullName));
        
        emitter.Emit(new Scalar("value"));

        SerializeObjectData(emitter, obj);

        emitter.Emit(new MappingEnd());
    }

    private void SerializeObjectData(IEmitter emitter, object obj)
    {
        var objectType = obj.GetType();
        
        if (objectType.IsPrimitive || typeof(string).IsAssignableFrom(objectType) || objectType.IsValueType)
        {
            emitter.Emit(new Scalar(obj.ToString() ?? string.Empty));
        }
        else if (typeof(IEnumerable).IsAssignableFrom(objectType))
        {
            var enumerable = obj as IEnumerable;
            emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Block));
            foreach (object element in enumerable!)
            {
                SerializeObjectData(emitter, element);
            }
            
            emitter.Emit(new SequenceEnd());
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