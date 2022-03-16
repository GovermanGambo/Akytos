using System.Numerics;

namespace Akytos.Graphics;

public interface IShaderProgram : IGraphicsResource
{
    string Name { get; }
    void Bind();
    void Unbind();

    void SetInt(string name, int value);
    void SetMat4(string name, Matrix4x4 value);
    void SetIntArray(string name, int[] value);
}