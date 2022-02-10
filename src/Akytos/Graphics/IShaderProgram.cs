namespace Akytos.Graphics;

internal interface IShaderProgram : IGraphicsResource
{
    string Name { get; }
    void Bind();
    void Unbind();

    void SetInt(string name, int value);
}