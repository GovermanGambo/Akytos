using Akytos;
using Akytos.SceneSystems;
using Windmill.Services;

namespace Windmill.Actions;

internal class DeleteNodeAction : IEditorAction
{
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly Node m_node;
    private readonly Node? m_parent;

    public DeleteNodeAction(SceneEditorContext sceneEditorContext, Node node)
    {
        m_sceneEditorContext = sceneEditorContext;
        m_node = node;
        m_parent = node.Owner;
    }

    public void Execute()
    {
        if (m_parent == null)
        {
            return;
        }
        
        m_parent.RemoveChild(m_node);
        if (m_sceneEditorContext.SelectedNode == m_node)
        {
            m_sceneEditorContext.SelectedNode = null;
        }
    }

    public void Undo()
    {
        m_parent?.AddChild(m_node);
    }
}