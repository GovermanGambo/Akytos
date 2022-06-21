using System;
using System.Numerics;
using Akytos.Events;
using Akytos.Graphics.Buffers;
using ImGuiNET;
using Windmill.Resources;
using Windmill.Runtime;

namespace Windmill.Panels;

internal class GamePanel : EditorPanel
{
    private readonly RuntimeManager m_runtimeManager;
    private readonly IFramebuffer m_framebuffer;
    
    private bool m_shouldFocus;

    public GamePanel(RuntimeManager runtimeManager, IFramebuffer gameFramebuffer)
    {
        m_runtimeManager = runtimeManager;
        m_framebuffer = gameFramebuffer;
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

        var size = new Vector2(m_framebuffer.Specification.Width, m_framebuffer.Specification.Height);
        var gamePanelSize = ImGui.GetContentRegionAvail();
        float ratio = gamePanelSize.X / size.X;
        size *= ratio;
        
        ImGui.SetCursorPosY(gamePanelSize.Y / 2f - size.Y / 2f);
        
        uint textureId = m_framebuffer.GetColorAttachmentRendererId();
        ImGui.Image((IntPtr) textureId, size, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));
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