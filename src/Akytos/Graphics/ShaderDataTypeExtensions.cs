namespace Akytos.Graphics;

public static class ShaderDataTypeExtensions
{
    public static int GetSize(this ShaderDataType shaderDataType)
    {
        return shaderDataType switch
        {
            ShaderDataType.Float => 4,
            ShaderDataType.Float2 => 4 * 2,
            ShaderDataType.Float3 => 4 * 3,
            ShaderDataType.Float4 => 4 * 4,
            _ => throw new ArgumentOutOfRangeException(nameof(shaderDataType), shaderDataType, null)
        };
    }
    
    public static int GetComponentCount(this ShaderDataType shaderDataType)
    {
        return shaderDataType switch
        {
            ShaderDataType.Float => 1,
            ShaderDataType.Float2 => 2,
            ShaderDataType.Float3 => 3,
            ShaderDataType.Float4 => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(shaderDataType), shaderDataType, null)
        };
    }
}