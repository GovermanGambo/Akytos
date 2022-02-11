using System.Numerics;
using Akytos.Editor;
using Akytos.Graphics.Buffers;
using ImGuiNET;

namespace Windmill.Panels;

internal class ViewportPanel : IEditorPanel
{
    private readonly IEditorViewport m_editorViewport;
    
    private Vector2 m_viewportSize;

    public ViewportPanel(IEditorViewport editorViewport)
    {
        m_editorViewport = editorViewport;
    }

    public void Dispose()
    {
        
    }

    public string DisplayName => "Viewport";
    public bool IsEnabled { get; set; } = true;

    public IFramebuffer Framebuffer { get; set; } = null!;

    public void OnDrawGui()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.Begin(DisplayName);

        var viewportPanelSize = ImGui.GetContentRegionAvail();
        if (m_viewportSize != viewportPanelSize)
        {
            Framebuffer.Resize((uint) viewportPanelSize.X, (uint) viewportPanelSize.Y);
            m_viewportSize = viewportPanelSize;

            m_editorViewport.Camera.SetProjection((int)viewportPanelSize.X, (int)viewportPanelSize.Y);
        }

        var textureId = Framebuffer.GetColorAttachmentRendererId();
        ImGui.Image((IntPtr) textureId, m_viewportSize, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));

        ImGui.End();
        ImGui.PopStyleVar();
    }
}