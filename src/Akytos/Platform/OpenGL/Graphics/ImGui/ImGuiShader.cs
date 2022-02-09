using System.Runtime.CompilerServices;
using Silk.NET.OpenGL;

namespace Akytos.Graphics;

internal struct UniformFieldInfo
{
    public int Location;
    public string Name;
    public int Size;
    public UniformType Type;
}

internal class ImGuiShader
{
    private readonly Dictionary<string, int> m_attribLocation = new();
    private readonly Dictionary<string, int> m_uniformToLocation = new();
    private readonly GL m_gl;
    private bool m_initialized;

    public ImGuiShader(GL gl, string vertexShader, string fragmentShader)
    {
        m_gl = gl;
        (ShaderType Type, string Path)[] files = new[]
        {
            (ShaderType.VertexShader, vertexShader),
            (ShaderType.FragmentShader, fragmentShader)
        };
        Program = CreateProgram(files);
    }

    public uint Program { get; }

    public void UseShader()
    {
        m_gl.UseProgram(Program);
    }

    public void Dispose()
    {
        if (m_initialized)
        {
            m_gl.DeleteProgram(Program);
            m_initialized = false;
        }
    }

    public UniformFieldInfo[] GetUniforms()
    {
        m_gl.GetProgram(Program, GLEnum.ActiveUniforms, out var uniformCount);

        var uniforms = new UniformFieldInfo[uniformCount];

        for (var i = 0; i < uniformCount; i++)
        {
            var name = m_gl.GetActiveUniform(Program, (uint)i, out var size, out var type);

            UniformFieldInfo fieldInfo;
            fieldInfo.Location = GetUniformLocation(name);
            fieldInfo.Name = name;
            fieldInfo.Size = size;
            fieldInfo.Type = type;

            uniforms[i] = fieldInfo;
        }

        return uniforms;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetUniformLocation(string uniform)
    {
        if (m_uniformToLocation.TryGetValue(uniform, out var location) == false)
        {
            location = m_gl.GetUniformLocation(Program, uniform);
            m_uniformToLocation.Add(uniform, location);

            if (location == -1) Debug.LogError($"The uniform '{uniform}' does not exist in the shader!");
        }

        return location;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetAttribLocation(string attrib)
    {
        if (m_attribLocation.TryGetValue(attrib, out var location) == false)
        {
            location = m_gl.GetAttribLocation(Program, attrib);
            m_attribLocation.Add(attrib, location);

            if (location == -1) Debug.LogError($"The attrib '{attrib}' does not exist in the shader!");
        }

        return location;
    }

    private uint CreateProgram(params (ShaderType Type, string source)[] shaderPaths)
    {
        var program = m_gl.CreateProgram();

        Span<uint> shaders = stackalloc uint[shaderPaths.Length];
        for (var i = 0; i < shaderPaths.Length; i++)
            shaders[i] = CompileShader(shaderPaths[i].Type, shaderPaths[i].source);

        foreach (var shader in shaders)
            m_gl.AttachShader(program, shader);

        m_gl.LinkProgram(program);

        m_gl.GetProgram(program, GLEnum.LinkStatus, out var success);
        if (success == 0)
        {
            var info = m_gl.GetProgramInfoLog(program);
            Debug.LogError($"GL.LinkProgram had info log:\n{info}");
        }

        foreach (var shader in shaders)
        {
            m_gl.DetachShader(program, shader);
            m_gl.DeleteShader(shader);
        }

        m_initialized = true;

        return program;
    }

    private uint CompileShader(ShaderType type, string source)
    {
        var shader = m_gl.CreateShader(type);
        m_gl.ShaderSource(shader, source);
        m_gl.CompileShader(shader);

        m_gl.GetShader(shader, ShaderParameterName.CompileStatus, out var success);
        if (success == 0)
        {
            var info = m_gl.GetShaderInfoLog(shader);
            Debug.LogError($"GL.CompileShader for shader [{type}] had info log:\n{info}");
        }

        return shader;
    }
}