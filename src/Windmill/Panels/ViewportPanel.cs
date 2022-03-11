using System.Numerics;
using Akytos;
using Akytos.Editor;
using Akytos.Events;
using Akytos.Graphics.Buffers;
using ImGuiNET;
using Windmill.Services;

namespace Windmill.Panels;

internal class ViewportPanel : IEditorPanel
{
    private readonly IEditorViewport m_editorViewport;
    private readonly GizmoService m_gizmoService;
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly Vector2[] m_viewportBounds = new Vector2[2];

    private Node? m_hoveredNode;
    private bool m_isFocused;
    private bool m_isDragging;
    private Vector2 m_dragStartPosition;
    private Vector2 m_currentCursorPosition;

    public ViewportPanel(IEditorViewport editorViewport, GizmoService gizmoService, SceneEditorContext sceneEditorContext)
    {
        m_editorViewport = editorViewport;
        m_gizmoService = gizmoService;
        m_sceneEditorContext = sceneEditorContext;
    }

    public void Dispose()
    {
        
    }

    public string DisplayName => "Viewport";
    public bool IsEnabled { get; set; } = true;

    public IFramebuffer Framebuffer { get; set; } = null!;
    
    public void OnRender()
    {
        var mousePos = ImGui.GetMousePos();
        mousePos.X -= m_viewportBounds[0].X;
        mousePos.Y -= m_viewportBounds[0].Y;
        var viewportSize = m_viewportBounds[1] - m_viewportBounds[0];
        mousePos.Y = viewportSize.Y - mousePos.Y;

        int mouseX = (int) mousePos.X;
        int mouseY = (int) mousePos.Y;

        if (mouseX >= 0 && mouseY >= 0 && mouseX < viewportSize.X && mouseY < viewportSize.Y)
        {
            m_currentCursorPosition = new Vector2(mouseX - m_editorViewport.Width / 2,
                mouseY - m_editorViewport.Height / 2);
            
            int nodeId = Framebuffer.ReadPixel(1, mouseX, mouseY);

            if (nodeId == -1)
            {
                return;
            }

            m_hoveredNode = m_sceneEditorContext.SceneTree.CurrentScene.GetChildren(true, node => node.Id == nodeId).FirstOrDefault();

            if (m_isDragging)
            {
                var cursorPosRelativeToWorldCenter = new Vector2(mouseX - m_editorViewport.Width / 2,
                    mouseY - m_editorViewport.Height / 2);
                
                var delta = m_dragStartPosition - cursorPosRelativeToWorldCenter;
                m_editorViewport.Camera.Position += delta / m_editorViewport.Camera.ScaleFactor;
                m_dragStartPosition = cursorPosRelativeToWorldCenter;
            }
        }
        else
        {
            m_hoveredNode = null;
        }
    }

    public void OnDrawGui()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.Begin(DisplayName);

        m_isFocused = ImGui.IsWindowFocused();
        
        var viewportPanelSize = ImGui.GetContentRegionAvail();
        if (m_editorViewport.Size != viewportPanelSize)
        {
            OnViewportResized(viewportPanelSize);
        }

        var textureId = Framebuffer.GetColorAttachmentRendererId();
        ImGui.Image((IntPtr) textureId, m_editorViewport.Size, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));
        
        var viewportMinRegion = ImGui.GetWindowContentRegionMin();
        var viewportMaxRegion = ImGui.GetWindowContentRegionMax();
        var viewportOffset = ImGui.GetWindowPos();
        m_viewportBounds[0] = viewportMinRegion + viewportOffset;
        m_viewportBounds[1] = viewportMaxRegion + viewportOffset;
        
        if (m_sceneEditorContext.SelectedNode is Node2D node2D)
        {
            m_gizmoService.DrawGizmos(m_editorViewport.Camera, node2D);
        }
        
        ImGui.End();
        ImGui.PopStyleVar();
    }

    private void OnViewportResized(Vector2 newViewportSize)
    {
        Framebuffer.Resize((uint) newViewportSize.X, (uint) newViewportSize.Y);

        m_editorViewport.ResizeViewport((int) newViewportSize.X, (int) newViewportSize.Y);
    }

    public void OnEvent(IEvent e)
    {
        var dispatcher = new EventDispatcher(e);
        dispatcher.Dispatch<KeyDownEvent>(OnKeyDownEvent, () => m_isFocused);
        dispatcher.Dispatch<KeyUpEvent>(OnKeyUpEvent);
        dispatcher.Dispatch<MouseDownEvent>(OnMouseDownEvent);
        dispatcher.Dispatch<MouseUpEvent>(OnMouseUpEvent);
        dispatcher.Dispatch<MouseScrolledEvent>(OnMouseScrolled);
    }

    private bool OnMouseUpEvent(MouseUpEvent e)
    {
        if (e.MouseButton == MouseButton.Right)
        {
            m_isDragging = false;
            return false;
        }

        return false;
    }

    private bool OnMouseScrolled(MouseScrolledEvent e)
    {
        if (e.Vertical == 0)
        {
            return false;
        }

        m_editorViewport.Camera.ScaleFactor += e.Vertical * 0.1f;

        return true;
    }

    private bool OnMouseDownEvent(MouseDownEvent e)
    {
        if (e.MouseButton == MouseButton.Left && m_hoveredNode != null && !m_gizmoService.IsUsing)
        {
            m_sceneEditorContext.SelectedNode = m_hoveredNode;
            return true;
        }

        if (e.MouseButton == MouseButton.Right)
        {
            m_dragStartPosition = m_currentCursorPosition;
            m_isDragging = true;
            return true;
        }

        return false;
    }

    private bool OnKeyDownEvent(KeyDownEvent e)
    {
        switch (e.KeyCode)
        {
            case KeyCode.ControlLeft:
            case KeyCode.ControlRight:
                m_gizmoService.IsSnapping = true;
                break;
            case KeyCode.Q:
                m_gizmoService.GizmoMode = GizmoMode.None;
                return true;
            case KeyCode.W:
                m_gizmoService.GizmoMode = GizmoMode.Translate;
                return true;
            case KeyCode.E:
                m_gizmoService.GizmoMode = GizmoMode.Rotate;
                return true;
            case KeyCode.R:
                m_gizmoService.GizmoMode = GizmoMode.Scale;
                return true;
        }

        return false;
    }

    private bool OnKeyUpEvent(KeyUpEvent e)
    {
        if (e.KeyCode == KeyCode.ControlLeft || e.KeyCode == KeyCode.ControlRight)
        {
            m_gizmoService.IsSnapping = false;
        }

        return false;
    }
}