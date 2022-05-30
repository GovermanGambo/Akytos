using Akytos;
using Akytos.Configuration;

namespace Pong;

public class PongApp : Application
{
    protected override void Configure(IAppConfigurator configurator)
    {
        configurator.ConfigureLayers(layers => layers.PushLayer<GameLayer>());
        configurator.ConfigureGame(game =>
        {
            game.SetWindowTitle("Pong");
            game.SetInitialWindowSize(640, 480);
        });
    }

    protected override void OnRestart()
    {
        
    }
}