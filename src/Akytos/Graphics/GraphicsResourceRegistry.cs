using System.Collections;

namespace Akytos.Graphics;

internal class GraphicsResourceRegistry : IEnumerable<IGraphicsResource>, IDisposable
{
    private readonly HashSet<IGraphicsResource> m_graphicsResources;

    private bool m_disposed;

    public GraphicsResourceRegistry()
    {
        m_graphicsResources = new HashSet<IGraphicsResource>();
    }

    public void Register(IGraphicsResource graphicsResource)
    {
        m_graphicsResources.Add(graphicsResource);
    }

    public IEnumerator<IGraphicsResource> GetEnumerator()
    {
        return m_graphicsResources.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        Dispose(true);
        
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (m_disposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (var graphicsResource in m_graphicsResources)
            {
                graphicsResource.Dispose();
            }
        }

        m_disposed = true;
    }
}