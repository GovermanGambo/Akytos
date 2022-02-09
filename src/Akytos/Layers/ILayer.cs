using Akytos.Events;

namespace Akytos.Layers;

internal interface ILayer : IDisposable
{
    bool IsEnabled { get; set; }
    void OnAttach();
    void OnDetach();
    void OnUpdate(DeltaTime time);
    void OnEvent(IEvent e);
    void OnImGui();
}