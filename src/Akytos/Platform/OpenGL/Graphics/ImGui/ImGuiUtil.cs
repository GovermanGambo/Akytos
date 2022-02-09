using System.Diagnostics;
using System.Diagnostics.Contracts;
using Silk.NET.OpenGL;

namespace Akytos.Graphics;

internal static class ImGuiUtil
{
    [Pure]
    public static float Clamp(float value, float min, float max)
    {
        return value < min ? min : value > max ? max : value;
    }

    [Conditional("DEBUG")]
    public static void CheckGlError(this GL gl, string title)
    {
        var error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            Debug.LogError("{0}: {1}", title, error);
        }
    }
}