using Akytos;
using Akytos.Events;
using ImGuiNET;
using Windmill.Services;

namespace Windmill.Panels;

internal class HierarchyPanel : IEditorPanel
{
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly CreateNodePanel m_createNodePanel;

    public HierarchyPanel(SceneEditorContext sceneEditorContext, CreateNodePanel createNodePanel)
    {
        m_sceneEditorContext = sceneEditorContext;
        m_createNodePanel = createNodePanel;
        IsEnabled = true;
    }
    
    public string DisplayName => "Scene Hierarchy";
    public bool IsEnabled { get; set; } = true;

    public void OnDrawGui()
    {
        bool open = IsEnabled;
        if (!ImGui.Begin(DisplayName, ref open))
        {
            IsEnabled = false;
            ImGui.End();
            return;
        }
        
        if (ImGui.Button("Add Node"))
        {
            m_createNodePanel.IsEnabled = true;
        }

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

        if (!opened) return;

        foreach (var child in node.ImmediateChildren) DrawNode(child);

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