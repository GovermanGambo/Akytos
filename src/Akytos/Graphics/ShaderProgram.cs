using Veldrid;

namespace Akytos.Graphics;

public class ShaderProgram
{
    public ShaderProgram(Shader vertexShader, Shader fragmentShader)
    {
        VertexShader = vertexShader;
        FragmentShader = fragmentShader;
    }

    public Shader VertexShader { get; }
    public Shader FragmentShader { get; }
}