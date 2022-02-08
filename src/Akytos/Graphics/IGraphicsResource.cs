namespace Akytos.Graphics;

public interface IGraphicsResource : IDisposable
{
    GraphicsHandle Handle { get; }
}