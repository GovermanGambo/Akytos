using System.Runtime.InteropServices;
using Akytos.Assertions;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Akytos.Graphics;

[Serializable]
internal class OpenGLTexture2D : ITexture2D
{
    private readonly GL m_gl;

    public unsafe OpenGLTexture2D(GL gl, string filePath)
    {
        m_gl = gl;

        var image = Image.Load<Rgba32>(filePath);
        
        image.Mutate(x => x.Flip(FlipMode.Vertical));

        Width = image.Width;
        Height = image.Height;

        image.ProcessPixelRows(accessor =>
        {
            fixed (void* d = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
            {
                LoadImageData(d, (uint) Width, (uint) Height);
            }
        });

        image.Dispose();
    }

    public unsafe OpenGLTexture2D(GL gl, Span<byte> data, int width, int height)
    {
        m_gl = gl;
        
        Assert.AreEqual(data.Length / 4, width * height, "The data size must match the size of the texture.");

        Width = width;
        Height = height;

        fixed (void* d = data)
        {
            LoadImageData(d, (uint) width, (uint) height);
        }
    }

    public void Dispose()
    {
        m_gl.DeleteTexture(Handle);
    }

    public GraphicsHandle Handle { get; private set; }
    public int Width { get; }
    public int Height { get; }

    public void Bind(int slot = 0)
    {
        m_gl.ActiveTexture(TextureUnit.Texture0 + slot);
        m_gl.BindTexture(TextureTarget.Texture2D, Handle);
    }

    private unsafe void LoadImageData(void* data, uint width, uint height)
    {
        Handle = new GraphicsHandle(m_gl.GenTexture());
        Bind();

        m_gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
            PixelType.UnsignedByte, data);

        m_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.Repeat);
        m_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.Repeat);
        m_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Nearest);
        m_gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Nearest);

        m_gl.GenerateMipmap(TextureTarget.Texture2D);
    }
}