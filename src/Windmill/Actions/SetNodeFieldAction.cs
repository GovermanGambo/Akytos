using System.Reflection;
using Akytos;

namespace Windmill.Actions;

public class SetNodeFieldAction : IEditorAction
{
    private readonly object m_node;
    private readonly FieldInfo m_field;
    private readonly object m_value;
    private readonly object? m_previousValue;

    public SetNodeFieldAction(object node, FieldInfo field, object value)
    {
        m_node = node;
        m_field = field;
        m_value = value;
        m_previousValue = m_field.GetValue(m_node);
    }

    public void Execute()
    {
        m_field.SetValue(m_node, m_value);
    }

    public void Undo()
    {
        m_field.SetValue(m_node, m_previousValue);
    }
}