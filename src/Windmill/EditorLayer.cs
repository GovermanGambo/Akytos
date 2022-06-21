using System.Collections.Generic;
using System.Numerics;
using Akytos;
using Akytos.Editor;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Graphics.Buffers;
using Akytos.Layers;
using Akytos.SceneSystems;
using Windmill.Panels;
using Windmill.ProjectManagement;
using Windmill.Services;

namespace Windmill;

internal class EditorLayer : ILayer
{
    private readonly AssemblyManager m_assemblyManager;
    private readonly EditorHotKeyService m_editorHotKeyService;
    private readonly IEditorViewport m_editorViewport;
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly IGraphicsResourceFactory m_graphicsResourceFactory;
    private readonly MenuService m_menuService;
    private readonly ModalStack m_modalStack;
    private readonly PanelManager m_panelManager;
    private readonly IProjectManager m_projectManager;
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly SceneTree m_sceneTree;
    private readonly ICamera m_camera;

    private IFramebuffer m_framebuffer = null!;
    private IFramebuffer m_gameFramebuffer = null!;

    public EditorLayer(IGraphicsDevice graphicsDevice, IGraphicsResourceFactory graphicsResourceFactory,
        IEditorViewport editorViewport, PanelManager panelManager, MenuService menuService, ModalStack modalStack, 
        EditorHotKeyService editorHotKeyService, SceneEditorContext sceneEditorContext, IProjectManager projectManager, 
        AssemblyManager assemblyManager, SceneTree sceneTree)
    {
        m_graphicsDevice = graphicsDevice;
        m_graphicsResourceFactory = graphicsResourceFactory;
        m_editorViewport = editorViewport;
        m_panelManager = panelManager;
        m_menuService = menuService;
        m_modalStack = modalStack;
        m_editorHotKeyService = editorHotKeyService;
        m_sceneEditorContext = sceneEditorContext;
        m_projectManager = projectManager;
        m_assemblyManager = assemblyManager;
        m_sceneTree = sceneTree;

        m_camera = new OrthographicCamera(245, 135);
    }

    public void Dispose()
    {
    }

    public bool IsEnabled { get; set; } = true;

    public void OnAttach()
    {
        LoadProject();
        
        // TODO: This may be superfluous when the assembly gets reloaded periodically by the monitor
        m_assemblyManager.BuildAndLoadAssemblies();
        
        var framebufferSpecification = new FrameBufferSpecification
        {
            Width = (uint) m_editorViewport.Width,
            Height = (uint) m_editorViewport.Height
        };

        var gameSpecification = new FrameBufferSpecification()
        {
            Width = 1920,
            Height = 1080
        };

        framebufferSpecification.Attachments = new FramebufferAttachmentSpecification(
            new List<FramebufferTextureSpecification>
            {
                new(FramebufferTextureFormat.Rgba8),
                new(FramebufferTextureFormat.RedInteger),
                new(FramebufferTextureFormat.Depth)
            });
        
        gameSpecification.Attachments = new FramebufferAttachmentSpecification(
            new List<FramebufferTextureSpecification>
            {
                new(FramebufferTextureFormat.Rgba8),
                new(FramebufferTextureFormat.RedInteger),
                new(FramebufferTextureFormat.Depth)
            });

        m_framebuffer = m_graphicsResourceFactory.CreateFramebuffer(framebufferSpecification);

        m_gameFramebuffer = m_graphicsResourceFactory.CreateFramebuffer(gameSpecification);
        
        m_panelManager.Initialize();

        var viewportPanel = m_panelManager.GetPanel<ViewportPanel>();
        viewportPanel.Framebuffer = m_framebuffer;

        var gamePanel = m_panelManager.GetPanel<GamePanel>();
        gamePanel.Initialize(m_gameFramebuffer, new Vector2(1920, 1080));

        m_sceneTree.Systems.Register<SpriteRendererSystem>();
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        // -- UPDATE START -- //
        
        m_sceneTree.OnUpdate(time);
        
        // -- UPDATE END -- //

        m_framebuffer.Bind();
        
        m_graphicsDevice.ClearColor(new Color(0.1f, 0.1f, 0.1f));
        m_graphicsDevice.Clear();

        // -- RENDER START -- //
        
        m_sceneTree.OnRender(m_editorViewport.Camera);

        m_panelManager.GetPanel<ViewportPanel>().OnRender();
        
        // -- RENDER END -- //

        m_framebuffer.Unbind();
        
        m_gameFramebuffer.Bind();
        m_graphicsDevice.ClearColor(new Color(0.1f, 0.1f, 0.1f));
        m_graphicsDevice.Clear();
        m_sceneTree.OnRender(m_camera);
        m_gameFramebuffer.Unbind();
    }

    public void OnEvent(IEvent e)
    {
        m_modalStack.Peek()?.OnEvent(e);
        m_panelManager.OnEvent(e);
        m_editorHotKeyService.OnEvent(e);
    }

    public void OnDrawGui()
    {
        Dockspace.Begin();

        m_menuService.OnDrawGui();

        m_panelManager.OnDrawGui();

        m_modalStack.OnDrawGui();

        Dockspace.End();
    }

    private void LoadProject()
    {
        if (!m_projectManager.LoadLastOpenedProject())
        {
            // TODO: Display project manager window at once. May have a separate launcher program?
            m_projectManager.CreateNewProject("TestProject", "C:/Akytos/TestProject");
            m_sceneEditorContext.CreateNewScene<Node2D>();
        }
        else
        {
            if (!m_sceneEditorContext.TryLoadPreviousScene()) m_sceneEditorContext.CreateNewScene<Node2D>();
        }
    }
}