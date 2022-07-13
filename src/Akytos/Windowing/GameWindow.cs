using Akytos.Events;
using Veldrid;
using Veldrid.Sdl2;

namespace Akytos.Windowing;

public class GameWindow : IGameWindow
{
    private readonly Sdl2Window m_window;
    private Action<IEvent>? m_eventCallback;

    public GameWindow(Sdl2Window window)
    {
        m_window = window;
    }

    public void Dispose()
    {
    }

    public string Title
    {
        get => m_window.Title;
        set => m_window.Title = value;
    }

    public int Width => m_window.Width;
    public int Height => m_window.Height;
    public bool IsClosing => !m_window.Exists;
    public bool IsInitialized => true;
    public double Time => throw new NotImplementedException();
    public double DeltaTime => m_window.PollIntervalInMs / 1000;
    public bool IsVisible => m_window.Visible;
    
    public InputSnapshot? InputSnapshot { get; private set; }

    public bool IsVSyncEnabled
    {
        get => m_window.LimitPollRate;
        set => m_window.LimitPollRate = value;
    } 
    void IGameWindow.SetEventCallback(Action<IEvent> e)
    {
        m_eventCallback = e;
    }

    public object GetNativeWindow()
    {
        return m_window;
    }

    public void Initialize()
    {
        m_window.KeyDown += keyEvent =>
            m_eventCallback?.Invoke(new KeyDownEvent((KeyCode) keyEvent.Key, keyEvent.Repeat ? 1 : 0));
        m_window.KeyUp += keyEvent => m_eventCallback?.Invoke(new KeyUpEvent((KeyCode) keyEvent.Key));

        m_window.MouseDown += mouseEvent =>
            m_eventCallback?.Invoke(new MouseDownEvent((MouseButton) mouseEvent.MouseButton));
        m_window.MouseUp += mouseEvent =>
            m_eventCallback?.Invoke(new MouseUpEvent((MouseButton) mouseEvent.MouseButton));
        m_window.MouseMove += args =>
            m_eventCallback?.Invoke(new CursorMovedEvent(args.MousePosition.X, args.MousePosition.Y));
        m_window.MouseWheel += args => m_eventCallback?.Invoke(new MouseScrolledEvent(0f, args.WheelDelta));

        m_window.Resized += () => m_eventCallback?.Invoke(new WindowResizedEvent(Width, Height));
        m_window.Closing += () => m_eventCallback?.Invoke(new WindowClosingEvent());
    }

    public void OnUpdate()
    {
        
    }

    public void PollEvents()
    {
        InputSnapshot = m_window.PumpEvents();
    }

    public void Close()
    {
        m_window.Close();
    }
}