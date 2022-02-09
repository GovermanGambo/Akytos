using Silk.NET.OpenGL;

namespace Akytos.Graphics;

internal static class OpenGLShaderDataTypeExtensions
{
    public static GLEnum ToGLShaderType(this ShaderDataType shaderDataType)
    {
        return shaderDataType switch
        {
            ShaderDataType.Float => GLEnum.Float,
            ShaderDataType.Float2 => GLEnum.Float,
            ShaderDataType.Float3 => GLEnum.Float,
            ShaderDataType.Float4 => GLEnum.Float,
            ShaderDataType.Int => GLEnum.Int,
            ShaderDataType.Int2 => GLEnum.Int,
            ShaderDataType.Int3 => GLEnum.Int,
            ShaderDataType.Int4 => GLEnum.Int,
            ShaderDataType.Bool => GLEnum.Bool,
            _ => throw new ArgumentOutOfRangeException(nameof(shaderDataType), shaderDataType, null)
        };
    }
}