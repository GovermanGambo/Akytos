
namespace Akytos.Assets;

public static class Asset
{
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