namespace Akytos.Assets;

public interface IAssetManager
{
    /// <summary>
    ///     A list of file paths to the currently loaded assets.
    /// </summary>
    IEnumerable<string> LoadedAssets { get; }
    /// <summary>
    ///     Loads all assets located in the "assets" folder.
    /// </summary>
    void LoadAll();
    /// <summary>
    ///     Loads the specified asset. The requested asset will be cached for later use.
    /// </summary>
    /// <param name="filename">The file path relative to the assets folder.</param>
    /// <typeparam name="T">The type of asset to load.</typeparam>
    /// <returns>An instance of <see cref="IAsset"/> containing the requested asset.</returns>
    IAsset<T>? Load<T>(string filename);
}