using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Akytos;
using Akytos.Editor;
using Akytos.Events;
using Akytos.Graphics.Buffers;
using Akytos.SceneSystems;
using ImGuiNET;
using Windmill.Modals;
using Windmill.Resources;
using Windmill.Services;
using Math = System.Math;

namespace Windmill.Panels;

internal class ViewportPanel : IEditorPanel
{
    private readonly IEditorViewport m_editorViewport;
    private readonly GizmoService m_gizmoService;
    private readonly ModalStack m_modalStack;
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly Vector2[] m_viewportBounds = new Vector2[2];
    private Vector2 m_currentCursorPosition;
    private Vector2 m_dragStartPosition;

    private Node? m_hoveredNode;
    private bool m_isDragging;
    private bool m_isFocused;

    public ViewportPanel(IEditorViewport editorViewport, GizmoService gizmoService,
        SceneEditorContext sceneEditorContext, ModalStack modalStack)
    {
        m_editorViewport = editorViewport;
        m_gizmoService = gizmoService;
        m_sceneEditorContext = sceneEditorContext;
        m_modalStack = modalStack;
    }

    public IFramebuffer Framebuffer { get; set; } = null!;

    public void Dispose()
    {
    }

    public string DisplayName => LocalizedStrings.Viewport;
    public bool IsEnabled { get; set; } = true;

    public void OnDrawGui()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.Begin(DisplayName);

        m_isFocused = ImGui.IsWindowFocused();

        var viewportPanelSize = ImGui.GetContentRegionAvail();
        if (m_editorViewport.Size != viewportPanelSize) OnViewportResized(viewportPanelSize);

        uint textureId = Framebuffer.GetColorAttachmentRendererId();
        ImGui.Image((IntPtr) textureId, m_editorViewport.Size, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));

        var viewportMinRegion = ImGui.GetWindowContentRegionMin();
        var viewportMaxRegion = ImGui.GetWindowContentRegionMax();
        var viewportOffset = ImGui.GetWindowPos();
        m_viewportBounds[0] = viewportMinRegion + viewportOffset;
        m_viewportBounds[1] = viewportMaxRegion + viewportOffset;

        if (m_sceneEditorContext.SelectedNode is Node2D node2D)
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                m_gizmoService.DrawGizmos(m_editorViewport.Camera, node2D);
        
        DrawGrid();

        ImGui.End();
        ImGui.PopStyleVar();
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

            if (nodeId == -1) return;

            m_hoveredNode = m_sceneEditorContext.SceneTree.CurrentScene.GetChildren(true, node => node.Id == nodeId)
                .FirstOrDefault();

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

    private void OnViewportResized(Vector2 newViewportSize)
    {
        Framebuffer.Resize((uint) newViewportSize.X, (uint) newViewportSize.Y);

        m_editorViewport.ResizeViewport((int) newViewportSize.X, (int) newViewportSize.Y);
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

    private void DrawGrid()
    {
        var drawList = ImGui.GetWindowDrawList();
        drawList.PushClipRect(m_viewportBounds[0], m_viewportBounds[1], true);

        var camPosition = new Vector2(m_editorViewport.Camera.Position.X * m_editorViewport.Camera.ScaleFactor + m_editorViewport.Width / 2f,
            m_editorViewport.Camera.Position.Y * m_editorViewport.Camera.ScaleFactor + m_editorViewport.Height / 2f);

        float gridStep = 128f * m_editorViewport.Camera.ScaleFactor;
        for (float x = camPosition.X % gridStep; x < m_editorViewport.Size.X; x += gridStep)
        {
            uint color = Math.Abs(x - camPosition.X) < 0.001f ? 0x88ff0000 : 0x88ffffff;
            drawList.AddLine(new Vector2(m_viewportBounds[1].X - x, m_viewportBounds[0].Y), new Vector2(m_viewportBounds[1].X - x, m_viewportBounds[1].Y), color);
        }

        for (float y = camPosition.Y % gridStep; y < m_editorViewport.Size.Y; y += gridStep)
        {
            uint color = Math.Abs(y - camPosition.Y) < 0.001f ? 0x880000ff : 0x88ffffff;
            drawList.AddLine(new Vector2(m_viewportBounds[0].X, m_viewportBounds[0].Y + y), new Vector2(m_viewportBounds[1].X, m_viewportBounds[0].Y + y), color);
        }
            
            
        drawList.PopClipRect();
    }

    private bool OnMouseScrolled(MouseScrolledEvent e)
    {
        if (e.Vertical == 0) return false;

        m_editorViewport.Camera.ScaleFactor += e.Vertical * 0.1f;

        return true;
    }

    private bool OnMouseDownEvent(MouseDownEvent e)
    {
        if (e.MouseButton == MouseButton.Left && !m_gizmoService.IsUsing)
        {
            if (m_hoveredNode != null)
            {
                m_sceneEditorContext.SelectedNode = m_hoveredNode;
            }
            
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
        }

        return false;
    }

    private bool OnKeyUpEvent(KeyUpEvent e)
    {
        switch (e.KeyCode)
        {
            case KeyCode.ControlLeft:
            case KeyCode.ControlRight:
                m_gizmoService.IsSnapping = false;
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
}