namespace Akytos.Graphics;

public interface IShaderProgram : IGraphicsResource
{
    string Name { get; }
    void Bind();
    void Unbind();
}