using Akytos.Editor;
using Akytos.Windowing;
using LightInject;

namespace Windmill;

public class EditorCompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterSingleton<EditorLayer>();
        serviceRegistry.RegisterSingleton<IEditorViewport>(factory =>
        {
            var window = factory.GetInstance<IGameWindow>();
            return new EditorViewport(window.Width, window.Height);
        });
        
        RegisterPanels(serviceRegistry);
    }

    private void RegisterPanels(IServiceRegistry serviceRegistry)
    {
        
    }
}