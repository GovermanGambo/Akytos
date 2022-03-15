using LightInject;

namespace Akytos;

public class ScenesCompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<SpriteRendererSystem>();
        
        // TODO: Check for custom implementation?
        serviceRegistry.RegisterSingleton<SceneTree>();
        serviceRegistry.Register<SceneLoader>();
    }
}