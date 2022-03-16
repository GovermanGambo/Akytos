using System.Numerics;
using System.Runtime.CompilerServices;
using Akytos.Events;
using Akytos.Graphics;
using Akytos.Layers;
using Akytos.Windowing;
using LightInject;
using YamlDotNet.Serialization;

[assembly: InternalsVisibleTo("Windmill")]
[assembly: InternalsVisibleTo("Akytos.Tests")]
namespace Akytos;

public abstract class Application : IDisposable
{
    private static Application s_application;
    
    private readonly IServiceContainer m_serviceContainer;
    private readonly IGameWindow m_window;
    private readonly ILayerStack m_layerStack;

    private ImGuiLayer m_imGuiLayer = null!;
    private IGraphicsDevice m_graphicsDevice = null!;
    private float m_lastFrameTime;
    private bool m_disposed;

    protected Application(string title, int initialWindowWidth, int initialWindowHeight)
    {
        m_serviceContainer = new ServiceContainer();
        m_serviceContainer.RegisterSingleton<IServiceFactory>(factory => factory);
        m_window = CreateWindow(title, initialWindowWidth, initialWindowHeight);

        m_layerStack = new LayerStack(m_serviceContainer);

        s_application = this;
    }
    
    public void Run()
    {
        OnInitialize();
        
        Debug.LogInformation("Application {0} started successfully.", m_window.Title);
        
        while (!m_window.IsClosing)
        {
            m_window.OnUpdate();
            
            float currentTime = (float)m_window.Time;
            var deltaTime = new DeltaTime(currentTime - m_lastFrameTime);
            m_lastFrameTime = currentTime;
            
            foreach (var layer in m_layerStack)
            {
                if (layer.IsEnabled)
                {
                    layer.OnUpdate(deltaTime);
                }
            }

            UpdateImGui();
            
            m_window.PollEvents();
        }
        
        Dispose();
    }

    private void UpdateImGui()
    {
        if (m_imGuiLayer.IsEnabled)
        {
            foreach (var layer in m_layerStack)
            {
                if (layer.IsEnabled)
                {
                    layer.OnDrawGui();
                }
            }

            m_imGuiLayer.OnRender();
        }
    }

    internal TLayer PushLayer<TLayer>() where TLayer : ILayer
    {
        var layer = m_layerStack.PushLayer<TLayer>();
        layer.OnAttach();
        
        Debug.LogInformation("Pushed layer {0}.", typeof(TLayer).Name);
        
        return layer;
    }

    protected virtual void RegisterServices(IServiceRegistry serviceRegistry)
    {
    }

    protected virtual void OnInitialize()
    {
        m_window.Initialize();

        m_imGuiLayer = PushLayer<ImGuiLayer>();
        m_graphicsDevice = m_serviceContainer.GetInstance<IGraphicsDevice>();
    }

    public void Close()
    {
        m_window.Close();
    }
    
    public static void Exit()
    {
        s_application.Close();
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
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
        {
            if (layer.IsEnabled)
            {
                layer.OnEvent(e);
            }
        }
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
        {
            // Everything registered in service container will get disposed
            m_serviceContainer.Dispose();
        }

        m_disposed = true;
    }
}