using System.Reflection;

namespace Akytos
{
    public static class TypeExtensions
    {
        /// <summary>
        ///     Returns a list of all serialized fields to a type. A serialized field is either specified by marking a
        ///     field with <see cref="SerializeFieldAttribute"/>, or by marking an entire class with <see cref="SerializableAttribute"/>
        /// </summary>
        /// <param name="type">The type to get the serialized fields of.</param>
        /// <returns>A list of <see cref="FieldInfo"/> that are serialized fields.</returns>
        public static IEnumerable<FieldInfo> GetSerializedFields(this Type type)
        {
            IEnumerable<FieldInfo> fields;
            if (type.GetCustomAttribute<SerializableAttribute>() != null)
            {
                fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            }
            else
            {
                fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(e => e.IsDefined(typeof(SerializeFieldAttribute), true));
            }

            var baseType = type.BaseType;

            if (baseType != null)
            {
                fields = GetSerializedFields(baseType).Concat(fields);
            }

            return fields;
        }
    }
}