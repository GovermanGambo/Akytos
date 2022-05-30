using Akytos.Layers;

namespace Akytos.Configuration;

public interface IConfigureLayers
{
    TLayer PushLayer<TLayer>() where TLayer : ILayer;
    void AddImGuiLayer();
}