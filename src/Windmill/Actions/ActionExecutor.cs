using System.Collections.Generic;

namespace Windmill.Actions;

public class ActionExecutor
{
    private const int MaxStackSize = 20;
    
    private readonly LinkedList<IEditorAction> m_undoStack;
    private readonly LinkedList<IEditorAction> m_redoStack;

    public ActionExecutor()
    {
        m_undoStack = new LinkedList<IEditorAction>();
        m_redoStack = new LinkedList<IEditorAction>();
    }

    public bool CanUndo => m_undoStack.Count != 0;
    public bool CanRedo => m_redoStack.Count != 0;

    public void Execute(IEditorAction action)
    {
        action.Execute();
        
        m_undoStack.AddLast(action);
        
        m_redoStack.Clear();

        if (m_undoStack.Count > MaxStackSize)
        {
            m_undoStack.RemoveFirst();
        }
    }

    public void Undo()
    {
        if (!CanUndo)
        {
            return;
        }

        var action = m_undoStack.Last!.Value;
        m_undoStack.RemoveLast();
        
        action.Undo();
        
        m_redoStack.AddLast(action);
    }

    public void Redo()
    {
        if (!CanRedo)
        {
            return;
        }

        var action = m_redoStack.Last!.Value;
        m_redoStack.RemoveLast();
        
        action.Execute();
        
        m_undoStack.AddLast(action);
    }
}