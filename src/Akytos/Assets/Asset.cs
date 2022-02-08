using System.Reflection;

namespace Akytos.Assets;

public static class Asset
{
    public static string GetAssetPath()
    {
        return GetExecutingPath("assets");
    }
    
    private static string GetExecutingPath(string path)
    {
        string? directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (directoryName == null)
            return "";
            
        string fullPath = Path.Combine(directoryName,
            Path.GetFullPath(path));
        return fullPath;
    }
}