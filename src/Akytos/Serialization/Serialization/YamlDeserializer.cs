using System.Collections;
using System.Reflection;
using Akytos.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;
using Scalar = YamlDotNet.Core.Tokens.Scalar;
using StreamStart = YamlDotNet.Core.Tokens.StreamStart;

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

    public object? Deserialize(string yaml)
    {
        var reader = new StringReader(yaml);
        var scanner = new Scanner(reader);

        // Begin
        scanner.Begin();
        // Stream Start
        scanner.Read<StreamStart>();
        // BlockMappingStart
        
        object? obj = DeserializeObject(scanner);

        return obj;
    }

    private object? DeserializeObject(Scanner scanner)
    {
        scanner.Read<BlockMappingStart>();

        string objectTypeName = scanner.ReadScalar("type");
        bool isArrayType = false;
        if (objectTypeName.EndsWith("[]"))
        {
            objectTypeName = objectTypeName.Remove(objectTypeName.Length - 2, 2);
            isArrayType = true;
        }
            
        scanner.ReadScalarKey("value");
        scanner.Read<Value>();
        var objectType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.FullName == objectTypeName);

        if (objectType == null)
        {
            throw new SerializationException($"Type {objectTypeName} was not found!");
        }
        
        if (isArrayType)
        {
            objectType = objectType.MakeArrayType();
        }

        object? result;

        // Case A: Found a surrogate. Just use this
        
        // Case B: Value is primitive. If so, read value, and create instance while passing in the value directly.
        if (objectType.IsPrimitive || typeof(string).IsAssignableFrom(objectType) || objectType.IsValueType)
        {
            result = CreatePrimitiveObject(scanner, objectType);
        }

        // Case C: Value is enumerable. Get the type, then parse fields
        else if (typeof(IEnumerable).IsAssignableFrom(objectType))
        {
            result = CreateEnumerableObject(scanner, objectType);
        }

        // Case D: If none of the above, deserialize fields directly and set each of them.
        else
        {
            result = CreateSerializedObject(scanner, objectType);
        }

        scanner.Read<BlockEnd>();
        
        return result;
    }

    private object CreatePrimitiveObject(Scanner scanner, Type type)
    {
        string value = scanner.Read<Scalar>().Value;
        object obj = Convert.ChangeType(value, type);
        
        return obj;
    }

    private IEnumerable CreateEnumerableObject(Scanner scanner, Type listType)
    {
        var elementType = listType.GetInterfaces().First(i => i.IsGenericType).GetGenericArguments()
            .FirstOrDefault();


        object? enumerable = listType.IsArray ? new List<object>() : Activator.CreateInstance(listType);
        
        var list = enumerable as IList;

        while (scanner.Current is not BlockEnd)
        {
            if (scanner.TryRead<BlockEntry>(out _))
            {
                var scalar = scanner.Read<Scalar>();
                object value = Convert.ChangeType(scalar.Value, elementType);
                list.Add(value);
            }
            else
            {
                object? element = DeserializeObject(scanner);

                if (element != null)
                {
                    list.Add(element);
                }
            }
        }

        return list as IEnumerable;
    }
    
    private object? CreateSerializedObject(Scanner scanner, Type type)
    {
        object? obj = Activator.CreateInstance(type);

        if (obj == null)
        {
            Debug.LogError("Failed to deserialize object of type {0}.", type.FullName);
            return null;
        }

        // Read serialized object fields
        scanner.Read<BlockMappingStart>();
        
        while (scanner.Current is not BlockEnd)
        {
            scanner.Read<Key>();
            string fieldName = scanner.Read<Scalar>().Value;

            scanner.Read<Value>();
            object? value = DeserializeObject(scanner);
            var fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (fieldInfo == null)
            {
                throw new SerializationException($"No field with name {fieldName} was found on type {type.FullName}.");
            }

            fieldInfo.SetValue(obj, value);
        }

        return obj;
    }
    

    public void AddSurrogate<T>(ISerializationSurrogate<T> surrogate)
    {
        m_serializationSurrogates.Add(surrogate);
    }
}