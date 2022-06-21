using System;
using Akytos.Events;
using ImGuiNET;

namespace Windmill.Panels;

public abstract class EditorPanel : IEditorPanel
{
    public EditorPanel()
    {
        Summary = ProvideSummary();
    }
    
    public virtual void Dispose()
    {
    }

    public PanelSummary Summary { get; }
    public Action<PanelSummary>? Closed { get; set; }

    public void DrawGui()
    {
        OnBeforeDraw();
        
        bool open = true;
        if (!ImGui.Begin(Summary.DisplayName, ref open))
        {
            ImGui.End();
            Closed?.Invoke(Summary);
            return;
        }
        
        OnDrawGui();
        
        ImGui.End();
        
        OnAfterDraw();
    }
    
    protected abstract void OnDrawGui();

    protected abstract PanelSummary ProvideSummary();

    protected virtual void OnBeforeDraw()
    {
        
    }

    protected virtual void OnAfterDraw()
    {
    }

    public virtual void OnRender()
    {
    }

    public virtual void OnEvent(IEvent e)
    {
    }
}