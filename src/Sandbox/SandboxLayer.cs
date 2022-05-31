using Akytos;
using Akytos.Configuration;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Layers;
using Akytos.SceneSystems;

namespace Sandbox;

internal class SandboxLayer : ILayer
{
    private readonly AkytosProject m_akytosProject;
    private readonly SceneLoader m_sceneLoader;
    private readonly SceneTree m_sceneTree;
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly SpriteRendererSystem m_spriteRendererSystem;
    
    public SandboxLayer(AkytosProject akytosProject, SceneLoader sceneLoader, SceneTree sceneTree, IGraphicsDevice graphicsDevice, SpriteRendererSystem spriteRendererSystem)
    {
        m_akytosProject = akytosProject;
        m_sceneLoader = sceneLoader;
        m_sceneTree = sceneTree;
        m_graphicsDevice = graphicsDevice;
        m_spriteRendererSystem = spriteRendererSystem;
    }

    public void Dispose()
    {
        
    }

    public bool IsEnabled { get; set; } = true;
    public void OnAttach()
    {
        string? initialScene = m_akytosProject.EditorSettings.ReadString(SystemConstants.ConfigurationKeys.LastViewedScene);

        if (initialScene == null)
        {
            throw new MissingConfigurationException(SystemConstants.ConfigurationKeys.LastViewedScene);
        }

        var rootNode = m_sceneLoader.LoadScene(initialScene);
        
        m_sceneTree.SetScene(rootNode);
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        // Update
        
        m_sceneTree.OnUpdate(time);
        
        // Render
        m_graphicsDevice.ClearColor(new Color(0.1f, 0.1f, 0.1f));
        m_graphicsDevice.Clear();
        
        m_spriteRendererSystem.OnUpdate(time);
    }

    public void OnEvent(IEvent e)
    {
    }

    public void OnDrawGui()
    {
    }
}