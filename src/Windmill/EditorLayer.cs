
using System.Collections.Generic;
using Akytos;
using Akytos.Assets;
using Akytos.Configuration;
using Akytos.Editor;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Graphics.Buffers;
using Akytos.Layers;
using Akytos.ProjectManagement;
using ImGuiNET;
using Microsoft.VisualBasic.CompilerServices;
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
    private readonly EditorHotKeyService m_editorHotKeyService;
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly IProjectManager m_projectManager;

    private IFramebuffer m_framebuffer = null!;

    public EditorLayer(IGraphicsDevice graphicsDevice, IGraphicsResourceFactory graphicsResourceFactory,
        IEditorViewport editorViewport, SpriteRendererSystem spriteRenderingSystem, PanelManager panelManager,
        MenuService menuService, SceneTree sceneTree, ModalStack modalStack, EditorHotKeyService editorHotKeyService, 
        SceneEditorContext sceneEditorContext, IProjectManager projectManager)
    {
        m_graphicsDevice = graphicsDevice;
        m_graphicsResourceFactory = graphicsResourceFactory;
        m_editorViewport = editorViewport;
        m_renderingSystem = spriteRenderingSystem;
        m_panelManager = panelManager;
        m_menuService = menuService;
        m_sceneTree = sceneTree;
        m_modalStack = modalStack;
        m_editorHotKeyService = editorHotKeyService;
        m_sceneEditorContext = sceneEditorContext;
        m_projectManager = projectManager;
    }

    public void Dispose()
    {
    }

    public bool IsEnabled { get; set; } = true;

    public void OnAttach()
    {
        if (!m_projectManager.LoadLastOpenedProject())
        {
            // TODO: Display project manager window at once
            m_projectManager.CreateNewProject("TestProject", "C:/Akytos/TestProject");
        }
        
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

        string? initialScene = AkytosProject.CurrentProject?.Configuration.ReadString("InitialScene");

        if (initialScene != null)
        {
            m_sceneEditorContext.LoadScene(initialScene);
        }
        else
        {
            m_sceneEditorContext.CreateNewScene<Node2D>();
        }
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
}