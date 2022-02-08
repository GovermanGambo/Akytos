namespace Akytos.Graphics;

internal interface IGraphicsDevice
{
    void Clear();
    void ClearColor(Color color);
    void DrawIndexed(IVertexArrayObject vertexArrayObject);
}