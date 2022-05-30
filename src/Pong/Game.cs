using Akytos;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Layers;

namespace Pong;

internal class Game : ILayer
{
    private readonly IGraphicsDevice m_graphicsDevice;

    public Game(IGraphicsDevice graphicsDevice)
    {
        m_graphicsDevice = graphicsDevice;
    }

    public void Dispose()
    {
        
    }

    public bool IsEnabled { get; set; } = true;
    public void OnAttach()
    {
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        m_graphicsDevice.ClearColor(Color.Blue);
        m_graphicsDevice.Clear();
    }

    public void OnEvent(IEvent e)
    {
    }

    public void OnDrawGui()
    {
    }
}