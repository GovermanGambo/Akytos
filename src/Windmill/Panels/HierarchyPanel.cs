using Akytos;
using Akytos.Events;
using ImGuiNET;
using Windmill.Modals;
using Windmill.Services;

namespace Windmill.Panels;

internal class HierarchyPanel : IEditorPanel
{
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly ModalStack m_modalStack;

    private bool m_treeWasModified;

    public HierarchyPanel(SceneEditorContext sceneEditorContext, ModalStack modalStack)
    {
        m_sceneEditorContext = sceneEditorContext;
        m_modalStack = modalStack;
        IsEnabled = true;
    }
    
    public string DisplayName => "Scene Hierarchy";
    public bool IsEnabled { get; set; } = true;

    public bool HideInMenu => false;

    public void OnDrawGui()
    {
        bool open = IsEnabled;
        if (!ImGui.Begin(DisplayName, ref open, ImGuiWindowFlags.NoCollapse))
        {
            IsEnabled = false;
            ImGui.End();
            return;
        }

        if (ImGui.Button("Add node"))
        {
            m_modalStack.PushModal<CreateNodeModal>();
        }

        if (ImGui.BeginPopupContextWindow())
        {
            if (ImGui.Selectable("Add node..."))
            {
                m_modalStack.PushModal<CreateNodeModal>();
            }
            
            ImGui.EndPopup();
        }
        
        ImGui.Separator();

        DrawNode(m_sceneEditorContext.SceneTree.CurrentScene);

        ImGui.End();
    }

    public void OnEvent(IEvent e)
    {
    }

    private void DrawNode(Node node)
    {
        var nodePath = node.GetPath();
        if (nodePath == null)
        {
            Debug.LogWarning($"No path was found for node {node.Name}.");
            return;
        }

        bool isSelected = m_sceneEditorContext.SelectedNode?.GetPath() == nodePath;
        var flags = (isSelected ? ImGuiTreeNodeFlags.Selected : 0) |
                    ImGuiTreeNodeFlags.OpenOnArrow |
                    ImGuiTreeNodeFlags.SpanAvailWidth |
                    ImGuiTreeNodeFlags.DefaultOpen;

        bool opened = ImGui.TreeNodeEx(nodePath, flags, node.Name);

        if (ImGui.IsItemClicked())
        {
            SelectNode(node);
        }

        if (node.Owner != null && ImGui.BeginPopupContextItem("hierarchy_node_context"))
        {
            if (ImGui.Selectable("Add child..."))
            {
                m_sceneEditorContext.SelectedNode = node;
                m_modalStack.PushModal<CreateNodeModal>();
            }
            
            if (ImGui.Selectable("Delete node"))
            {
                node.Owner.RemoveChild(node, true);
                m_treeWasModified = true;
                if (m_sceneEditorContext.SelectedNode == node)
                {
                    m_sceneEditorContext.SelectedNode = null;
                }
            }
            
            ImGui.EndPopup();
        }

        if (!opened) return;

        foreach (var child in node.ImmediateChildren)
        {
            DrawNode(child);
            if (m_treeWasModified)
            {
                m_treeWasModified = false;
                break;
            }
        }

        ImGui.TreePop();
    }

    private void SelectNode(Node? node)
    {
        m_sceneEditorContext.SelectedNode = node;
    }

    public void Dispose()
    {
    }
}