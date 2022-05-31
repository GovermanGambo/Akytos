using Akytos;
using Akytos.Assets;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Layers;
using Akytos.SceneSystems;
using Akytos.Windowing;
using Pong.Nodes;

namespace Pong;

internal class Game : ILayer
{
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly ISpriteBatch m_spriteBatch;
    private readonly ICamera m_camera;
    private readonly SceneTree m_sceneTree;
    private readonly IAssetManager m_assetManager;

    public Game(IGraphicsDevice graphicsDevice, ISpriteBatch spriteBatch, IGameWindow window, SceneTree sceneTree, IAssetManager assetManager)
    {
        m_graphicsDevice = graphicsDevice;
        m_spriteBatch = spriteBatch;
        m_sceneTree = sceneTree;
        m_assetManager = assetManager;

        m_camera = new OrthographicCamera(window.Width, window.Height);
    }

    public void Dispose()
    {
        
    }

    public bool IsEnabled { get; set; } = true;
    
    public void OnAttach()
    {
        m_sceneTree.Systems.Register<SpriteRendererSystem>();
        var white = m_assetManager.Load<ITexture2D>("Sprites/white.png") as Texture2DAsset;

        var rootNode = new Node2D("Root");
        var playerOne = new PlayerController("Player_1", ControlScheme.Arrows);
        playerOne.AddChild(new SpriteNode("Sprite", white));
        var playerTwo = new PlayerController("Player_2", ControlScheme.WASD);
        playerTwo.AddChild(new SpriteNode("Sprite", white));

        rootNode.AddChild(playerOne);
        rootNode.AddChild(playerTwo);
        
        m_sceneTree.SetScene(rootNode);
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