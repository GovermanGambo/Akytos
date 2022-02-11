namespace Akytos.Graphics.Buffers;

public enum FramebufferTextureFormat
{
    None = 0,
    Rgba8,
    RedInteger,
    Depth24Stencil8,
    Depth = Depth24Stencil8
}

public static class FramebufferTextureFormatExtensions
{
    public static bool IsDepthFormat(this FramebufferTextureFormat framebufferTextureFormat)
    {
        return framebufferTextureFormat == FramebufferTextureFormat.Depth24Stencil8;
    }
}