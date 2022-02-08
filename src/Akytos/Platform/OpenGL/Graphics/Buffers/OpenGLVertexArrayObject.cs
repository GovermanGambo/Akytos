using Silk.NET.OpenGL;

namespace Akytos.Graphics.Buffers;

internal class OpenGLVertexArrayObject<TArray, TElement> : IVertexArrayObject<TArray, TElement>
    where TArray : unmanaged where TElement : unmanaged
{
    private readonly GL m_gl;
    private readonly List<IBufferObject<TArray>> m_arrayBufferObjects;
    
    private IBufferObject<TElement>? m_elementArrayBufferObject;
    private BufferLayout? m_bufferLayout;

    public OpenGLVertexArrayObject(GL gl)
    {
        m_gl = gl;

        m_arrayBufferObjects = new List<IBufferObject<TArray>>();

        Handle = new GraphicsHandle(m_gl.GenVertexArray());
    }

    public void Dispose()
    {
        m_gl.DeleteVertexArray(Handle);
    }

    public GraphicsHandle Handle { get; }
    
    public int ElementCount => m_elementArrayBufferObject?.Length ?? 0;
    
    public unsafe void AddArrayBuffer(IBufferObject<TArray> arrayBuffer)
    {
        if (m_bufferLayout == null)
        {
            return;
        }
        
        Bind();
        arrayBuffer.Bind();

        uint index = 0;

        foreach (var element in m_bufferLayout)
        {
            switch (element.DataType)
            {
                case ShaderDataType.Float:
                case ShaderDataType.Float2:
                case ShaderDataType.Float3:
                case ShaderDataType.Float4:
                {
                    m_gl.EnableVertexAttribArray(index);
                    m_gl.VertexAttribPointer(index,
                        element.ComponentCount,
                        element.DataType.ToGLShaderType(),
                        element.IsNormalized,
                        (uint) m_bufferLayout.Stride,
                        (void*) element.Offset);
                    index++;
                    break;
                }
                case ShaderDataType.Int:
                case ShaderDataType.Int2:
                case ShaderDataType.Int3:
                case ShaderDataType.Int4:
                case ShaderDataType.Bool:
                {
                    m_gl.EnableVertexAttribArray(index);
                    m_gl.VertexAttribIPointer(index,
                        element.ComponentCount,
                        element.DataType.ToGLShaderType(),
                        (uint) m_bufferLayout.Stride,
                        (void*) element.Offset);
                    index++;
                    break;
                }
                default:
                    throw new NotSupportedException();
            }
        }

        m_arrayBufferObjects.Add(arrayBuffer);
        
        arrayBuffer.Unbind();
    }

    public void SetElementArrayBuffer(IBufferObject<TElement> elementArrayBuffer)
    {
        Bind();
        elementArrayBuffer.Bind();
        m_elementArrayBufferObject = elementArrayBuffer;
    }

    public void SetBufferLayout(BufferLayout bufferLayout)
    {
        m_bufferLayout = bufferLayout;
    }

    public void Bind()
    {
        m_gl.BindVertexArray(Handle);
    }
}