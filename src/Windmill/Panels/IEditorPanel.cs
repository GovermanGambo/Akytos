namespace Windmill.Panels;

public interface IEditorPanel : IDisposable
{
    bool IsEnabled { get; }
    void OnDrawGui();
}