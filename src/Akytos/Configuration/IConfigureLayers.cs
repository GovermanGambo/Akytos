using Akytos.Layers;

namespace Akytos.Configuration;

public interface IConfigureLayers
{
    void AddLayer<TLayer>() where TLayer : ILayer;
    void AddImGuiLayer();
}