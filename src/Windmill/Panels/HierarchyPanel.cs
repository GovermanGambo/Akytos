using Akytos;
using ImGuiNET;

namespace Windmill.Panels;

public class HierarchyPanel : IEditorPanel
{
    private Node? m_selectedNode;

    public HierarchyPanel(SceneTree context)
    {
        Context = context;
        IsEnabled = true;
    }
        
    public SceneTree Context { get; set; }
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

        DrawNode(Context.CurrentScene);

        ImGui.End();
    }

    public Node? SelectedNode
    {
        get => m_selectedNode;
        set => SelectNode(value);
    }

    public event EventHandler<Node?>? SelectedNodeChanged;

    private void DrawNode(Node node)
    {
        var nodePath = node.GetPath();
        if (nodePath == null)
        {
            Debug.LogWarning($"No path was found for node {node.Name}.");
            return;
        }

        bool isSelected = SelectedNode?.GetPath() == nodePath;
        var flags = (isSelected ? ImGuiTreeNodeFlags.Selected : 0) |
                    ImGuiTreeNodeFlags.OpenOnArrow |
                    ImGuiTreeNodeFlags.SpanAvailWidth;

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
        m_selectedNode = node;
        SelectedNodeChanged?.Invoke(this, node);
    }

    public void Dispose()
    {
    }
}