namespace Windmill.Panels;

public interface IEditorPanel : IDisposable
{
    string DisplayName { get; }
    bool IsEnabled { get; set; }
    void OnDrawGui();
}