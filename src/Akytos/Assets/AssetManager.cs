using Akytos.Diagnostics.Logging;
using Akytos.Graphics;
using Veldrid;

namespace Akytos.Assets;

internal class AssetManager : IAssetManager
{
    private readonly Dictionary<string, Asset> m_loadedAssets;
    private readonly IResourceFactory m_resourceFactory;
    
    public AssetManager(IResourceFactory resourceFactory)
    {
        m_resourceFactory = resourceFactory;
        m_loadedAssets = new Dictionary<string, Asset>();
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

    public Asset<T>? Load<T>(string filename)
    {
        try
        {
            if (m_loadedAssets.TryGetValue(filename, out var asset))
            {
                return asset as Asset<T>;
            }

            if (typeof(Texture).IsAssignableFrom(typeof(T)))
            {
                string path = Asset.GetAssetPath(filename);
                var texture = m_resourceFactory.CreateTexture(path);
                asset = new Asset<Texture>(filename, texture);
                m_loadedAssets.Add(filename, asset);
                return asset as Asset<T>;
            }
            
            if (typeof(Shader).IsAssignableFrom(typeof(T)))
            {
                string path = Asset.GetAssetPath(filename);
                var shader = m_resourceFactory.CreateShader(path);
                asset = new Asset<ShaderProgram>(filename, shader);
                m_loadedAssets.Add(filename, asset);
                return asset as Asset<T>;
            }

            throw new NotSupportedException();
        }
        catch (Exception e)
        {
            Log.Core.Error(e, "Failed to load file {0}: {1}", filename, e.Message);
            return null;
        }
    }

    private void ProcessAssetFile(FileSystemInfo fileInfo)
    {
        string filename = Asset.GetRelativePath(fileInfo.FullName).Replace("\\", "/");
        
        if (fileInfo.FullName.EndsWith(".png"))
        {
            Load<Asset<Texture>>(filename);
        }
        else if (fileInfo.FullName.EndsWith(".glsl"))
        {
            Load<Asset<Shader>>(filename);
        }
        else
        {
            // Do nothing.
        }
    }
}