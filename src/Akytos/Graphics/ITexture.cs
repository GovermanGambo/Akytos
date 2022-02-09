namespace Akytos.Graphics;

public interface ITexture : IGraphicsResource
{
    int Width { get; }
    int Height { get; }
    void Bind(int slot = 0);
}