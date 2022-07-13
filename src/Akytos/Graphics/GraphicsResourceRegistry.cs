using System.Collections;
using Veldrid;

namespace Akytos.Graphics;

internal class GraphicsResourceRegistry : IEnumerable<DeviceResource>, IDisposable
{
    private readonly HashSet<DeviceResource> m_graphicsResources;

    private bool m_disposed;

    public GraphicsResourceRegistry()
    {
        m_graphicsResources = new HashSet<DeviceResource>();
    }

    public void Register(DeviceResource graphicsResource)
    {
        m_graphicsResources.Add(graphicsResource);
    }
    
    public void Destroy(DeviceResource resource)
    {
        if (resource is IDisposable disposable)
        {
            disposable.Dispose();
        }

        m_graphicsResources.Remove(resource);
    }

    public IEnumerator<DeviceResource> GetEnumerator()
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
                if (graphicsResource is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                
            }
        }

        m_disposed = true;
    }
}