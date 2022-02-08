using Silk.NET.OpenGL;

namespace Akytos.Graphics.Buffers;

internal sealed class OpenGLBufferObject<TData> : IBufferObject<TData> where TData : unmanaged
{
    private readonly BufferTargetARB m_bufferTargetArb;
    private readonly GL m_gl;

    public OpenGLBufferObject(GL gl, BufferTarget bufferTarget, Span<TData> data)
    {
        m_gl = gl;
        m_bufferTargetArb = (BufferTargetARB) bufferTarget;
        Length = data.Length;

        Handle = new GraphicsHandle(m_gl.GenBuffer());
        m_gl.BindBuffer(m_bufferTargetArb, Handle);

        m_gl.BufferData<TData>(m_bufferTargetArb, data, BufferUsageARB.StaticDraw);
    }

    public unsafe OpenGLBufferObject(GL gl, BufferTarget bufferTarget, int size)
    {
        m_gl = gl;
        m_bufferTargetArb = (BufferTargetARB) bufferTarget;

        Handle = new GraphicsHandle(m_gl.GenBuffer());
        m_gl.BindBuffer(m_bufferTargetArb, Handle);

        m_gl.BufferData(m_bufferTargetArb, (uint) size, null, BufferUsageARB.DynamicDraw);
    }

    public void Dispose()
    {
        m_gl.DeleteBuffer(Handle);
    }

    public int Length { get; }

    public void Bind()
    {
        m_gl.BindBuffer(m_bufferTargetArb, Handle);
    }

    public void Unbind()
    {
        m_gl.BindBuffer(m_bufferTargetArb, 0);
    }

    public void SetData(Span<TData> data)
    {
        Bind();

        m_gl.BufferSubData<TData>(m_bufferTargetArb, 0, data);

        Unbind();
    }

    public GraphicsHandle Handle { get; }
}