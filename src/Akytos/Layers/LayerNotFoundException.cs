using System.Reflection;

namespace Akytos.Layers;

internal class LayerNotFoundException : Exception
{
    public LayerNotFoundException(MemberInfo layerType)
        : base($"Layer of type {layerType.Name} was not found.")
    {
        LayerType = layerType;
    }
    
    public MemberInfo LayerType { get; }
}