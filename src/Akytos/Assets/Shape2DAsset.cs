using Akytos.Physics;

namespace Akytos.Assets;

public class Shape2DAsset : IAsset<IShape2D>
{
    public Shape2DAsset(string filePath)
    {
        FilePath = filePath;
    }

    public Shape2DAsset(string filePath, IShape2D data)
    {
        FilePath = filePath;
        Data = data;
    }

    public string FilePath { get; }
    public IShape2D? Data { get; }
}