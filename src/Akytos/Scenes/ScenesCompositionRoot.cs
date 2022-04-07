using LightInject;

namespace Akytos;

public class ScenesCompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton<SpriteRendererSystem>();
        
        // TODO: Check for custom implementation?
        serviceRegistry.Register<SceneLoader>();
    }
}