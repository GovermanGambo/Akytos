namespace Akytos.Graphics;

public struct GraphicsHandle
{
    private readonly uint m_id;

    public static implicit operator uint(GraphicsHandle graphicsHandle) => graphicsHandle.m_id;

    public GraphicsHandle(uint id)
    {
        m_id = id;
    }
}