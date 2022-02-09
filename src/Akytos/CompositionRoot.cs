using Akytos.Graphics;
using LightInject;

namespace Akytos;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton<ImGuiLayer>();
    }
}