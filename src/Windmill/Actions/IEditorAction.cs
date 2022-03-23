namespace Windmill.Actions;

public interface IEditorAction
{
    void Execute();
    void Undo();
}