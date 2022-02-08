using Akytos;
using LightInject;

namespace Windmill;

public class WindmillApp : Application
{
    public WindmillApp(string title, int initialWindowWidth, int initialWindowHeight) : base(title, initialWindowWidth, initialWindowHeight)
    {
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        
        PushLayer<EditorLayer>();
    }

    protected override void RegisterServices(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterFrom<EditorCompositionRoot>();
    }
}