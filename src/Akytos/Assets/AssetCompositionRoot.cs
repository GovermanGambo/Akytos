using LightInject;

namespace Akytos.Assets;

public class AssetCompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton<IAssetManager, AssetManager>();
    }
}