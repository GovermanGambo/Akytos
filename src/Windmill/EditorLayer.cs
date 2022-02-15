using System.Numerics;
using Akytos;
using Akytos.Assets;
using Akytos.Editor;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Graphics.Buffers;
using Akytos.Layers;
using ImGuiNET;
using Windmill.Panels;
using Windmill.Services;
using YamlDotNet.Serialization;

namespace Windmill;

internal class EditorLayer : ILayer
{
    private readonly IEditorViewport m_editorViewport;
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly IGraphicsResourceFactory m_graphicsResourceFactory;
    private readonly MenuService m_menuService;
    private readonly PanelManager m_panelManager;
    private readonly SpriteRendererSystem m_renderingSystem;
    private readonly SceneTree m_sceneTree;

    private IFramebuffer m_framebuffer = null!;
    private ITexture2D m_texture2D = null!;

    public EditorLayer(IGraphicsDevice graphicsDevice, IGraphicsResourceFactory graphicsResourceFactory,
        IEditorViewport editorViewport, SpriteRendererSystem spriteRenderingSystem, PanelManager panelManager, MenuService menuService, SceneTree sceneTree)
    {
        m_graphicsDevice = graphicsDevice;
        m_graphicsResourceFactory = graphicsResourceFactory;
        m_editorViewport = editorViewport;
        m_renderingSystem = spriteRenderingSystem;
        m_panelManager = panelManager;
        m_menuService = menuService;
        m_sceneTree = sceneTree;
    }

    public void Dispose()
    {
    }

    public bool IsEnabled { get; set; } = true;

    public void OnAttach()
    {
        var framebufferSpecification = new FrameBufferSpecification
        {
            Width = (uint)m_editorViewport.Width,
            Height = (uint)m_editorViewport.Height
        };

        framebufferSpecification.Attachments = new FramebufferAttachmentSpecification(
            new List<FramebufferTextureSpecification>
            {
                new(FramebufferTextureFormat.Rgba8),
                new(FramebufferTextureFormat.Depth)
            });

        m_framebuffer = m_graphicsResourceFactory.CreateFramebuffer(framebufferSpecification);

        m_texture2D =
            m_graphicsResourceFactory.CreateTexture2D(Asset.GetAssetPath("sprites/character_malePerson_idle.png"));

        m_panelManager.Initialize();

        var viewportPanel = m_panelManager.GetPanel<ViewportPanel>();
        viewportPanel.Framebuffer = m_framebuffer;

        var node = new Node("RootNode");
        var node2D = new SpriteNode("Node2D")
        {
            GlobalPosition = new Vector2(-96, 0),
            Texture = m_texture2D
        };
        var spriteNode = new SpriteNode("SpriteNode")
        {
            Texture = m_texture2D
        };
        node.AddChild(node2D);
        node.AddChild(spriteNode);
        
        m_sceneTree.SetScene(node);

        m_renderingSystem.Camera = m_editorViewport.Camera;
        m_renderingSystem.Context = node;
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        m_framebuffer.Bind();

        m_graphicsDevice.ClearColor(new Color(0.1f, 0.1f, 0.1f));
        m_graphicsDevice.Clear();

        m_renderingSystem.OnUpdate(time);
        
        m_panelManager.GetPanel<ViewportPanel>().OnRender();

        m_framebuffer.Unbind();
    }

    public void OnEvent(IEvent e)
    {
    }

    public void OnDrawGui()
    {
        Dockspace.Begin();

        ImGui.ShowDemoWindow();
        m_menuService.OnDrawGui();

        m_panelManager.OnDrawGui();

        Dockspace.End();
    }
}