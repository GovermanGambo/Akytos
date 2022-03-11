using Akytos.Graphics;

namespace Akytos.Assets;

[Serializable]
public class Texture2DAsset : IAsset<ITexture2D>
{
    private readonly string m_filePath;

    public Texture2DAsset()
    {
        m_filePath = string.Empty;
    }
    
    public Texture2DAsset(ITexture2D data, string filePath)
    {
        Data = data;
        m_filePath = filePath;
    }

    public string FilePath => m_filePath;

    public ITexture2D? Data { get; }
}