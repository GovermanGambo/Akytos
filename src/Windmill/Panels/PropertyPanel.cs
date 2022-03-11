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

        DrawSerializedFields();
        
        ImGui.End();

        /*if (m_sceneEditorContext.SelectedNode == null)
        
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
            
            var fieldType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == serializedField.Type);

            if (fieldType == null)
            {
                Debug.LogError("Type {0} was not found.", serializedField.Type);
                continue;
            }

            var attribute = fieldInfo.GetCustomAttribute<SerializeFieldAttribute>();
            
            object currentValue = RenderAnonymousField(fieldType, attribute?.Name ?? "N/A", serializedField, attribute);

            if (currentValue != fieldInfo.GetValue(m_sceneEditorContext.SelectedNode))
            {
                if (currentValue is SerializedObject serialized)
                {
                }
                else
                {
                    fieldInfo.SetValue(m_sceneEditorContext.SelectedNode, currentValue);
                }
                
                
            }
        }
        */
    }

    private void DrawSerializedFields()
    {
        if (m_sceneEditorContext.SelectedNode == null)
        {
            return;
        }

        DrawSerializedFieldsForType(m_sceneEditorContext.SelectedNode);
    }

    private void DrawSerializedFieldsForType(object o)
    {
        var type = o.GetType();
        var serializedFields = NodeUtils.GetSerializedFields(type);

        Type? previousDeclaringType = null;
        bool collapsed = false;
        
        foreach (var serializedField in serializedFields)
        {
            var fieldType = serializedField.FieldType;

            if (serializedField.GetCustomAttribute<HideInInspectorAttribute>() != null)
            {
                continue;
            }
            
            var controlRendererType = typeof(IGuiControlRenderer<>).MakeGenericType(fieldType);
            var guiControlRenderer = m_serviceFactory.TryGetInstance(controlRendererType) as IGuiControlRenderer;

            var fieldValue = serializedField.GetValue(o);
            var declaringType = serializedField.DeclaringType;

            if (declaringType != previousDeclaringType && declaringType != typeof(Node))
            {
                collapsed = ImGui.CollapsingHeader(declaringType.Name, ImGuiTreeNodeFlags.DefaultOpen);
                
                previousDeclaringType = declaringType;
            } 
            
            if (!collapsed && declaringType != typeof(Node))
            {
                continue;
            }

            if (guiControlRenderer != null)
            {
                var attribute = serializedField.GetCustomAttribute<SerializeFieldAttribute>();
                object currentValue =
                    guiControlRenderer.DrawControl(attribute.Name, fieldValue, attribute);
                
                serializedField.SetValue(o, currentValue);
            }
            else
            {
                DrawSerializedFieldsForType(fieldValue);
            }
        }
    }

    private object RenderAnonymousField(Type fieldType, string fieldKey, SerializedField serializedField, Attribute? attribute)
    {
        var controlRendererType = typeof(IGuiControlRenderer<>).MakeGenericType(fieldType);

        var guiControlRenderer = m_serviceFactory.TryGetInstance(controlRendererType) as IGuiControlRenderer;

        if (guiControlRenderer == null)
        {
            // TODO: Automatic handling of serialized objects.
            throw new NotSupportedException();
        }
        
         object currentValue = guiControlRenderer.DrawControl(fieldKey, serializedField.Value, attribute);

         return currentValue;
    }

    public void OnEvent(IEvent e)
    {
    }
}