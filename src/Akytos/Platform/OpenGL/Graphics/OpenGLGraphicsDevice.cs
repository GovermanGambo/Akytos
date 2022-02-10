using Silk.NET.OpenGL;

namespace Akytos.Graphics;

internal class OpenGLGraphicsDevice : IGraphicsDevice
{
    private readonly GL m_gl;

    public OpenGLGraphicsDevice(GL gl)
    {
        m_gl = gl;
        
        m_gl.Enable(EnableCap.Blend);
        m_gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    public void Clear()
    {
        m_gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void ClearColor(Color color)
    {
        m_gl.ClearColor(color.R, color.G, color.B, color.A);
    }

    public unsafe void DrawIndexed(IVertexArrayObject vertexArrayObject)
    {
        m_gl.DrawElements(PrimitiveType.Triangles, (uint)vertexArrayObject.ElementCount, DrawElementsType.UnsignedInt, null);
    }
}