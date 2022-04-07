using Akytos.Analytics;
using Akytos.Assertions;
using Silk.NET.OpenGL;
using Boolean = Silk.NET.OpenGL.Boolean;

namespace Akytos.Graphics.Buffers;

internal class OpenGLFramebuffer : IFramebuffer
{
    private const uint MaxFrameBufferSize = 8192;

    private readonly List<FramebufferTextureSpecification> m_colorAttachmentSpecifications;
    private readonly FramebufferTextureSpecification m_depthAttachmentSpecification;

    private readonly GL m_gl;
    private List<uint> m_colorAttachments;
    private uint m_depthAttachment;

    private int[] m_windowViewportSize = new int[4];

    public OpenGLFramebuffer(GL gl, FrameBufferSpecification framebufferSpecification)
    {
        m_gl = gl;
        Specification = framebufferSpecification;

        m_colorAttachmentSpecifications = new List<FramebufferTextureSpecification>();
        m_depthAttachmentSpecification = new FramebufferTextureSpecification();

        if (Specification.Attachments != null)
            foreach (var specification in Specification.Attachments.Attachments)
                if (!specification.TextureFormat.IsDepthFormat())
                    m_colorAttachmentSpecifications.Add(specification);
                else
                    m_depthAttachmentSpecification = specification;

        m_colorAttachments = new List<uint>();
        for (int i = 0; i < m_colorAttachmentSpecifications.Count; i++) m_colorAttachments.Add(0);

        Invalidate();
    }

    public FrameBufferSpecification Specification { get; }

    public uint GetColorAttachmentRendererId(int index = 0)
    {
        return m_colorAttachments[index];
    }

    public int ReadPixel(int attachmentIndex, int x, int y)
    {
        Assert.IsTrue(attachmentIndex < m_colorAttachments.Count);
        m_gl.ReadBuffer(ReadBufferMode.ColorAttachment0 + attachmentIndex);
        m_gl.ReadPixels(x, y, 1, 1, PixelFormat.RedInteger, PixelType.Int, out int pixel);

        return pixel;
    }

