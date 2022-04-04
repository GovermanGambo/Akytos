using Akytos;
using LightInject;

namespace Windmill;

public class WindmillApp : Application
{
    protected override void OnInitialize()
    {
        base.OnInitialize();
        
        PushLayer<EditorLayer>();
    }

    internal override void Configure(AppConfigurator configurator)
    {
        configurator.Title = "Akytos Windmill";
        configurator.Width = 1920;
        configurator.Height = 1080;
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