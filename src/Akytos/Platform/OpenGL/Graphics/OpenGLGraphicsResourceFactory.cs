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

    public IBufferObject<TData> CreateBuffer<TData>(BufferTarget bufferTarget, int size) where TData : unmanaged
    {
        var bufferObject = new OpenGLBufferObject<TData>(m_gl, bufferTarget, size);
        m_graphicsResourceRegistry.Register(bufferObject);
        return bufferObject;
    }

    public IShaderProgram CreateShader(string filePath)
    {
        var shader = new OpenGLShaderProgram(m_gl, filePath);
        m_graphicsResourceRegistry.Register(shader);
        return shader;
    }

    public IVertexArrayObject<TArray, TElement> CreateVertexArray<TArray, TElement>() where TArray : unmanaged where TElement : unmanaged
    {
        var vertexArrayObject = new OpenGLVertexArrayObject<TArray, TElement>(m_gl);
        m_graphicsResourceRegistry.Register(vertexArrayObject);
        return vertexArrayObject;
    }
}