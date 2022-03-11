using Akytos.Graphics;

namespace Akytos.Assets;

internal class AssetManager
{
    private readonly Dictionary<string, IAsset> m_loadedAssets;
    private readonly IGraphicsResourceFactory m_graphicsResourceFactory;
    
    public AssetManager(IGraphicsResourceFactory graphicsResourceFactory)
    {
        m_graphicsResourceFactory = graphicsResourceFactory;
        m_loadedAssets = new Dictionary<string, IAsset>();
    }

    public IAsset<T> Load<T>(string filename)
    {
        if (typeof(ITexture2D).IsAssignableFrom(typeof(T)))
        {
            string path = Asset.GetAssetPath(filename);
            var texture = m_graphicsResourceFactory.CreateTexture2D(path);
            var asset = new Texture2DAsset(texture, filename);
            m_loadedAssets.Add(filename, asset);
            return asset as IAsset<T>;
        }

        throw new NotSupportedException();
    }
    
    public TAsset Get<TAsset>(string url)
    {
        var asset = m_loadedAssets[url] as IAsset<TAsset>;

        return asset.Data;
    }
}