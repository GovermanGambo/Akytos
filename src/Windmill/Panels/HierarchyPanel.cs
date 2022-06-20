using Akytos;
using Akytos.Diagnostics.Logging;
using Akytos.Editor;
using Akytos.Events;
using Akytos.SceneSystems;
using ImGuiNET;
using Windmill.Actions;
using Windmill.Modals;
using Windmill.Resources;
using Windmill.Services;

namespace Windmill.Panels;

internal class HierarchyPanel : IEditorPanel
{
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly ActionExecutor m_actionExecutor;
    private readonly ModalStack m_modalStack;
    private readonly IEditorViewport m_editorViewport;

    private bool m_treeWasModified;
    private NodePath? m_renamingNode;

    public HierarchyPanel(SceneEditorContext sceneEditorContext, ModalStack modalStack, ActionExecutor actionExecutor, IEditorViewport editorViewport)
    {
        m_sceneEditorContext = sceneEditorContext;
        m_modalStack = modalStack;
        m_actionExecutor = actionExecutor;
        m_editorViewport = editorViewport;
        IsEnabled = true;
    }

    public string DisplayName => LocalizedStrings.SceneHierarchy;
    public bool IsEnabled { get; set; }

    public void OnDrawGui()
    {
        bool open = IsEnabled;
        if (!ImGui.Begin(DisplayName, ref open, ImGuiWindowFlags.NoCollapse))
        {
            IsEnabled = false;
            ImGui.End();
            return;
        }

        if (ImGui.Button(LocalizedStrings.AddNode_Button))
        {
            m_modalStack.PushModal<CreateNodeModal>();
        }

        if (ImGui.BeginPopupContextWindow())
        {
            if (ImGui.Selectable(LocalizedStrings.AddNode_Button))
            {
                m_modalStack.PushModal<CreateNodeModal>();
            }
            
            ImGui.EndPopup();
        }

        if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(0))
        {
            m_renamingNode = null;
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
            Log.Core.Error("No path was found for node {0}.", node.Name);
            return;
        }

        bool isSelected = m_sceneEditorContext.SelectedNode?.GetPath() == nodePath;
        var flags = (isSelected ? ImGuiTreeNodeFlags.Selected : 0) |
                    ImGuiTreeNodeFlags.OpenOnArrow |
                    ImGuiTreeNodeFlags.SpanAvailWidth |
                    ImGuiTreeNodeFlags.DefaultOpen;


        bool opened = false;
        if (m_renamingNode is null || m_renamingNode != nodePath)
        {
            opened = ImGui.TreeNodeEx(nodePath, flags, node.Name);
        }
        else
        {
            string name = node.Name;
            if (ImGui.InputText(string.Empty, ref name, 10, ImGuiInputTextFlags.EnterReturnsTrue))
            {
                node.Name = name;
                m_renamingNode = null;
            }
        }

        if (ImGui.IsItemClicked())
        {
            if (ImGui.IsMouseDoubleClicked(0))
            {
                if (node is Node2D node2D && m_editorViewport.Camera.Position != node2D.GlobalPosition)
                {
                    m_editorViewport.Camera.Position = node2D.GlobalPosition;
                }
                else
                {
                    // TODO: Rename node
                    m_renamingNode = nodePath;
                }
            }
            else
            {
                SelectNode(node);
            }
        }

        if (node.Owner != null && ImGui.BeginPopupContextItem("hierarchy_node_context"))
        {
            if (ImGui.Selectable(LocalizedStrings.AddChild))
            {
                m_sceneEditorContext.SelectedNode = node;
                m_modalStack.PushModal<CreateNodeModal>();
            }
            
            if (ImGui.Selectable(LocalizedStrings.Delete))
            {
                var deleteNodeAction = new DeleteNodeAction(m_sceneEditorContext, node);
                m_actionExecutor.Execute(deleteNodeAction);
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