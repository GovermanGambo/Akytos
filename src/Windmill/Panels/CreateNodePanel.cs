using Akytos;
using Akytos.Events;
using ImGuiNET;
using Windmill.Services;

namespace Windmill.Panels
{
    // TODO: This should be a popup/modal. Not sure if this should be an IEditorPanel at all. Maybe dont use ImGui.Begin, but rather some BeginChild or something?
    internal class CreateNodePanel : IEditorPanel
    {
        private readonly SceneEditorContext m_context;
        private readonly Type[] m_nodeTypes;
        
        private string m_searchTerm = "";
        private Type? m_selectedNodeType;

        public CreateNodePanel(SceneEditorContext context)
        {
            m_context = context;
            m_nodeTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(Node)) || typeof(Node).IsAssignableFrom(t))
                .ToArray();
        }
        
        public Node? DefaultParentNode { get; set; }
        public string DisplayName => "Create Node";
        public bool IsEnabled { get; set; }
        public void OnDrawGui()
        {
            bool open = IsEnabled;
            if (!ImGui.Begin(DisplayName, ref open) || !ImGui.IsWindowFocused())
            {
                IsEnabled = false;
                ImGui.End();
                return;
            }

            if (ImGui.IsWindowFocused() && !ImGui.IsAnyItemActive() && !ImGui.IsMouseClicked(0))
                ImGui.SetKeyboardFocusHere(0);
            
            string searchText = m_searchTerm;
            if (ImGui.InputText("", ref searchText, 50)) m_searchTerm = searchText;

            ImGui.SameLine();
            if (ImGui.Button("Add"))
                if (m_selectedNodeType != null)
                {
                    var result = CreateNode(m_selectedNodeType, "NewNode");
                    if (result == Result.Ok) IsEnabled = false;
                }

            DrawNodeType(typeof(Node));

            ImGui.End();
        }

        public void OnEvent(IEvent e)
        {
            var dispatcher = new EventDispatcher(e);
            dispatcher.Dispatch<KeyDownEvent>(OnKeyDown);
        }

        public void Dispose()
        {
        }
        
        private void DrawNodeType(Type nodeType)
        {
            string name = nodeType.Name;

            bool isSelected = m_selectedNodeType?.Name == name;
            var flags = (isSelected ? ImGuiTreeNodeFlags.Selected : 0) |
                        ImGuiTreeNodeFlags.OpenOnArrow |
                        ImGuiTreeNodeFlags.SpanAvailWidth |
                        ImGuiTreeNodeFlags.DefaultOpen;

            bool opened = ImGui.TreeNodeEx(nodeType.GUID.ToString(), flags, name);

            if (ImGui.IsItemClicked()) m_selectedNodeType = nodeType;

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
                var result = CreateNode(m_selectedNodeType, "NewNode");
                if (result == Result.Ok) IsEnabled = false;
            }

            if (e.KeyCode == KeyCode.Escape) IsEnabled = false;

            return true;
        }
        
        private Result CreateNode(Type nodeType, string name)
        {
            var node = (Node?) Activator.CreateInstance(nodeType, name);

            if (node == null)
            {
                Debug.LogError("Failed to create Node instance of type: {0}", nodeType.FullName);
                return Result.InvalidData;
            }

            var rootNode = DefaultParentNode ?? m_context.SceneTree.CurrentScene;
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
            bool match = nodeType.Name.Contains(m_searchTerm);

            return match ? match : GetDirectInheritedTypes(nodeType).Any(DoesNodeMatchSearch);
        }
    }
}