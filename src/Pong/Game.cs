using Akytos;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Layers;
using Akytos.SceneSystems;
using Akytos.Windowing;

namespace Pong;

internal class Game : ILayer
{
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly ISpriteBatch m_spriteBatch;
    private readonly ICamera m_camera;
    private readonly SceneTree m_sceneTree;

    public Game(IGraphicsDevice graphicsDevice, ISpriteBatch spriteBatch, IGameWindow window, SceneTree sceneTree)
    {
        m_graphicsDevice = graphicsDevice;
        m_spriteBatch = spriteBatch;
        m_sceneTree = sceneTree;

        m_camera = new OrthographicCamera(window.Width, window.Height);
    }

    public void Dispose()
    {
        
    }

    public bool IsEnabled { get; set; } = true;
    
    public void OnAttach()
    {
        m_sceneTree.Systems.Register<SpriteRendererSystem>();
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        m_graphicsDevice.ClearColor(Color.Blue);
        m_graphicsDevice.Clear();
        
        m_spriteBatch.Begin(m_camera);
        
        m_spriteBatch.End();
    }

    public void OnEvent(IEvent e)
    {
    }

    public void OnDrawGui()
    {
    }
}