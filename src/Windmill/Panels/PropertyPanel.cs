using System;
using System.Reflection;
using Akytos;
using Akytos.Editor;
using Akytos.Events;
using Akytos.SceneSystems;
using Akytos.Serialization;
using Akytos.Utilities;
using ImGuiNET;
using LightInject;
using Windmill.Actions;
using Windmill.Resources;
using Windmill.Services;

namespace Windmill.Panels;

internal class PropertyPanel : EditorPanel
{
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly IServiceFactory m_serviceFactory;
    private readonly ActionExecutor m_actionExecutor;

    public PropertyPanel(SceneEditorContext sceneEditorContext, IServiceFactory serviceFactory, ActionExecutor actionExecutor)
    {
        m_sceneEditorContext = sceneEditorContext;
        m_serviceFactory = serviceFactory;
        m_actionExecutor = actionExecutor;
    }

    protected override void OnDrawGui()
    {
        DrawSerializedFields();
    }

    protected override PanelSummary ProvideSummary()
    {
        return new PanelSummary("general_properties", LocalizedStrings.Properties, typeof(PropertyPanel));
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
        var serializedFields = type.GetSerializedFields();

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

            object? fieldValue = serializedField.GetValue(o);
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
                bool didChange =
                    guiControlRenderer.DrawControl(attribute.Name ?? serializedField.Name.SplitCamelCase(), ref fieldValue, attribute);
                
                //serializedField.SetValue(o, currentValue);

                if (didChange)
                {
                    var action = new SetNodeFieldAction(o, serializedField, fieldValue);
                    m_actionExecutor.Execute(action);
                }
            }
            else
            {
                DrawSerializedFieldsForType(fieldValue);
            }
        }
    }
}