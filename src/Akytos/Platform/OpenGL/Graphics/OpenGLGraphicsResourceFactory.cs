using Akytos.Graphics.Buffers;
using Silk.NET.OpenGL;

namespace Akytos.Graphics;

internal class OpenGLGraphicsResourceFactory : IGraphicsResourceFactory
{
    private readonly GL m_gl;
    private readonly GraphicsResourceRegistry m_graphicsResourceRegistry;

    public OpenGLGraphicsResourceFactory(GL gl, GraphicsResourceRegistry graphicsResourceRegistry)
    {
        m_gl = gl;
        m_graphicsResourceRegistry = graphicsResourceRegistry;
    }
    
    public IBufferObject<TData> CreateBuffer<TData>(BufferTarget bufferTarget, Span<TData> data = new()) where TData : unmanaged
    {
        var bufferObject = new OpenGLBufferObject<TData>(m_gl, bufferTarget, data);
        m_graphicsResourceRegistry.Register(bufferObject);
        return bufferObject;
    }

    public IBufferObject<TData> CreateBuffer<TData>(BufferTarget bufferTarget, int length) where TData : unmanaged
    {
        var bufferObject = new OpenGLBufferObject<TData>(m_gl, bufferTarget, length);
        m_graphicsResourceRegistry.Register(bufferObject);
        return bufferObject;
    }

    public IFramebuffer CreateFramebuffer(FrameBufferSpecification specification)
    {
        var framebuffer = new OpenGLFramebuffer(m_gl, specification);
        m_graphicsResourceRegistry.Register(framebuffer);
        return framebuffer;
    }

    public IShaderProgram CreateShader(string filePath)
    {
        var shader = new OpenGLShaderProgram(m_gl, filePath);
        m_graphicsResourceRegistry.Register(shader);
        return shader;
    }

    public IShaderProgram CreateShader(string name, Stream fileStream)
    {
        var shader = new OpenGLShaderProgram(m_gl, name, fileStream);
        m_graphicsResourceRegistry.Register(shader);
        return shader;
    }

    public ITexture2D CreateTexture2D(string filePath)
    {
        var texture = new OpenGLTexture2D(m_gl, filePath);
        m_graphicsResourceRegistry.Register(texture);
        return texture;
    }

    public ITexture2D CreateTexture2D(Span<byte> data, int width, int height)
    {
        var texture = new OpenGLTexture2D(m_gl, data, width, height);
        m_graphicsResourceRegistry.Register(texture);
        return texture;
    }

    public IVertexArrayObject<TArray, TElement> CreateVertexArray<TArray, TElement>() where TArray : unmanaged where TElement : unmanaged
    {
        var vertexArrayObject = new OpenGLVertexArrayObject<TArray, TElement>(m_gl);
        m_graphicsResourceRegistry.Register(vertexArrayObject);
        return vertexArrayObject;
    }
}