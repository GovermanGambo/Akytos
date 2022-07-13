using Veldrid;

namespace Akytos.Graphics;

internal class FramebufferService
{
    private readonly IResourceFactory m_resourceFactory;
    private readonly GraphicsResourceRegistry m_graphicsResourceRegistry;
    private readonly Dictionary<string, FramebufferSet> m_framebuffers;

    public FramebufferService(IResourceFactory resourceFactory, GraphicsResourceRegistry graphicsResourceRegistry)
    {
        m_resourceFactory = resourceFactory;
        m_graphicsResourceRegistry = graphicsResourceRegistry;
        m_framebuffers = new Dictionary<string, FramebufferSet>();
    }

    public Framebuffer CreateFramebuffer(string name, FramebufferDescription description)
    {
        var framebuffer = m_resourceFactory.CreateFramebuffer(description);
        m_framebuffers.Add(name, new FramebufferSet(framebuffer, description));
        return framebuffer;
    }

    public Framebuffer GetFramebuffer(string name)
    {
        return m_framebuffers[name].Framebuffer;
    }

    public void ResizeFramebuffer(string name, int width, int height)
    {
        if (!m_framebuffers.TryGetValue(name, out var framebufferSet))
        {
            return;
        }

        framebufferSet.Framebuffer.Dispose();
        m_framebuffers.Remove(name);
        m_graphicsResourceRegistry.Destroy(framebufferSet.Framebuffer);

        var rgbaAttachment = m_resourceFactory.CreateTexture(width, height,
            PixelFormat.R8_G8_B8_A8_UInt, TextureUsage.RenderTarget);
        var redIntegerAttachment = m_resourceFactory.CreateTexture(width, height,
            PixelFormat.R32_UInt, TextureUsage.RenderTarget);
        var depthAttachment = m_resourceFactory.CreateTexture(width, height,
            PixelFormat.D24_UNorm_S8_UInt, TextureUsage.DepthStencil);
            
        var framebufferDescription = new FramebufferDescription(
            new FramebufferAttachmentDescription(depthAttachment, 0),
            new[]
            {
                new FramebufferAttachmentDescription(rgbaAttachment, 0),
                new FramebufferAttachmentDescription(redIntegerAttachment, 0)
            });

        CreateFramebuffer(name, framebufferDescription);
    }
    
    private class FramebufferSet
    {
        public FramebufferSet(Framebuffer framebuffer, FramebufferDescription description)
        {
            Framebuffer = framebuffer;
            Description = description;
        }

        public Framebuffer Framebuffer { get; }
        public FramebufferDescription Description { get; }
    }
}

