using Akytos;
using Akytos.Configuration;
using Akytos.Graphics;
using LightInject;
using Serilog;

namespace Pong;

public class PongApp : Application
{
    protected override void Configure(IAppConfigurator configurator)
    {
        configurator.ConfigureLayers(layers => layers.AddLayer<Game>());
        configurator.ConfigureGame(game =>
        {
            game.SetWindowTitle("Pong");
            game.SetInitialWindowSize(640, 480);
        });
        configurator.ConfigureLogging((coreConfiguration, clientConfiguration) =>
        {
            coreConfiguration
                .WriteTo.Console()
                .MinimumLevel.Information();
            
            clientConfiguration
                .WriteTo.Console()
                .MinimumLevel.Debug();
        });
    }

    protected override void RegisterServices(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.AddGraphics(GraphicsBackend.OpenGL);
    }

    protected override void OnRestart()
    {
        
    }
}