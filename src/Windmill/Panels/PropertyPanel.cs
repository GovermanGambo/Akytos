using System.Reflection;
using Akytos;
using Akytos.Editor;
using Akytos.Events;
using ImGuiNET;
using LightInject;
using Windmill.Services;

namespace Windmill.Panels;

internal class PropertyPanel : IEditorPanel
{
    private const ImGuiTreeNodeFlags TreeNodeFlags = ImGuiTreeNodeFlags.DefaultOpen |
                                                     ImGuiTreeNodeFlags.Framed |
                                                     ImGuiTreeNodeFlags.FramePadding |
                                                     ImGuiTreeNodeFlags.AllowItemOverlap;

    private readonly SceneEditorContext m_sceneEditorContext;
    private bool m_isEditingName;
    private IServiceFactory m_serviceFactory;

    public PropertyPanel(SceneEditorContext sceneEditorContext, IServiceFactory serviceFactory)
    {
        m_sceneEditorContext = sceneEditorContext;
        m_serviceFactory = serviceFactory;
    }

    public void Dispose()
    {
        
    }

    public string DisplayName => "Properties";
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

        if (m_sceneEditorContext.SelectedNode == null)
        {
            ImGui.End();
            return;
        }

        var serializedObject = SerializedObject.Create(m_sceneEditorContext.SelectedNode);

        foreach (var serializedField in serializedObject.Fields)
        {
            const BindingFlags bf = BindingFlags.Instance | 
                                    BindingFlags.NonPublic | 
                                    BindingFlags.DeclaredOnly;
            
            FieldInfo? fieldInfo;
            var type = m_sceneEditorContext.SelectedNode.GetType();
            while ((fieldInfo = type.GetField(serializedField.Key, bf)) == null && (type = type.BaseType) != null)
            {
            }

            if (fieldInfo == null)
            {
                continue;
            }

            if (fieldInfo.GetCustomAttribute<HideInInspectorAttribute>() != null)
            {
                continue;
            }
            
            var fieldType = Type.GetType(serializedField.Type);

            if (fieldType == null)
            {
                Debug.LogError("Type {0} was not found.", serializedField.Type);
                continue;
            }
            
            string fieldKey = serializedField.Key;

            var controlRendererType = typeof(IGuiControlRenderer<>).MakeGenericType(fieldType);

            var guiControlRenderer = m_serviceFactory.TryGetInstance(controlRendererType) as IGuiControlRenderer;

            if (guiControlRenderer == null)
            {
                // TODO: Automatic handling of serialized objects.
                throw new NotSupportedException();
            }
            
            object currentValue = guiControlRenderer.DrawControl(fieldKey, serializedField.Value);

            if (currentValue != fieldInfo.GetValue(m_sceneEditorContext.SelectedNode))
            {
                fieldInfo.SetValue(m_sceneEditorContext.SelectedNode, currentValue);
            }
        }
    }

    private void DrawHeader(Node target)
    {
        string name = target.Name;
        if (ImGui.InputText("", ref name, 50))
        {
            m_isEditingName = true;
        }

        if (!m_isEditingName || !ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Enter)))
        {
            return;
        }

        target.Name = name;
        m_isEditingName = false;
    }

    public void OnEvent(IEvent e)
    {
    }
}