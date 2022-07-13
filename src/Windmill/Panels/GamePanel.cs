using System.Numerics;
using Akytos.Graphics;
using ImGuiNET;
using Veldrid;
using Windmill.Resources;
using Windmill.Runtime;

namespace Windmill.Panels;

internal class GamePanel : EditorPanel
{
    private readonly RuntimeManager m_runtimeManager;
    private readonly FramebufferService m_framebufferService;
    private readonly ImGuiRenderer m_imGuiRenderer;
    private readonly GraphicsDevice m_graphicsDevice;
    
    private bool m_shouldFocus;

    public GamePanel(RuntimeManager runtimeManager, FramebufferService framebufferService, ImGuiRenderer imGuiRenderer, GraphicsDevice graphicsDevice)
    {
        m_runtimeManager = runtimeManager;
        m_framebufferService = framebufferService;
        m_imGuiRenderer = imGuiRenderer;
        m_graphicsDevice = graphicsDevice;
        m_runtimeManager.GameStarted += RuntimeManager_OnGameStarted;
    }

    public override void Dispose()
    {
        m_runtimeManager.GameStarted -= RuntimeManager_OnGameStarted;
    }

    protected override void OnDrawGui()
    {
        if (m_shouldFocus)
        {
            ImGui.SetWindowFocus();
            m_shouldFocus = false;
        }

        var framebuffer = m_framebufferService.GetFramebuffer("Game");
        var size = new Vector2(framebuffer.Width, framebuffer.Height);
        var gamePanelSize = ImGui.GetContentRegionAvail();
        float ratio = gamePanelSize.X / size.X;
        size *= ratio;
        
        ImGui.SetCursorPosY(gamePanelSize.Y / 2f - size.Y / 2f);

        var texture = framebuffer.ColorTargets[0].Target;
        var textureId = m_imGuiRenderer.GetOrCreateImGuiBinding(m_graphicsDevice.ResourceFactory, texture);
        ImGui.Image(textureId, size, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));
    }
    
    protected override void OnBeforeDraw()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
    }

    protected override void OnAfterDraw()
    {
        ImGui.PopStyleVar();
    }

    protected override PanelSummary ProvideSummary()
    {
        return new PanelSummary("general_game", LocalizedStrings.Game, typeof(GamePanel));
    }

    private void RuntimeManager_OnGameStarted()
    {
        m_shouldFocus = true;
    }
}