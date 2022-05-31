using Akytos;
using Akytos.Configuration;
using Akytos.Graphics;
using Akytos.SceneSystems;
using LightInject;
using Serilog;
using Serilog.Events;
using Windmill.Diagnostics.Logging;

namespace Windmill;

public class WindmillApp : Application
{
    private ConsoleService m_consoleService = null!;
    
    protected override void OnRestart()
    {
        // Starts a new instance of the program itself
        System.Diagnostics.Process.Start("Windmill.exe");
    }

    protected override void Configure(IAppConfigurator configurator)
    {
        configurator.ConfigureLayers(layers =>
        {
            layers.AddLayer<EditorLayer>();
            layers.AddImGuiLayer();
        });

        configurator.ConfigureGame(game =>
        {
            game.SetWindowTitle("Akytos Windmill");
            game.SetInitialWindowSize(1920, 1080);
        });

        configurator.ConfigureLogging(ConfigureLogs);
    }

    private void ConfigureLogs(LoggerConfiguration coreConfiguration, LoggerConfiguration clientConfiguration)
    {
        m_consoleService = new ConsoleService();
        
        coreConfiguration
#if DEBUG_EDITOR || DEBUG
            .WriteTo.Console()
            .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
            .WriteTo.File(SystemConstants.FileSystem.LogOutputFile, rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
            .WriteTo.ConsoleService(m_consoleService, LogEventLevel.Error);

        clientConfiguration
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.ConsoleService(m_consoleService);
    }

    protected override void RegisterServices(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.AddGraphics(GraphicsBackend.OpenGL);
        serviceRegistry.AddSceneSystems(SceneProcessMode.Editor);
        serviceRegistry.RegisterFrom<EditorCompositionRoot>();
        serviceRegistry.RegisterSingleton(_ => m_consoleService);
    }
}