using System.Numerics;
using Akytos.Assertions;
using Silk.NET.OpenGL;

namespace Akytos.Graphics;

internal class OpenGLShaderProgram : IShaderProgram
{
    private readonly GL m_gl;

    public OpenGLShaderProgram(GL gl, string filePath)
    {
        m_gl = gl;

        Name = Path.GetFileNameWithoutExtension(filePath);

        var shaderIds = new List<uint>();
        
        string shaderSource = File.ReadAllText(filePath);
        var processedSources = PreProcessShaderSource(shaderSource);

        Handle = new GraphicsHandle(m_gl.CreateProgram());
            
        foreach ((var key, string value) in processedSources)
        {
            uint shaderId = CompileShader(value, key);
            m_gl.AttachShader(Handle, shaderId);
            shaderIds.Add(shaderId);
        }

        m_gl.LinkProgram(Handle);
        m_gl.ValidateProgram(Handle);

        foreach (uint shaderId in shaderIds)
        {
            m_gl.DetachShader(Handle, shaderId);
            m_gl.DeleteShader(shaderId);
        }
    }

    public void Dispose()
    {
        m_gl.DeleteProgram(Handle);
    }

    public GraphicsHandle Handle { get; }

    public string Name { get; }

    public void Bind()
    {
        m_gl.UseProgram(Handle);
    }

    public void Unbind()
    {
        m_gl.UseProgram(0);
    }

    public void SetInt(string name, int value)
    {
        int uniformLocation = m_gl.GetUniformLocation(Handle, name);
        Assert.AreNotEqual(-1, uniformLocation, $"Uniform {name} does not exist in shader!");
        m_gl.Uniform1(uniformLocation, value);
    }

    public unsafe void SetMat4(string name, Matrix4x4 value)
    {
        int uniformLocation = m_gl.GetUniformLocation(Handle, name);
        Assert.AreNotEqual(-1, uniformLocation, $"Uniform {name} does not exist in shader!");
        m_gl.UniformMatrix4(uniformLocation, 1, false, (float*)&value);
    }

    private uint CompileShader(string shaderSource, ShaderType type)
    {
        uint shaderId = m_gl.CreateShader(type);
        m_gl.ShaderSource(shaderId, shaderSource);
        m_gl.CompileShader(shaderId);

        return shaderId;
    }

    private static Dictionary<ShaderType, string> PreProcessShaderSource(string sourceString)
    {
        var result = new Dictionary<ShaderType, string>();

        string[] shaderSources = sourceString.Split("#type ");

        foreach (var shaderSource in shaderSources)
        {
            if (shaderSource == "") continue;

            string newLine = PlatformConstants.NewLine;
                
            string type = shaderSource[..shaderSource.IndexOf(newLine, StringComparison.Ordinal)];
            var glType = ShaderTypeFromString(type);
            string source = shaderSource[(shaderSource.IndexOf(newLine, StringComparison.Ordinal) + 1)..];
            result.Add(glType, source);
        }

        return result;
    }
        
    private static ShaderType ShaderTypeFromString(string type)
    {
        return type switch
        {
            "vertex" => ShaderType.VertexShader,
            "fragment" => ShaderType.FragmentShader,
            _ => throw new ArgumentException($"Invalid OpenGL shader type: {type}")
        };
    }
}