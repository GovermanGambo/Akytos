using System.Runtime.InteropServices;

namespace Akytos;

public static class PlatformConstants
{
    public static string NewLine
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "\n";
            }
            
            return "\r\n";
        }
    }
}