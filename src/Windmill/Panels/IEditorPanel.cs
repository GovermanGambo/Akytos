using Akytos.Events;

namespace Windmill.Panels;

internal interface IEditorPanel : IDisposable
{
    string DisplayName { get; }
    bool IsEnabled { get; set; }
    bool HideInMenu { get; }
    void OnDrawGui();
    void OnEvent(IEvent e);
}