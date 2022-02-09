using System.Diagnostics;
using Silk.NET.OpenGL;

namespace Akytos.Graphics;

internal static class GLUtil
{
    [Conditional("DEBUG")]
    public static void CheckGlError(this GL gl, string title)
    {
        var error = gl.GetError();
        if (error != GLEnum.NoError) Debug.LogError("{0}: {1}", title, error);
    }
}