using Veldrid;

namespace Akytos.Graphics;

public interface IResourceFactory
{
    Framebuffer CreateFramebuffer(FramebufferDescription framebufferDescription);
    Texture CreateTexture(int width, int height, PixelFormat pixelFormat, TextureUsage usage);
    Texture CreateTexture(string filePath, TextureUsage usage = TextureUsage.Sampled);
    ShaderProgram CreateShader(string filePath);
    ShaderProgram CreateShader(Stream fileStream);
}