using System.Collections;
using System.Reflection;
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

    [YamlMember] public string Type { get; set; }
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

            if (!IsPrimitiveType(value.GetType()))
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
                else
                {
                    // Create new serialized object if not value type
                    value = Create(value);
                }
            }

            serializedFields.Add(new SerializedField(key, type, value));
        }

        return new SerializedObject(serializedFields.ToArray(), objectType.FullName!);
    }

    private static bool IsPrimitiveType(Type type)
    {
        return type.IsPrimitive || type.IsValueType || type == typeof(string);
    }
}