namespace Akytos.Assets;

public interface IAsset<out T> : IAsset
{
    T? Data { get; }
}

public interface IAsset
{
    string FilePath { get; }
}