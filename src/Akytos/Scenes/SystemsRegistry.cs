using LightInject;

namespace Akytos;

internal class SystemsRegistry
{
    private readonly IServiceContainer m_serviceContainer;
    private readonly HashSet<ISceneSystem> m_registeredSystems;

    public SystemsRegistry(IServiceContainer serviceContainer)
    {
        m_serviceContainer = serviceContainer;
        m_registeredSystems = new HashSet<ISceneSystem>();
    }

    public TSystem Register<TSystem>(bool enable = true) where TSystem : ISceneSystem
    {
        var system = m_serviceContainer.TryGetInstance<TSystem>();

        if (system is null)
        {
            m_serviceContainer.Register<TSystem>();
            system = m_serviceContainer.GetInstance<TSystem>();
        }

        m_registeredSystems.Add(system);

        system.IsEnabled = enable;

        return system;
    }

    public void OnUpdate(DeltaTime deltaTime)
    {
        foreach (var sceneSystem in m_registeredSystems)
        {
            if (sceneSystem.IsEnabled)
            {
                sceneSystem.OnUpdate(deltaTime);
            }
        }
    }

    public void OnRender()
    {
        foreach (var sceneSystem in m_registeredSystems)
        {
            if (sceneSystem.IsEnabled)
            {
                sceneSystem.OnRender();
            }
        }
    }
}