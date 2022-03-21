using Akytos.Graphics;

namespace Akytos.Assets;

internal class AssetManager : IAssetManager
{
    private readonly Dictionary<string, IAsset> m_loadedAssets;
    private readonly IGraphicsResourceFactory m_graphicsResourceFactory;
    
    public AssetManager(IGraphicsResourceFactory graphicsResourceFactory)
    {
        m_graphicsResourceFactory = graphicsResourceFactory;
        m_loadedAssets = new Dictionary<string, IAsset>();
    }

    public IEnumerable<string> LoadedAssets => m_loadedAssets.Keys;

    public void LoadAll()
    {
        var directoryInfo = new DirectoryInfo(Asset.AssetsDirectory);
        var allFiles = directoryInfo.GetDirectories().SelectMany(d => d.GetFiles());

        foreach (var fileInfo in allFiles)
        {
            ProcessAssetFile(fileInfo);
        }
    }

    public IAsset<T>? Load<T>(string filename)
    {
        try
        {
            if (m_loadedAssets.TryGetValue(filename, out var asset))
            {
                return asset as IAsset<T>;
            }

            if (typeof(ITexture2D).IsAssignableFrom(typeof(T)))
            {
                string path = Asset.GetAssetPath(filename);
                var texture = m_graphicsResourceFactory.CreateTexture2D(path);
                asset = new Texture2DAsset(texture, filename);
                m_loadedAssets.Add(filename, asset);
                return asset as IAsset<T>;
            }
            
            if (typeof(IShaderProgram).IsAssignableFrom(typeof(T)))
            {
                string path = Asset.GetAssetPath(filename);
                var shader = m_graphicsResourceFactory.CreateShader(path);
                asset = new ShaderAsset(shader, filename);
                m_loadedAssets.Add(filename, asset);
                return asset as IAsset<T>;
            }

            throw new NotSupportedException();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load file {0}: {1}", filename, e.Message);
            return null;
        }
    }

    private void ProcessAssetFile(FileSystemInfo fileInfo)
    {
        string filename = Asset.GetRelativePath(fileInfo.FullName).Replace("\\", "/");
        
        if (fileInfo.FullName.EndsWith(".png"))
        {
            Load<Texture2DAsset>(filename);
        }
        else if (fileInfo.FullName.EndsWith(".glsl"))
        {
            Load<ShaderAsset>(filename);
        }
        else
        {
            // Do nothing.
        }
    }
}