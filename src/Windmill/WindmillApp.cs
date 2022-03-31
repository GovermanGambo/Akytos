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

    internal override void Configure(AppConfigurator configurator)
    {
        configurator.EnableImGui = true;
    }

    protected override void OnRestart()
    {
        // Starts a new instance of the program itself
        System.Diagnostics.Process.Start("Windmill.exe");
    }

    protected override void RegisterServices(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterFrom<EditorCompositionRoot>();
    }
}