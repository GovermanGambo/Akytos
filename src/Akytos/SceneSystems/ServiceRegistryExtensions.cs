using LightInject;

namespace Akytos.SceneSystems;

public static class ServiceRegistryExtensions
{
    public static IServiceRegistry AddSceneSystems(this IServiceRegistry serviceRegistry, SceneProcessMode sceneProcessMode = SceneProcessMode.Runtime)
    {
        serviceRegistry.Register<ISystemsRegistry, SystemsRegistry>();
        
        serviceRegistry.RegisterSingleton(factory => new SceneTree(factory.GetInstance<ISystemsRegistry>(), sceneProcessMode));
        serviceRegistry.RegisterSingleton<SpriteRendererSystem>();
        serviceRegistry.Register<SceneLoader>();

        return serviceRegistry;
    }
}