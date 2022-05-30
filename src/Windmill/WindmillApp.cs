using Akytos;
using Akytos.Configuration;
using LightInject;

namespace Windmill;

public class WindmillApp : Application
{
    protected override void OnRestart()
    {
        // Starts a new instance of the program itself
        System.Diagnostics.Process.Start("Windmill.exe");
    }

    protected override void Configure(IAppConfigurator configurator)
    {
        configurator.ConfigureLayers(layers =>
        {
            layers.PushLayer<EditorLayer>();
            layers.AddImGuiLayer();
        });

        configurator.ConfigureGame(game =>
        {
            game.SetWindowTitle("Akytos Windmill");
            game.SetInitialWindowSize(1920, 1080);
        });
    }

    protected override void RegisterServices(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.RegisterFrom<EditorCompositionRoot>();
    }
}