using Akytos.Graphics;
using LightInject;

namespace Akytos.Assets;

public class AssetCompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton<IAssetManager, AssetManager>();
        serviceRegistry.Register<IAsset<ITexture2D>, Texture2DAsset>();
    }
}