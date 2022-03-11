using System.Reflection;

namespace Akytos
{
    public static class NodeUtils
    {
        public static IEnumerable<FieldInfo> GetSerializedFields(Type nodeType)
        {
            IEnumerable<FieldInfo> fields;
            if (nodeType.GetCustomAttribute<SerializableAttribute>() != null)
            {
                fields = nodeType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            }
            else
            {
                fields = nodeType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(e => e.IsDefined(typeof(SerializeFieldAttribute), true));
            }
            

            var baseType = nodeType.BaseType;

            if (baseType != null)
            {
                fields = GetSerializedFields(baseType).Concat(fields);
            }

            return fields;
        }
        
        public static IEnumerable<FieldInfo> GetSerializedFields<TNode>() where TNode : Node
        {
            return GetSerializedFields(typeof(TNode));
        }
    }
}