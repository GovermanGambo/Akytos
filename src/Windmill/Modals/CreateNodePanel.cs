using Akytos;
using Akytos.Events;
using ImGuiNET;
using Windmill.Panels;
using Windmill.Services;
using Math = System.Math;

namespace Windmill.Modals
{
    internal class CreateNodePanel : IEditorPanel
    {
        private readonly SceneEditorContext m_sceneEditorContext;
        private readonly Type[] m_nodeTypes;
        
        private string m_searchTerm = "";
        private Type? m_selectedNodeType;

        public CreateNodePanel(SceneEditorContext sceneEditorContext)
        {
            m_sceneEditorContext = sceneEditorContext;
            m_nodeTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(Node)) || typeof(Node).IsAssignableFrom(t))
                .ToArray();
        }
        
        public string DisplayName => "Create Node";
        public bool IsEnabled { get; set; }

        public bool HideInMenu => true;

        public void OnDrawGui()
        {
            bool open = IsEnabled;
            if (!ImGui.BeginPopupModal("Create Node", ref open))
            {
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

            ImGui.EndPopup();
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
                if (result == Result.Ok)
                {
                    IsEnabled = false;
                    ImGui.CloseCurrentPopup();
                }
            }

            if (e.KeyCode == KeyCode.Escape)
            {
                IsEnabled = false;
                ImGui.CloseCurrentPopup();
            }

            if (e.KeyCode == KeyCode.Down)
            {
                if (m_selectedNodeType == null)
                {
                    m_selectedNodeType = m_nodeTypes.FirstOrDefault();
                }
                else
                {
                    int currentIndex = Array.FindIndex(m_nodeTypes, t => t == m_selectedNodeType);
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
                    int currentIndex = Array.FindIndex(m_nodeTypes, t => t == m_selectedNodeType);
                    currentIndex = Math.Clamp(currentIndex - 1, 0, m_nodeTypes.Length - 1);
                    m_selectedNodeType = m_nodeTypes[currentIndex];
                }
            }

            return false;
        }
        
        private Result CreateNode(Type nodeType, string name)
        {
            var node = (Node?) Activator.CreateInstance(nodeType, name);

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
            bool match = nodeType.Name.Contains(m_searchTerm);

            return match ? match : GetDirectInheritedTypes(nodeType).Any(DoesNodeMatchSearch);
        }
    }
}