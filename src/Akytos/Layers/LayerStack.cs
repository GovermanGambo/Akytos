using System.Collections;
using LightInject;

namespace Akytos.Layers;

internal class LayerStack : ILayerStack
{
    private readonly List<ILayer> m_layers;
    private readonly IServiceFactory m_serviceFactory;
    private int m_count;

    public LayerStack(IServiceFactory serviceFactory)
    {
        m_serviceFactory = serviceFactory;

        m_layers = new List<ILayer>();
    }

    public void Dispose()
    {
        foreach (var layer in m_layers)
        {
            layer.Dispose();
        }
    }

    public TLayer PushLayer<TLayer>() where TLayer : ILayer
    {
        var layer = m_serviceFactory.TryGetInstance<TLayer>();

        if (layer == null)
        {
            throw new LayerNotFoundException(typeof(TLayer));
        }
        
        m_layers.Insert(m_count, layer);
        m_count++;

        return layer;
    }

    public TOverlay PushOverlay<TOverlay>() where TOverlay : ILayer
    {
        var overlay = m_serviceFactory.TryGetInstance<TOverlay>();

        if (overlay == null)
        {
            throw new LayerNotFoundException(typeof(TOverlay));
        }
        
        m_layers.Add(overlay);

        return overlay;
    }
    
    public IEnumerator<ILayer> GetEnumerator()
    {
        return m_layers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}