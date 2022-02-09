using Silk.NET.OpenGL;

namespace Akytos.Graphics;

internal enum TextureCoordinate
{
    S = TextureParameterName.TextureWrapS,
    T = TextureParameterName.TextureWrapT,
    R = TextureParameterName.TextureWrapR
}

internal class ImGuiTexture : IDisposable
{
    public const SizedInternalFormat Srgb8Alpha8 = (SizedInternalFormat)GLEnum.Srgb8Alpha8;
    public const SizedInternalFormat Rgb32F = (SizedInternalFormat)GLEnum.Rgb32f;

    public const GLEnum MaxTextureMaxAnisotropy = (GLEnum)0x84FF;

    public static float? MaxAniso;
    private readonly GL m_gl;
    public readonly uint GlTexture;
    public readonly SizedInternalFormat InternalFormat;
    public readonly uint MipmapLevels;
    public readonly string Name;
    public readonly uint Width, Height;

    public unsafe ImGuiTexture(GL gl, int width, int height, IntPtr data, bool generateMipmaps = false,
        bool srgb = false)
    {
        m_gl = gl;
        MaxAniso ??= gl.GetFloat(MaxTextureMaxAnisotropy);
        Width = (uint)width;
        Height = (uint)height;
        InternalFormat = srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;
        MipmapLevels = (uint)(generateMipmaps == false ? 1 : (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2)));

        GlTexture = m_gl.GenTexture();
        Bind();

        var pxFormat = PixelFormat.Bgra;

        m_gl.TexStorage2D(GLEnum.Texture2D, MipmapLevels, InternalFormat, Width, Height);
        m_gl.TexSubImage2D(GLEnum.Texture2D, 0, 0, 0, Width, Height, pxFormat, PixelType.UnsignedByte, (void*)data);

        if (generateMipmaps)
            m_gl.GenerateTextureMipmap(GlTexture);

        SetWrap(TextureCoordinate.S, TextureWrapMode.Repeat);
        SetWrap(TextureCoordinate.T, TextureWrapMode.Repeat);

        m_gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
    }

    public void Dispose()
    {
        m_gl.DeleteTexture(GlTexture);
    }

    public void Bind()
    {
        m_gl.BindTexture(GLEnum.Texture2D, GlTexture);
    }

    public void SetMinFilter(TextureMinFilter filter)
    {
        m_gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMinFilter, (int)filter);
    }

    public void SetMagFilter(TextureMagFilter filter)
    {
        m_gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMagFilter, (int)filter);
    }

    public void SetAnisotropy(float level)
    {
        const TextureParameterName textureMaxAnisotropy = (TextureParameterName)0x84FE;
        m_gl.TexParameter(GLEnum.Texture2D, (GLEnum)textureMaxAnisotropy,
            ImGuiUtil.Clamp(level, 1, MaxAniso.GetValueOrDefault()));
    }

    public void SetLod(int @base, int min, int max)
    {
        m_gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureLodBias, @base);
        m_gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMinLod, min);
        m_gl.TexParameterI(GLEnum.Texture2D, TextureParameterName.TextureMaxLod, max);
    }

    public void SetWrap(TextureCoordinate coord, TextureWrapMode mode)
    {
        m_gl.TexParameterI(GLEnum.Texture2D, (TextureParameterName)coord, (int)mode);
    }
}