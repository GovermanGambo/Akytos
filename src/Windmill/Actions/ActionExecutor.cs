using System.Collections.Generic;

namespace Windmill.Actions;

public class ActionExecutor
{
    private readonly Stack<IEditorAction> m_undoStack;
    private readonly Stack<IEditorAction> m_redoStack;

    public ActionExecutor()
    {
        m_undoStack = new Stack<IEditorAction>();
        m_redoStack = new Stack<IEditorAction>();
    }

    public bool CanUndo => m_undoStack.Count != 0;
    public bool CanRedo => m_redoStack.Count != 0;

    public void Execute(IEditorAction action)
    {
        action.Execute();
        
        m_undoStack.Push(action);
        
        m_redoStack.Clear();
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            return;
        }

        var action = m_undoStack.Pop();
        
        action.Undo();
        
        m_redoStack.Push(action);
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            return;
        }

        var action = m_redoStack.Pop();
        
        action.Execute();
        
        m_undoStack.Push(action);
    }
}