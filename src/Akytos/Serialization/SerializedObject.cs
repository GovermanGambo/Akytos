using System.Collections;
using Akytos.Assertions;
using YamlDotNet.Serialization;

namespace Akytos;

public class SerializedObject
{
    public SerializedObject()
    {
    }
    
    public SerializedObject(SerializedField[] fields, string type)
    {
        Fields = fields;
        Type = type;
    }

    [YamlMember] public string Type { get; set; } = null!;
    [YamlMember] public SerializedField[] Fields { get; set; } = null!;

    public static SerializedObject Create(object o)
    {
        var objectType = o.GetType();
        var fieldInfos = NodeUtils.GetSerializedFields(objectType);
        var serializedFields = new List<SerializedField>();
        foreach (var fieldInfo in fieldInfos)
        {
            string key = fieldInfo.Name;
            string type = fieldInfo.FieldType.FullName!;
            object? value = fieldInfo.GetValue(o);

            if (!IsPrimitiveType(fieldInfo.FieldType))
            {
                if (value is IEnumerable enumerable)
                {
                    var list = new List<object>();
                    foreach (object element in enumerable)
                    {
                        object elementValue;
                        if (!IsPrimitiveType(element.GetType()))
                        {
                            // Create new serialized object if not value type
                            elementValue = Create(element);
                        }
                        else
                        {
                            elementValue = element;
                        }
                    
                        list.Add(elementValue);
                    }

                    value = list;
                }
                else if (value != null)
                {
                    // Create new serialized object if not value type
                    value = Create(value);
                }
            }

            serializedFields.Add(new SerializedField(key, type, value));
        }

        return new SerializedObject(serializedFields.ToArray(), objectType.FullName!);
    }

    public static object Deserialize(SerializedObject serializedObject)
    {
        var objectType = System.Type.GetType(serializedObject.Type);
        
        Assert.IsNotNull(objectType, $"Type {serializedObject.Type} was not found!");
        
        object? instance = Activator.CreateInstance(objectType);
        
        Assert.IsNotNull(instance, $"Could not create instance of type {serializedObject.Type}!");

        var fields = NodeUtils.GetSerializedFields(objectType).ToList();

        foreach (var serializedField in serializedObject.Fields)
        {
            var fieldType = System.Type.GetType(serializedField.Type);
            Assert.IsNotNull(fieldType, $"Type {serializedField.Type} was not found!");

            var fieldInfo = fields.FirstOrDefault(f => f.Name == serializedField.Key);
            object? value = DeserializeField(serializedField);
            
            Assert.IsNotNull(fieldInfo, $"Field with name {serializedField.Key} was not found!");
            
            fieldInfo.SetValue(instance, value);
        }

        return instance;
    }

    private static object? DeserializeField(SerializedField serializedField)
    {
        var type = System.Type.GetType(serializedField.Type);
        
        Assert.IsNotNull(type, $"Type {serializedField.Type} was not found!");
        
        if (IsPrimitiveType(type))
        {
            return serializedField.Value;
        }
        else if (serializedField.Value is IEnumerable enumerable)
        {
            var list = new List<object>();
            foreach (object element in enumerable)
            {
            }
        }

        if (serializedField.Value is SerializedObject serializedObject)
        {
            return Deserialize(serializedObject);
        }

        throw new NotSupportedException();
    }

    private static bool IsPrimitiveType(Type type)
    {
        return type.IsPrimitive || type.IsValueType || type == typeof(string);
    }
}