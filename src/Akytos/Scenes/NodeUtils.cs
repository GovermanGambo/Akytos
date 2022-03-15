using System.Reflection;

namespace Akytos
{
    public static class TypeExtensions
    {
        public static IEnumerable<FieldInfo> GetSerializedFields(this Type nodeType)
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
    }
}