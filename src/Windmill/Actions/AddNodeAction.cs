using Akytos;
using Akytos.SceneSystems;
using Windmill.Services;

namespace Windmill.Actions;

internal class AddNodeAction : IEditorAction
{
    private readonly SceneEditorContext m_sceneEditorContext;
    private readonly Node m_node;
    private readonly Node m_owner;

    public AddNodeAction(Node node, Node owner, SceneEditorContext sceneEditorContext)
    {
        m_node = node;
        m_owner = owner;
        m_sceneEditorContext = sceneEditorContext;
    }

    public void Execute()
    {
        m_owner.AddChild(m_node, true);
    }

    public void Undo()
    {
        m_owner.RemoveChild(m_node);
        if (m_sceneEditorContext.SelectedNode == m_node)
        {
            m_sceneEditorContext.SelectedNode = null;
        }
    }
}