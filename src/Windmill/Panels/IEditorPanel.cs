using Akytos.Events;

namespace Windmill.Panels;

internal interface IEditorPanel : IDisposable
{
    string DisplayName { get; }
    bool IsEnabled { get; set; }
    void OnDrawGui();
    void OnEvent(IEvent e);
}