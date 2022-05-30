using Akytos.Configuration;
using Akytos.Diagnostics.Logging;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Layers;
using Akytos.Windowing;
using LightInject;
using Silk.NET.Input;
using Silk.NET.Windowing;

namespace Akytos;

public abstract class Application : IConfigureGame, IConfigureLayers
{
    private static Application? s_application;

    private readonly AppConfigurator m_appConfigurator;
    private readonly ILayerStack m_layerStack;
    private readonly IServiceContainer m_serviceContainer;

    private bool m_disposed;
    private IGraphicsDevice m_graphicsDevice = null!;

    private string m_initialWindowTitle = "Akytos";
    private int m_initialWindowWidth = 960;
    private int m_initialWindowHeight = 640;
    private ImGuiLayer? m_imGuiLayer;
    
    private float m_lastFrameTime;

    private bool m_shouldRestartOnExit;
    private IGameWindow? m_window;
    private string m_workingDirectory = string.Empty;

    protected Application()
    {
        m_appConfigurator = new AppConfigurator(this);

        m_serviceContainer = new ServiceContainer();
        m_serviceContainer.RegisterSingleton<IServiceFactory>(factory => factory);
        m_serviceContainer.RegisterSingleton(_ => m_serviceContainer);

        m_layerStack = new LayerStack(m_serviceContainer);

        s_application = this;
    }

    /// <summary>
    ///     The current working directory of the application.
    /// </summary>
    public static string WorkingDirectory
    {
        get
        {
            if (s_application is null) throw new NullReferenceException("Application has not been initialized!");

            if (s_application.m_workingDirectory == string.Empty)
                throw new Exception("Working directory was accessed before it was set!");

            return s_application.m_workingDirectory;
        }
        set
        {
            if (s_application is null) throw new NullReferenceException("Application has not been initialized!");

            s_application.m_workingDirectory = value;
        }
    }

    public void SetInitialWindowSize(int width, int height)
    {
        m_initialWindowWidth = width;
        m_initialWindowHeight = height;
    }

    public void SetWindowTitle(string title)
    {
        m_initialWindowTitle = title;
    }

    public TLayer PushLayer<TLayer>() where TLayer : ILayer
    {
        var layer = m_layerStack.PushLayer<TLayer>();
        layer.OnAttach();

        Log.Core.Information("Pushed layer {0}.", typeof(TLayer).Name);

        return layer;
    }

    public void AddImGuiLayer()
    {
        m_imGuiLayer = PushLayer<ImGuiLayer>();
    }

    protected abstract void Configure(IAppConfigurator configurator);

    private void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    internal void Run()
    {
        OnInitialize();

        Log.Core.Information("Application {0} started successfully.", m_window.Title);

        while (!m_window.IsClosing)
        {
            m_window.OnUpdate();

            float currentTime = (float) m_window.Time;
            var deltaTime = new DeltaTime(currentTime - m_lastFrameTime);
            m_lastFrameTime = currentTime;

            foreach (var layer in m_layerStack)
                if (layer.IsEnabled)
                    layer.OnUpdate(deltaTime);

            UpdateImGui();

            m_window.PollEvents();
        }

        Dispose();

        if (m_shouldRestartOnExit)
        {
            OnRestart();
            m_shouldRestartOnExit = false;
        }
    }

    private void UpdateImGui()
    {
        if (m_imGuiLayer != null && m_imGuiLayer.IsEnabled)
        {
            foreach (var layer in m_layerStack)
                if (layer.IsEnabled)
                    layer.OnDrawGui();

            m_imGuiLayer.OnRender();
        }
    }

    protected virtual void RegisterServices(IServiceRegistry serviceRegistry)
    {
    }

    protected virtual void OnInitialize()
    {
        Configure(m_appConfigurator);
        
        m_window = CreateWindow(m_initialWindowTitle, m_initialWindowWidth, m_initialWindowHeight);
        
        m_window.Initialize();

        Input.Initialize(((IWindow) m_window.GetNativeWindow()).CreateInput());

        RegisterServices(m_serviceContainer);

        m_graphicsDevice = m_serviceContainer.GetInstance<IGraphicsDevice>();
    }

    protected abstract void OnRestart();

    private void Close()
    {
        m_window?.Close();
    }

    /// <summary>
    ///     Exits the application
    /// </summary>
    public static void Exit()
    {
        s_application.Close();
    }

    /// <summary>
    ///     Exits the current application and attempts to start it again.
    /// </summary>
    public static void Restart()
    {
        s_application.m_shouldRestartOnExit = true;
        s_application.Close();
    }

    private IGameWindow CreateWindow(string title, int initialWindowWidth, int initialWindowHeight)
    {
        var windowFactory = m_serviceContainer.GetInstance<IWindowFactory>();

        var props = new WindowProperties(title, initialWindowWidth, initialWindowHeight);
        var window = windowFactory.Create(props);
        window.SetEventCallback(OnEvent);
        m_serviceContainer.RegisterSingleton(_ => window, "MainWindow");

        return window;
    }

    private void OnEvent(IEvent e)
    {
        var dispatcher = new EventDispatcher(e);
        dispatcher.Dispatch<WindowResizedEvent>(OnWindowResized);

        foreach (var layer in m_layerStack.Reverse())
            if (layer.IsEnabled)
                layer.OnEvent(e);
    }

    private bool OnWindowResized(WindowResizedEvent e)
    {
        m_graphicsDevice.SetViewport(0, 0, e.Width, e.Height);

        return false;
    }

    private void Dispose(bool disposing)
    {
        if (m_disposed) return;

        if (disposing)
            // Every singleton registered in service container will get disposed
            m_serviceContainer.Dispose();

        m_disposed = true;
    }
}