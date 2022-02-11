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

namespace Windmill;

internal class EditorLayer : ILayer
{
    private readonly IEditorViewport m_editorViewport;
    private readonly IGraphicsDevice m_graphicsDevice;
    private readonly IGraphicsResourceFactory m_graphicsResourceFactory;
    private readonly MenuService m_menuService;
    private readonly PanelManager m_panelManager;
    private readonly SpriteBatch m_spriteBatch;

    private IFramebuffer m_framebuffer = null!;
    private ITexture2D m_texture2D = null!;

    public EditorLayer(IGraphicsDevice graphicsDevice, IGraphicsResourceFactory graphicsResourceFactory,
        IEditorViewport editorViewport, SpriteBatch spriteBatch, PanelManager panelManager, MenuService menuService)
    {
        m_graphicsDevice = graphicsDevice;
        m_graphicsResourceFactory = graphicsResourceFactory;
        m_editorViewport = editorViewport;
        m_spriteBatch = spriteBatch;
        m_panelManager = panelManager;
        m_menuService = menuService;
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
    }

    public void OnDetach()
    {
    }

    public void OnUpdate(DeltaTime time)
    {
        m_framebuffer.Bind();

        m_graphicsDevice.ClearColor(new Color(0.1f, 0.1f, 0.1f));
        m_graphicsDevice.Clear();

        m_spriteBatch.Begin(m_editorViewport.Camera);

        m_spriteBatch.Draw(m_texture2D, Vector2.Zero, Color.White);
        m_spriteBatch.Draw(m_texture2D, new Vector2(-68f, 0f), Color.White);

        m_spriteBatch.End();

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