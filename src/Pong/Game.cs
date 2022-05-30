using Akytos;
using Akytos.Assets;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Layers;

namespace Pong;

public class Game : ILayer
{
    private readonly IAssetManager m_assetManager;

    public Game(IAssetManager assetManager)
    {
        m_assetManager = assetManager;
    }

    public void Dispose()
    {
        
    }

    public bool IsEnabled { get; set; } = true;
    public void OnAttach()
    {
        var white = m_assetManager.Load<ITexture2D>("white.png");
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
    }

    public void OnEvent(IEvent e)
    {
    }

    public void OnDrawGui()
    {
    }
}