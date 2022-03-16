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
        m_gl.Enable(EnableCap.DepthTest);
        m_gl.DepthFunc(DepthFunction.Lequal);
    }

    public void Clear()
    {
        m_gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void ClearColor(Color color)
    {
        m_gl.ClearColor(color.R, color.G, color.B, color.A);
    }

    public void SetViewport(int x, int y, int width, int height)
    {
        m_gl.Viewport(x, y, (uint)width, (uint)height);
    }

    public unsafe void DrawIndexed(IVertexArrayObject vertexArrayObject, int elementCount = 0)
    {
        m_gl.DrawElements(PrimitiveType.Triangles, (uint)elementCount, DrawElementsType.UnsignedInt, null);
    }
}