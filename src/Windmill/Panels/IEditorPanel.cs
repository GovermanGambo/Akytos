using System;
using Akytos.Events;

namespace Windmill.Panels;

internal interface IEditorPanel : IDisposable
{
    PanelSummary Summary { get; }
    Action<PanelSummary>? Closed { get; set; }
    void OnDrawGui();
    void OnRender();
    void OnEvent(IEvent e);
}