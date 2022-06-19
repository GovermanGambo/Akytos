using Akytos.Events;

namespace Akytos.Windowing;

public interface IGameWindow : IDisposable
{
    string Title { get; set; }
    int Width { get; }
    int Height { get; }
    bool IsClosing { get; }
    bool IsInitialized { get; }
    double Time { get; }
    bool IsVisible { get; }
    bool IsVSyncEnabled { get; set; }
    internal void SetEventCallback(Action<IEvent> e);
    object GetNativeWindow();
    void Initialize();
    void OnUpdate();
    void PollEvents();
    void Close();
}