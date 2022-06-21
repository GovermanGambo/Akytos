using System;
using System.Numerics;
using Akytos.Events;
using Akytos.Graphics.Buffers;
using ImGuiNET;
using Windmill.Resources;
using Windmill.Runtime;

namespace Windmill.Panels;

internal class GamePanel : IEditorPanel
{
    private readonly RuntimeManager m_runtimeManager;
    
    private IFramebuffer m_framebuffer;
    private Vector2 m_size;
    private bool m_shouldFocus;

    public GamePanel(RuntimeManager runtimeManager)
    {
        m_runtimeManager = runtimeManager;
        m_runtimeManager.GameStarted += RuntimeManager_OnGameStarted;
    }

    public void Dispose()
    {
        m_runtimeManager.GameStarted -= RuntimeManager_OnGameStarted;
    }

    public string DisplayName => LocalizedStrings.Game;
    public bool IsEnabled { get; set; } = true;

    public void Initialize(IFramebuffer framebuffer, Vector2 size)
    {
        m_framebuffer = framebuffer;
        m_size = size;
    }
    
    public void OnDrawGui()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.Begin(DisplayName);

        if (m_shouldFocus)
        {
            ImGui.SetWindowFocus();
            m_shouldFocus = false;
        }
        
        var gamePanelSize = ImGui.GetContentRegionAvail();
        float ratio = gamePanelSize.X / m_size.X;
        m_size *= ratio;
        
        ImGui.SetCursorPosY(gamePanelSize.Y / 2f - m_size.Y / 2f);
        
        uint textureId = m_framebuffer.GetColorAttachmentRendererId();
        ImGui.Image((IntPtr) textureId, m_size, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));

        ImGui.End();
        ImGui.PopStyleVar();
    }

    public void OnEvent(IEvent e)
    {
    }
    
    private void RuntimeManager_OnGameStarted()
    {
        m_shouldFocus = true;
    }
}