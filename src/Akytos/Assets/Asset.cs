namespace Akytos.Assets;

public class Asset<TData> : Asset
{
    public Asset(string filePath, TData data)
        : base(filePath)
    {
        Data = data;
    }

    public TData Data { get; }
}

public abstract class Asset
{
    protected Asset(string filePath)
    {
        FilePath = filePath;
    }

    public string Name { get; }
    public string FilePath { get; }

    
    
    public static string GetAssetPath(string path)
    {
        return Path.Combine(AssetsDirectory, path);
    }

    public static string GetRelativePath(string path)
    {
        return Path.GetRelativePath(AssetsDirectory, path);
    }

    public static string AssetsDirectory => GetWorkingDirectory(SystemConstants.FileSystem.AssetsSubDirectory);
    
    private static string GetWorkingDirectory(string path)
    {
        string directoryName = Application.WorkingDirectory;

        string fullPath = Path.Combine(directoryName,
            path);
        return fullPath;
    }
}