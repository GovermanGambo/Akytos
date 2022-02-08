using LightInject;

namespace Akytos.Windowing;

internal class WindowingCompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<IWindowFactory, WindowsWindowFactory>();
    }
}