using System.Reflection;

namespace Akytos
{
    public static class NodeUtils
    {
        public static IEnumerable<FieldInfo> GetSerializedFields(Type nodeType)
        {
            var fields = nodeType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(e => e.IsDefined(typeof(SerializeFieldAttribute), true));

            var baseType = nodeType.BaseType;

            if (baseType != null)
            {
                fields = fields.Concat(GetSerializedFields(baseType));
            }

            return fields;
        }
        
        public static IEnumerable<FieldInfo> GetSerializedFields<TNode>() where TNode : Node
        {
            return GetSerializedFields(typeof(TNode));
        }
    }
}