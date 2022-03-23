using System;
using System.Collections.Generic;
using System.Linq;
using Akytos;
using Akytos.Events;
using ImGuiNET;
using Windmill.Services;
using Math = System.Math;

namespace Windmill.Modals;

internal class CreateNodeModal : IModal
{
    private readonly Type[] m_nodeTypes;
    private readonly SceneEditorContext m_sceneEditorContext;

    private string m_searchTerm = "";
    private Type? m_selectedNodeType;
    private bool m_isOpen;
    private bool m_shouldOpen;

    public CreateNodeModal(SceneEditorContext sceneEditorContext)
    {
        m_sceneEditorContext = sceneEditorContext;
        m_nodeTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(Node)) || typeof(Node).IsAssignableFrom(t))
            .ToArray();
    }

    public string Name => "Create Node";

    public bool IsOpen
    {
        get => m_isOpen;
        private set
        {
            m_isOpen = value;

            if (!value)
            {
                Closing?.Invoke();
            }
        }
    }

    public event Action? Closing;

    public void Open()
    {
        m_shouldOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    public void OnDrawGui()
    {
        if (m_shouldOpen)
        {
            ImGui.OpenPopup(Name);
            IsOpen = true;
            m_shouldOpen = false;
        }
        
        bool open = IsOpen;

        if (open)
        {
            ImGui.SetNextWindowSize(ImGui.GetWindowSize() / 2f);
        }
        
        if (!ImGui.BeginPopupModal(Name, ref open))
        {
            IsOpen = false;
            return;
        }

        if (ImGui.IsWindowFocused() && !ImGui.IsAnyItemActive() && !ImGui.IsMouseClicked(0))
            ImGui.SetKeyboardFocusHere(0);

        var searchText = m_searchTerm;
        if (ImGui.InputText("", ref searchText, 50)) m_searchTerm = searchText;

        ImGui.SameLine();
        if (ImGui.Button("Add"))
            if (m_selectedNodeType != null)
            {
                var result = CreateNode(m_selectedNodeType);
                if (result == Result.Ok) IsOpen = false;
            }

        DrawNodeType(typeof(Node));

        ImGui.EndPopup();
    }

    public void Dispose()
    {
    }

    public void OnEvent(IEvent e)
    {
        var dispatcher = new EventDispatcher(e);
        dispatcher.Dispatch<KeyDownEvent>(OnKeyDown);
    }

    private void DrawNodeType(Type nodeType)
    {
        var name = nodeType.Name;

        var isSelected = m_selectedNodeType?.Name == name;
        var flags = (isSelected ? ImGuiTreeNodeFlags.Selected : 0) |
                    ImGuiTreeNodeFlags.OpenOnArrow |
                    ImGuiTreeNodeFlags.SpanAvailWidth |
                    ImGuiTreeNodeFlags.DefaultOpen;

        var opened = ImGui.TreeNodeEx(nodeType.GUID.ToString(), flags, name);

        if (ImGui.IsItemClicked()) m_selectedNodeType = nodeType;
        
        if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(0))
        {
            var result = CreateNode(nodeType);
            if (result == Result.Ok) Close();
        }

        if (!opened) return;

        // TODO: Only top-level nodes will work with search
        foreach (var type in GetDirectInheritedTypes(nodeType))
        {
            if (!DoesNodeMatchSearch(type)) continue;

            DrawNodeType(type);
        }

        if (nodeType.Name == m_searchTerm) m_selectedNodeType = nodeType;

        ImGui.TreePop();
    }

    private bool OnKeyDown(KeyDownEvent e)
    {
        if (e.KeyCode == KeyCode.Enter && m_selectedNodeType != null)
        {
            var result = CreateNode(m_selectedNodeType);
            if (result == Result.Ok)
            {
                IsOpen = false;
            }
        }

        if (e.KeyCode == KeyCode.Escape)
        {
            IsOpen = false;
        }

        if (e.KeyCode == KeyCode.Down)
        {
            if (m_selectedNodeType == null)
            {
                m_selectedNodeType = m_nodeTypes.FirstOrDefault();
            }
            else
            {
                var currentIndex = Array.FindIndex(m_nodeTypes, t => t == m_selectedNodeType);
                currentIndex = Math.Clamp(currentIndex + 1, 0, m_nodeTypes.Length - 1);
                m_selectedNodeType = m_nodeTypes[currentIndex];
            }
        }
        else if (e.KeyCode == KeyCode.Up)
        {
            if (m_selectedNodeType == null)
            {
                m_selectedNodeType = m_nodeTypes.LastOrDefault();
            }
            else
            {
                var currentIndex = Array.FindIndex(m_nodeTypes, t => t == m_selectedNodeType);
                currentIndex = Math.Clamp(currentIndex - 1, 0, m_nodeTypes.Length - 1);
                m_selectedNodeType = m_nodeTypes[currentIndex];
            }
        }

        return false;
    }

    private Result CreateNode(Type nodeType)
    {
        var node = (Node?)Activator.CreateInstance(nodeType);

        if (node == null)
        {
            Debug.LogError("Failed to create Node instance of type: {0}", nodeType.FullName);
            return Result.InvalidData;
        }

        var rootNode = m_sceneEditorContext.SelectedNode ?? m_sceneEditorContext.SceneTree.CurrentScene;
        rootNode.AddChild(node, true);

        return Result.Ok;
    }

    private IEnumerable<Type> GetDirectInheritedTypes(Type type)
    {
        return m_nodeTypes
            .Where(t => t.BaseType == type);
    }

    private bool DoesNodeMatchSearch(Type nodeType)
    {
        var match = nodeType.Name.Contains(m_searchTerm);

        return match ? match : GetDirectInheritedTypes(nodeType).Any(DoesNodeMatchSearch);
    }
}