    public void Bind()
    {
        m_gl.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);
        var previousDimensions = new Span<int>(m_windowViewportSize);
        m_gl.GetInteger(GLEnum.Viewport, previousDimensions);
        m_gl.Viewport(0, 0, Specification.Width, Specification.Height);
    }

    public void Unbind()
    {
        m_gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        m_gl.Viewport(m_windowViewportSize[0], m_windowViewportSize[1], (uint)m_windowViewportSize[2], (uint)m_windowViewportSize[3]);
    }

    public void Resize(uint width, uint height)
    {
        if (width == 0 || height == 0 || width > MaxFrameBufferSize || height > MaxFrameBufferSize)
            Log.Core.Error($"Attempted to resize frame buffer: {width}, {height}");

        Specification.Width = width;
        Specification.Height = height;

        Invalidate();
    }

    public void ClearAttachment(int attachmentIndex, int value)
    {
        Assert.IsTrue(attachmentIndex < m_colorAttachments.Count);

        var specification = m_colorAttachmentSpecifications[attachmentIndex];
        var format = specification.TextureFormat;

        m_gl.ClearTexImage(m_colorAttachments[attachmentIndex], 0, Utils.GetGLTextureFormat(format), PixelType.Int,
            value);
    }

    public void Dispose()
    {
        m_gl.DeleteFramebuffer(Handle);
        m_gl.DeleteTextures(new ReadOnlySpan<uint>(m_colorAttachments.ToArray()));
        m_gl.DeleteTexture(m_depthAttachment);
    }

    public GraphicsHandle Handle { get; private set; }

    private void Invalidate()
    {
        if (Handle != 0)
        {
            m_gl.DeleteFramebuffer(Handle);
            m_gl.DeleteTextures(new ReadOnlySpan<uint>(m_colorAttachments.ToArray()));
            m_gl.DeleteTexture(m_depthAttachment);

            m_colorAttachments.Clear();
            for (int i = 0; i < m_colorAttachmentSpecifications.Count; i++) m_colorAttachments.Add(0);

            m_depthAttachment = 0;
        }

        Handle = new GraphicsHandle(m_gl.GenFramebuffer());
        m_gl.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);

        bool multisample = Specification.Samples > 1;
        if (m_colorAttachmentSpecifications.Count != 0)
        {
            var colorAttachments = new Span<uint>(m_colorAttachments.ToArray());
            Utils.CreateTextures(m_gl, multisample, (uint) m_colorAttachments.Count, ref colorAttachments);
            m_colorAttachments = new List<uint>(colorAttachments.ToArray());

            for (int i = 0; i < colorAttachments.Length; i++)
            {
                Utils.BindTexture(m_gl, multisample, colorAttachments[i]);
                switch (m_colorAttachmentSpecifications[i].TextureFormat)
                {
                    case FramebufferTextureFormat.Rgba8:
                        Utils.AttachColorTexture(m_gl, m_colorAttachments[i], Specification.Samples,
                            InternalFormat.Rgba8, GLEnum.Rgba,
                            Specification.Width, Specification.Height, i);
                        break;
                    case FramebufferTextureFormat.RedInteger:
                        Utils.AttachColorTexture(m_gl, m_colorAttachments[i], Specification.Samples,
                            InternalFormat.R32i, GLEnum.RedInteger,
                            Specification.Width, Specification.Height, i);
                        break;
                }
            }
        }

        if (m_depthAttachmentSpecification.TextureFormat != FramebufferTextureFormat.None)
        {
            var depthAttachment = new Span<uint>(new[] {m_depthAttachment});
            Utils.CreateTextures(m_gl, multisample, 1, ref depthAttachment);
            m_depthAttachment = depthAttachment[0];
            Utils.BindTexture(m_gl, multisample, m_depthAttachment);
            switch (m_depthAttachmentSpecification.TextureFormat)
            {
                case FramebufferTextureFormat.Depth24Stencil8:
                    Utils.AttachDepthTexture(m_gl, m_depthAttachment, Specification.Samples, GLEnum.Depth24Stencil8,
                        GLEnum.DepthStencilAttachment, Specification.Width, Specification.Height);
                    break;
            }
        }

        if (m_colorAttachments.Count > 1)
        {
            Assert.IsTrue(m_colorAttachments.Count <= 4);
            GLEnum[] buffers =
            {
                GLEnum.ColorAttachment0, GLEnum.ColorAttachment1, GLEnum.ColorAttachment2, GLEnum.ColorAttachment3
            };
            m_gl.DrawBuffers(4, new ReadOnlySpan<GLEnum>(buffers));
        }
        else if (m_colorAttachments.Count == 0)
        {
            m_gl.DrawBuffers(new ReadOnlySpan<GLEnum>());
        }

        var status = m_gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (status != GLEnum.FramebufferComplete) Log.Core.Error($"FrameBuffer is incomplete! ({status})");

        m_gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    private static class Utils
    {
        public static GLEnum GetGLTextureFormat(FramebufferTextureFormat format)
        {
            return format switch
            {
                FramebufferTextureFormat.Rgba8 => GLEnum.Rgba8,
                FramebufferTextureFormat.RedInteger => GLEnum.RedInteger,
                FramebufferTextureFormat.Depth24Stencil8 => GLEnum.Depth24Stencil8,
                _ => GLEnum.None
            };
        }

        public static TextureTarget GetTextureTarget(bool multisampled)
        {
            return multisampled
                ? TextureTarget.Texture2DMultisample
                : TextureTarget.Texture2D;
        }

        public static void CreateTextures(GL gl, bool multisampled, uint count, ref Span<uint> outId)
        {
            gl.CreateTextures(GetTextureTarget(multisampled), count, outId);
        }

        public static void BindTexture(GL gl, bool multisampled, uint id)
        {
            gl.BindTexture(GetTextureTarget(multisampled), id);
        }

        public static unsafe void AttachColorTexture(GL gl, uint id, uint specificationSamples,
            InternalFormat internalFormat, GLEnum format, uint width, uint height, int index)
        {
            bool multisampled = specificationSamples > 1;
            if (multisampled)
            {
                gl.TexImage2DMultisample(TextureTarget.Texture2DMultisample, specificationSamples, format, width,
                    height, Boolean.False);
            }
            else
            {
                gl.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, width, height, 0, format,
                    PixelType.UnsignedByte, null);

                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int) GLEnum.ClampToEdge);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
            }

            gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + index,
                GetTextureTarget(multisampled), id, 0);
        }

        public static void AttachDepthTexture(GL gl, uint id, uint specificationSamples, GLEnum format,
            GLEnum attachmentType, uint width, uint height)
        {
            bool multisampled = specificationSamples > 1;
            if (multisampled)
            {
                gl.TexImage2DMultisample(TextureTarget.Texture2DMultisample, specificationSamples, format, width,
                    height, Boolean.False);
            }
            else
            {
                gl.TexStorage2D(TextureTarget.Texture2D, 1, format, width, height);

                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) GLEnum.ClampToEdge);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) GLEnum.ClampToEdge);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int) GLEnum.ClampToEdge);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) GLEnum.Linear);
                gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) GLEnum.Linear);
            }

            gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachmentType, GetTextureTarget(multisampled), id,
                0);
        }
    }
}