namespace Akytos.Graphics;

internal interface IGraphicsDevice
{
    void Clear();
    void ClearColor(Color color);
    void SetViewport(int x, int y, int width, int height);
    void DrawIndexed(IVertexArrayObject vertexArrayObject, int elementCount = 0);
}