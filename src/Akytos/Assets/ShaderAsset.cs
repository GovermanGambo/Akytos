using Akytos.Graphics;

namespace Akytos.Assets;

public class ShaderAsset : IAsset<IShaderProgram>
{
    public ShaderAsset()
    {
        FilePath = string.Empty;
    }
    
    public ShaderAsset(IShaderProgram? data, string filePath)
    {
        Data = data;
        FilePath = filePath;
    }

    public string FilePath { get; }
    public IShaderProgram? Data { get; }
}