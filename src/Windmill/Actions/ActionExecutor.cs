using System.Collections.Generic;

namespace Windmill.Actions;

public class ActionExecutor
{
    private readonly List<IEditorAction> m_performedActions;
    
    private int m_actionPointer = 0;

    public ActionExecutor()
    {
        m_performedActions = new List<IEditorAction>();
    }

    public bool CanUndo => m_actionPointer > 0;
    public bool CanRedo => m_actionPointer < m_performedActions.Count;

    private int CurrentIndex => m_actionPointer - 1;

    public void Execute(IEditorAction action)
    {
        m_actionPointer++;

        m_performedActions.Add(action);

        m_performedActions[CurrentIndex].Execute();
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            return;
        }

        m_actionPointer--;

        m_performedActions[CurrentIndex].Undo();
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            return;
        }

        m_actionPointer++;

        m_performedActions[CurrentIndex].Execute();
    }
}