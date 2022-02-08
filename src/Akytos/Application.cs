using System.Runtime.CompilerServices;
using Akytos.Events;
using Akytos.Layers;
using Akytos.Windowing;
using LightInject;

[assembly: InternalsVisibleTo("Windmill")]
namespace Akytos;

public abstract class Application : IDisposable
{
    private readonly IServiceContainer m_serviceContainer;
    private readonly IGameWindow m_window;
    private readonly ILayerStack m_layerStack;

    private float m_lastFrameTime;
    private bool m_disposed;

    protected Application(string title, int initialWindowWidth, int initialWindowHeight)
    {
        m_serviceContainer = new ServiceContainer();
        m_window = CreateWindow(title, initialWindowWidth, initialWindowHeight);

        m_layerStack = new LayerStack(m_serviceContainer);
    }
    
    public void Run()
    {
        OnInitialize();
        
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
            
            m_window.PollEvents();
        }
        
        Dispose();
    }

    internal TLayer PushLayer<TLayer>() where TLayer : ILayer
    {
        var layer = m_layerStack.PushLayer<TLayer>();
        layer.OnAttach();
        return layer;
    }

    protected virtual void RegisterServices(IServiceRegistry serviceRegistry)
    {
    }

    protected virtual void OnInitialize()
    {
        m_window.Initialize();   
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
        foreach (var layer in m_layerStack.Reverse())
        {
            if (layer.IsEnabled)
            {
                layer.OnEvent(e);
            }
        }
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