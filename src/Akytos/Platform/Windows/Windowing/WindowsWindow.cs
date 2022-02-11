using Akytos.Events;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Akytos.Windowing;

internal class WindowsWindow : IGameWindow
{
    private readonly IWindow m_window;
    private Action<IEvent>? m_eventCallback;

    public WindowsWindow(IWindow window)
    {
        m_window = window;
    }
    
    public string Title => m_window.Title;
    public int Width => m_window.Size.X;
    public int Height => m_window.Size.Y;
    public bool IsClosing => m_window.IsClosing;
    public bool IsInitialized => m_window.IsInitialized;
    public double Time => m_window.Time;
    public bool IsVisible => m_window.IsVisible;

    public bool IsVSyncEnabled
    {
        get => m_window.VSync;
        set => m_window.VSync = value;
    }

    void IGameWindow.SetEventCallback(Action<IEvent> eventCallback)
    {
        m_eventCallback = eventCallback;
    }

    public object GetNativeWindow()
    {
        return m_window;
    }

    public void Initialize()
    {
        m_window.Initialize();
        
        m_window.Resize += vector2D => m_eventCallback?.Invoke(new WindowResizedEvent(vector2D.X, vector2D.Y));
        m_window.Closing += () => m_eventCallback?.Invoke(new WindowClosingEvent());

        var input = m_window.CreateInput();
        foreach (var keyboard in input.Keyboards)
        {
            keyboard.KeyDown += (_, key, repeats) => m_eventCallback?.Invoke(new KeyDownEvent((KeyCode)key, repeats));
            keyboard.KeyUp += (_, key, _) => m_eventCallback?.Invoke(new KeyUpEvent((KeyCode) key));
        }

        foreach (var mouse in input.Mice)
        {
            mouse.MouseDown += (_, button) => m_eventCallback?.Invoke(new MouseDownEvent((MouseButton) button));
            mouse.MouseUp += (_, button) => m_eventCallback?.Invoke(new MouseUpEvent((MouseButton) button));
            mouse.MouseMove += (_, vector2) => m_eventCallback?.Invoke(new CursorMovedEvent(vector2.X, vector2.Y));
            mouse.Scroll += (_, wheel) => m_eventCallback?.Invoke(new MouseScrolledEvent(wheel.X, wheel.Y));
        }
    }

    public void OnUpdate()
    {
        m_window.DoUpdate();
    }

    public void PollEvents()
    {
        m_window.DoRender();
        m_window.DoEvents();
    }

    public void Close()
    {
        m_window.Close();
    }

    public void Dispose()
    {
        m_window.Dispose();
    }
}