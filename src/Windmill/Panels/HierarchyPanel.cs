using Akytos;
using Akytos.Events;
using ImGuiNET;
using Windmill.Modals;
using Windmill.Services;

namespace Windmill.Panels;

internal class HierarchyPanel : IEditorPanel
{
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly CreateNodePanel m_createNodePanel;

    private bool m_treeWasModified;

    public HierarchyPanel(SceneEditorContext sceneEditorContext, IEditorPanel createNodePanel)
    {
        m_sceneEditorContext = sceneEditorContext;
        m_createNodePanel = createNodePanel as CreateNodePanel;
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

        bool shouldOpenModal = ImGui.Button("Add node");

        if (ImGui.BeginPopupContextWindow())
        {
            if (ImGui.Selectable("Add node..."))
            {
                shouldOpenModal = true;
            }
            
            ImGui.EndPopup();
        }
        
        ImGui.Separator();

        DrawNode(m_sceneEditorContext.SceneTree.CurrentScene, ref shouldOpenModal);
        
        if (shouldOpenModal)
        {
            m_createNodePanel.IsEnabled = true;
            ImGui.OpenPopup("Create Node");
        }
        
        m_createNodePanel.OnDrawGui();

        ImGui.End();
    }

    public void OnEvent(IEvent e)
    {
    }

    private void DrawNode(Node node, ref bool shouldOpenModal)
    {
        var nodePath = node.GetPath();
        if (nodePath == null)
        {
            Debug.LogWarning($"No path was found for node {node.Name}.");
            shouldOpenModal = false;
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
                shouldOpenModal = true;
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
            DrawNode(child, ref shouldOpenModal);
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