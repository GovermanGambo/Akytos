namespace Akytos.Layers;

internal interface ILayerStack : IEnumerable<ILayer>, IDisposable
{
    TLayer PushLayer<TLayer>() where TLayer : ILayer;
    TOverlay PushOverlay<TOverlay>() where TOverlay : ILayer;
}