namespace Akytos.Graphics.Buffers;

public readonly struct FramebufferTextureSpecification
{
    public FramebufferTextureSpecification(FramebufferTextureFormat textureFormat)
    {
        TextureFormat = textureFormat;
    }

    public FramebufferTextureFormat TextureFormat { get; } = FramebufferTextureFormat.None;
}