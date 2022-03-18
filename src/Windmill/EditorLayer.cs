
using System.Collections.Generic;
using Akytos;
using Akytos.Editor;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Graphics.Buffers;
using Akytos.Layers;
using ImGuiNET;
using Windmill.Panels;
using Windmill.Services;

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
    private readonly ModalStack m_modalStack;

    private IFramebuffer m_framebuffer = null!;
    private ITexture2D m_texture2D = null!;

    public EditorLayer(IGraphicsDevice graphicsDevice, IGraphicsResourceFactory graphicsResourceFactory,
        IEditorViewport editorViewport, SpriteRendererSystem spriteRenderingSystem, PanelManager panelManager,
        MenuService menuService, SceneTree sceneTree, ModalStack modalStack)
    {
        m_graphicsDevice = graphicsDevice;
        m_graphicsResourceFactory = graphicsResourceFactory;
        m_editorViewport = editorViewport;
        m_renderingSystem = spriteRenderingSystem;
        m_panelManager = panelManager;
        m_menuService = menuService;
        m_sceneTree = sceneTree;
        m_modalStack = modalStack;
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
                new(FramebufferTextureFormat.RedInteger),
                new(FramebufferTextureFormat.Depth)
            });

        m_framebuffer = m_graphicsResourceFactory.CreateFramebuffer(framebufferSpecification);

        m_panelManager.Initialize();

        var viewportPanel = m_panelManager.GetPanel<ViewportPanel>();
        viewportPanel.Framebuffer = m_framebuffer;

        m_renderingSystem.Camera = m_editorViewport.Camera;

        // TODO: Temporary Scene Setup
        
        var node = new Node("RootNode");

        m_sceneTree.SetScene(node);
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
        m_panelManager.OnEvent(e);
        m_modalStack.Peek()?.OnEvent(e);
    }

    public void OnDrawGui()
    {
        Dockspace.Begin();

        m_menuService.OnDrawGui();

        m_panelManager.OnDrawGui();

        m_modalStack.OnDrawGui();

        Dockspace.End();
    }
}